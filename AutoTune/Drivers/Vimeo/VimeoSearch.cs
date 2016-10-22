using AutoTune.Settings;
using AutoTune.Shared;
using System.Linq;

namespace AutoTune.Drivers.Vimeo {

    class VimeoSearch : Search {

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            var client = new VimeoVideoClient(user.VimeoClientId, user.VimeoClientSecret);
            int? page = pageToken == null ? (int?)null : int.Parse(pageToken);
            if (totalResults >= 0 && page != null && page == -1 || (page - 1) * app.SearchPageSize >= totalResults)
                return new SearchResult {
                    State = new SearchState {
                        NextPageToken = "-1",
                        TotalResults = totalResults,
                    },
                    Results = Enumerable.Empty<Result>()
                };
            var videos = client.GetVideos(query, similarTo?.VideoId, page, app.SearchPageSize);
            return new SearchResult {
                State = new SearchState {
                    TotalResults = videos.total,
                    NextPageToken = (videos.page + 1).ToString()
                },
                Results = videos.data.Select(v => new Result {
                    Title = v.name,
                    VideoId = v.id?.ToString(),
                    Description = v.description,
                    ThumbnailUrl = v.pictures.Any() && v.pictures[0].sizes.Any() ? v.pictures[0].sizes[0].link : null
                })
            };
        }
    }
}
