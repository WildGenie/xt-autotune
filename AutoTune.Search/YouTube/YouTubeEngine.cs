using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Linq;

namespace AutoTune.Search.YouTube {

    internal class YouTubeEngine : SearchEngine {

        internal override SearchResults Execute(SearchQuery query, SearchState state) {
            using (var service = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = query.Credentials[YouTubeTypeId].Key,
                ApplicationName = query.Credentials[YouTubeTypeId].AppName
            })) {
                var request = service.Search.List("snippet");
                request.Type = "video";
                request.Q = query.Query;
                request.MaxResults = query.PageSize;
                request.PageToken = state.NextPageToken;
                request.RelatedToVideoId = query.RelatedId;
                SearchListResponse response = request.Execute();
                var results = response.Items.Select(i => new SearchResult {
                    TypeId = YouTubeTypeId,
                    VideoId = i.Id.VideoId,
                    Title = i.Snippet.Title,
                    Description = i.Snippet.Description,
                    ThumbnailUrl = i.Snippet.Thumbnails.Default__.Url
                });
                SearchState newState = new SearchState(-1, response.NextPageToken);
                return new SearchResults(newState, results.ToList());
            }
        }
    }
}
