using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTune.Drivers {

    abstract class Search {

        protected abstract SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo);

        public static void Continue(IDictionary<string, SearchState> state, string query, Result similarTo, Action<Results> callback) {
            DoSearch(state, query, similarTo, callback);
        }

        public static IDictionary<string, SearchState> Start(string query, Result similarTo, Action<Results> callback) {
            IDictionary<string, SearchState> state = new ConcurrentDictionary<string, SearchState>();
            DoSearch(state, query, similarTo, callback);
            return state;
        }

        static void DoSearch(IDictionary<string, SearchState> state, string query, Result similarTo, Action<Results> callback) {
            if ((query == null) == (similarTo == null))
                throw new ArgumentException(nameof(similarTo));
            foreach (var entry in AppSettings.Searchers)
                if (similarTo == null || entry.Key.Equals(similarTo.Type))
                    ThreadPool.QueueUserWorkItem(_ => {
                        try {
                            SearchState thisState = state.ContainsKey(entry.Key) ? state[entry.Key] : null;
                            string pageToken = thisState?.NextPageToken;
                            int totalResults = thisState?.TotalResults ?? -1;
                            var searchResult = entry.Value.Execute(totalResults, pageToken, query, similarTo);
                            Results results = new Results();
                            results.Items = searchResult.Results.ToList();
                            foreach (var item in results.Items)
                                item.Type = entry.Key;
                            state[entry.Key] = searchResult.State;
                            callback(results);
                        } catch (Exception e) {
                            callback(new Results { Error = e });
                        }
                    });
        }
    }
}
