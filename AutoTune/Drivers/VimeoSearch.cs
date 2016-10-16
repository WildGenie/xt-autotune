using AutoTune.Shared;
using System.Linq;
using VimeoDotNet;

namespace AutoTune.Drivers {

    class VimeoSearch : Search {

        public const string TypeId = "Vimeo";

        protected override SearchResult Execute(int totalResults, string pageToken, string query) {
            var settings = Settings.Instance;
            var vimeo = settings.Vimeo;
            var client = new VimeoVideoClient(vimeo.ClientId, vimeo.ClientSecret);
            int? page = pageToken == null ? (int?)null : int.Parse(pageToken);
            if (totalResults >= 0 && page != null && page == -1 || (page - 1) * settings.General.PageSize >= totalResults)
                return new SearchResult {
                    State = new SearchState {
                        NextPageToken = "-1",
                        TotalResults = totalResults,
                    },
                    Results = Enumerable.Empty<Result>()
                };
            var videos = client.GetVideos(query, page, settings.General.PageSize);
            return new SearchResult {
                State = new SearchState {
                    TotalResults = videos.total,
                    NextPageToken = (videos.page + 1).ToString()
                },
                Results = videos.data.Select(v => new Result {
                    Type = TypeId,
                    Title = v.name,
                    Description = v.description,
                    KeepOriginal = vimeo.KeepOriginal,
                    PlayUrl = string.Format(vimeo.PlayUrlPattern, v.id),
                    ShouldPostProcess = Settings.Instance.Vimeo.PostProcess,
                    DownloadUrl = string.Format(vimeo.DownloadUrlPattern, v.id),
                    ThumbnailUrl = v.pictures.Any() && v.pictures[0].sizes.Any() ? v.pictures[0].sizes[0].link : null
                })
            };
        }
    }
}
