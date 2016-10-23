using System.Collections.Generic;

namespace AutoTune.Search {

    internal class SearchResults {

        internal SearchState State { get; }
        internal List<SearchResult> Results { get; }

        internal SearchResults(SearchState state, List<SearchResult> results) {
            State = state;
            Results = results;
        }
    }
}
