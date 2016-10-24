using System.Collections.Generic;

namespace AutoTune.Search {

    class SearchResults {

        internal string NextPage { get; }
        internal List<SearchResult> Results { get; }

        internal SearchResults(string nextPage, List<SearchResult> results) {           
            Results = results;
            NextPage = nextPage;
        }
    }
}
