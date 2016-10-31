using System;
using System.Collections.Generic;

namespace AutoTune.Shared {

    public class SearchResponse {

        public Exception Error { get; }
        public List<SearchResult> Results { get; }

        public SearchResponse(Exception error, List<SearchResult> results) {
            Error = error;
            Results = results;
        }
    }
}
