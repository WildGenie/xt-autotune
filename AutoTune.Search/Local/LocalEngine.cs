using AutoTune.Local;
using System;
using System.Linq;

namespace AutoTune.Search.Local {

    class LocalEngine : SearchEngine {

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            int page = currentPage == null ? 0 : int.Parse(currentPage);
            var results = Library.Search(query.Query, page, query.PageSize).Select(t => new SearchResult {
                Local = true,
                VideoId = t.Path,
                TypeId = LocalTypeId,
                Description = t.Comment,
                ThumbnailBase64 = t.ImageBase64,
                Title = string.Format("{0} - {1}", t.Artist?.Name ?? "?", t.Title ?? "?")
            }).ToList();
            return new SearchResults((page + 1).ToString(), results);
        }
    }
}
