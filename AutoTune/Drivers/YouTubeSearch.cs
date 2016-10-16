using AutoTune.Shared;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Linq;

namespace AutoTune.Drivers {

    class YouTubeSearch : Search {

        public const string TypeId = "YouTube";

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            var apiKeys = ApiKeys.Instance;
            var settings = Settings.Instance;
            var youTube = settings.YouTube;
            var general = settings.General;
            using (var service = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = apiKeys.YouTubeAPIKey,
                ApplicationName = general.AppName
            })) {
                var request = service.Search.List("snippet");
                if (query != null)
                    request.Q = query;
                else
                    request.RelatedToVideoId = similarTo.VideoId;
                request.Type = "video";
                request.PageToken = pageToken;
                request.MaxResults = general.PageSize;
                SearchListResponse response = request.Execute();
                return new SearchResult {
                    State = new SearchState {
                        TotalResults = -1,
                        NextPageToken = response.NextPageToken,
                    },
                    Results = response.Items.Select(i => new Result {
                        Type = TypeId,
                        VideoId = i.Id.VideoId,
                        Title = i.Snippet.Title,
                        Description = i.Snippet.Description,
                        KeepOriginal = youTube.KeepOriginal,
                        ShouldPostProcess = youTube.PostProcess,
                        ThumbnailUrl = i.Snippet.Thumbnails.Default__.Url,
                        Url = string.Format(youTube.UrlPattern, i.Id.VideoId),
                        PlayUrl = string.Format(youTube.PlayUrlPattern, i.Id.VideoId),
                        DownloadUrl = string.Format(youTube.DownloadUrlPattern, i.Id.VideoId)
                    })
                };
            }
        }
    }
}
