using AutoTune.Settings;
using AutoTune.Shared;
using System.Linq;

namespace AutoTune.Drivers {

    class VimeoSearch : Search {

        public const string TypeId = "Vimeo";

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
                    Type = TypeId,
                    Title = v.name,
                    VideoId = v.id?.ToString(),
                    Description = v.description,
                    Url = string.Format(app.Vimeo.UrlPattern, v.id),
                    PlayUrl = string.Format(app.Vimeo.PlayUrlPattern, v.id),
                    DownloadUrl = string.Format(app.Vimeo.DownloadUrlPattern, v.id),
                    ThumbnailUrl = v.pictures.Any() && v.pictures[0].sizes.Any() ? v.pictures[0].sizes[0].link : null
                })
            };
        }
    }
}
