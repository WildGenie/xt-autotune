using AutoTune.Local;
using AutoTune.Shared;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoTune.Search.YouTube {

    class YouTubeEngine : SearchEngine {

        static List<SearchResult> TransformResponse(Google.Apis.YouTube.v3.Data.SearchListResponse response) {
            return response.Items.Select(i => new SearchResult {
                Local = false,
                TypeId = YouTubeTypeId,
                VideoId = i.Id.VideoId,
                Title = i.Snippet.Title,
                Description = i.Snippet.Description,
                ThumbnailBase64 = Convert.ToBase64String(Utility.Download(i.Snippet.Thumbnails.Default__.Url))
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
                ApplicationName = query.Credentials[YouTubeTypeId].ApplicationName
            })) {
                var request = CreateRequest(service, query, currentPage);
                var response = request.Execute();
                var results = TransformResponse(response);
                if (query.Favourite)
                    results = Library.FilterFavourites(results);
                return new SearchResults(response.NextPageToken, results);
            }
        }
    }
}
