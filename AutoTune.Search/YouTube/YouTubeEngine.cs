using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;

namespace AutoTune.Search.YouTube {

    class YouTubeEngine : SearchEngine {

        static List<SearchResult> TransformResponse(SearchListResponse response) {
            return response.Items.Select(i => new SearchResult {
                TypeId = YouTubeTypeId,
                VideoId = i.Id.VideoId,
                Title = i.Snippet.Title,
                Description = i.Snippet.Description,
                ThumbnailUrl = i.Snippet.Thumbnails.Default__.Url
            }).ToList();
        }

        static SearchResource.ListRequest CreateRequest(YouTubeService service, SearchQuery query, string currentPage) {
            var result = service.Search.List("snippet");
            result.Type = "video";
            result.Q = query.Query;
            result.PageToken = currentPage;
            result.MaxResults = query.PageSize;
            result.RelatedToVideoId = query.RelatedId;
            return result;
        }

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            using (var service = new YouTubeService(new BaseClientService.Initializer() {
                ApiKey = query.Credentials[YouTubeTypeId].Key,
                ApplicationName = query.Credentials[YouTubeTypeId].AppName
            })) {
                var request = CreateRequest(service, query, currentPage);
                var response = request.Execute();
                return new SearchResults(response.NextPageToken, TransformResponse(response));
            }
        }
    }
}
