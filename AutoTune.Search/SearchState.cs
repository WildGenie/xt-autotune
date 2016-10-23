namespace AutoTune.Search {

    internal class SearchState {

        internal int TotalResults { get; }
        internal string NextPageToken { get; }

        internal SearchState() {
        }

        internal SearchState(int totalResults, string nextPageToken) {
            TotalResults = totalResults;
            NextPageToken = nextPageToken;
        }
    }
}
