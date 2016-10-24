﻿using AutoTune.Local;
using System.Linq;

namespace AutoTune.Search.Local {

    class LocalEngine : SearchEngine {

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            var results = Database.Search(query.Query, 0, 0).Select(t => new SearchResult {
                VideoId = t.Path,
                TypeId = LocalTypeId,
                Description = t.Album?.Name,
                Title = string.Format("{0} - {1}", t.Artist?.Name ?? "?", t.Title ?? "?"),
            }).ToList();
            return new SearchResults(null, results);
        }
    }
}
