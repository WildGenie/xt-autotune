﻿using AutoTune.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTune.Drivers {

    abstract class Search {

        static readonly object Lock = new object();
        static readonly Dictionary<string, Search> Drivers = new Dictionary<string, Search>();

        protected abstract SearchResult Execute(int totalResults, string pageToken, string query);

        static void InitializeDrivers() {
            lock (Lock) {
                if (Drivers.Count > 0)
                    return;
                if (Settings.Instance.YouTube.Enabled)
                    Drivers.Add(YouTubeSearch.TypeId, new YouTubeSearch());
                if (Settings.Instance.Vimeo.Enabled)
                    Drivers.Add(VimeoSearch.TypeId, new VimeoSearch());
            }
        }

        public static void Continue(IDictionary<string, SearchState> state, string query, Action<Results> callback) {
            InitializeDrivers();
            DoSearch(state, query, callback);
        }

        public static IDictionary<string, SearchState> Start(string query, Action<Results> callback) {
            InitializeDrivers();
            IDictionary<string, SearchState> state = new ConcurrentDictionary<string, SearchState>();
            DoSearch(state, query, callback);
            return state;
        }

        static void DoSearch(IDictionary<string, SearchState> state, string query, Action<Results> callback) {
            InitializeDrivers();
            foreach (var entry in Drivers)
                ThreadPool.QueueUserWorkItem(_ => {
                    try {
                        SearchState thisState = state.ContainsKey(entry.Key) ? state[entry.Key] : null;
                        string pageToken = thisState?.NextPageToken;
                        int totalResults = thisState?.TotalResults ?? -1;
                        var searchResult = entry.Value.Execute(totalResults, pageToken, query);
                        callback(new Results { Items = searchResult.Results.ToList() });
                        state[entry.Key] = searchResult.State;
                    } catch (Exception e) {
                        callback(new Results { Error = e });
                    }
                });
        }
    }
}
