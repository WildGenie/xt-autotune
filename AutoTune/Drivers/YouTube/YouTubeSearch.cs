using AutoTune.Settings;
using AutoTune.Shared;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Linq;

namespace AutoTune.Drivers.YouTube {

    class YouTubeSearch : Search {

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            using (var service = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = user.YouTubeAPIKey,
                ApplicationName = user.AppName
            })) {
                var request = service.Search.List("snippet");
                if (query != null)
                    request.Q = query;
                else
                    request.RelatedToVideoId = similarTo.VideoId;
                request.Type = "video";
                request.PageToken = pageToken;
                request.MaxResults = app.SearchPageSize;
                SearchListResponse response = request.Execute();
                return new SearchResult {
                    State = new SearchState {
                        TotalResults = -1,
                        NextPageToken = response.NextPageToken,
                    },
                    Results = response.Items.Select(i => new Result {
                        VideoId = i.Id.VideoId,
                        Title = i.Snippet.Title,
                        Description = i.Snippet.Description,
                        ThumbnailUrl = i.Snippet.Thumbnails.Default__.Url
                    })
                };
            }
        }
    }
}
