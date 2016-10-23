using System;
using System.Collections.Generic;

namespace AutoTune.Search {

    public class SearchResponse {

        public Exception Error { get; }
        public List<SearchResult> Results { get; }

        internal SearchResponse(Exception error, List<SearchResult> results) {
            Error = error;
            Results = results;
        }
    }
}
