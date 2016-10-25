using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using VimeoDotNet.Models;

namespace AutoTune.Search.Vimeo {

    class VimeoEngine : SearchEngine {

        const string NoMoreResults = "NoMoreResults";

        static List<SearchResult> TransformResponse(Paginated<Video> response) {
            return response.data.Select(v => new SearchResult {
                Title = v.name,
                TypeId = VimeoTypeId,
                VideoId = v.id?.ToString(),
                Description = v.description,
                ThumbnailBase64 = Convert.ToBase64String(Utility.Download(v.pictures.SelectMany(p => p.sizes).Select(s => s.link).FirstOrDefault()))
            }).ToList();
        }

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            if (NoMoreResults.Equals(currentPage))
                return new SearchResults(NoMoreResults, new List<SearchResult>());
            var credentials = query.Credentials[VimeoTypeId];
            var client = new VimeoVideoClient(credentials.Key, credentials.Secret);
            var response = client.GetVideos(query, currentPage);
            return new SearchResults(response.paging.next ?? NoMoreResults, TransformResponse(response));
        }
    }
}
