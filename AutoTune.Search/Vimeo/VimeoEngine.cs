using AutoTune.Local;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using VimeoDotNet.Models;

namespace AutoTune.Search.Vimeo {

    class VimeoEngine : SearchEngine {

        const string NoMoreResults = "NoMoreResults";

        static string GetThumbnailUrl(Video video) {
            return video?.pictures?.SelectMany(p => p.sizes).Select(s => s.link).FirstOrDefault();
        }

        static List<SearchResult> TransformResponse(Paginated<Video> response) {
            return response.data.Select(v => new SearchResult {
                Local = false,
                Title = v.name,
                TypeId = VimeoTypeId,
                VideoId = v.id?.ToString(),
                Description = v.description,
                ThumbnailBase64 = GetThumbnailUrl(v) == null ? null : Convert.ToBase64String(Utility.Download(GetThumbnailUrl(v)))
            }).ToList();
        }

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            if (query.RelatedId == null && string.IsNullOrWhiteSpace(query.Query))
                return new SearchResults(NoMoreResults, new List<SearchResult>());
            if (NoMoreResults.Equals(currentPage))
                return new SearchResults(NoMoreResults, new List<SearchResult>());
            var credentials = query.Credentials[VimeoTypeId];
            var client = new VimeoVideoClient(credentials.Key, credentials.Secret);
            var response = client.GetVideos(query, currentPage);
            var results = TransformResponse(response);
            if (query.Favourite)
                results = Library.FilterFavourites(results);
            return new SearchResults(response.paging.next ?? NoMoreResults, results);
        }
    }
}
