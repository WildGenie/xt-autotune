using AutoTune.Search.DailyMotion;
using AutoTune.Search.Local;
using AutoTune.Search.Vimeo;
using AutoTune.Search.YouTube;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTune.Search {

    public abstract class SearchEngine {

        public static string LocalTypeId = "Local";
        public static string VimeoTypeId = "Vimeo";
        public static string YouTubeTypeId = "YouTube";
        public static string DailyMotionTypeId = "DailyMotion";

        static readonly Dictionary<string, SearchEngine> Engines = new Dictionary<string, SearchEngine> {
            { LocalTypeId, new LocalEngine() },
            { VimeoTypeId, new VimeoEngine() },
            { YouTubeTypeId, new YouTubeEngine() },
            { DailyMotionTypeId, new DailyMotionEngine() }
        };

        internal abstract SearchResults Execute(SearchQuery query, SearchState state);

        public static void Continue(SearchQuery query, object state, Action<SearchResponse> callback) {
            DoSearch(query, state, callback);
        }

        public static object Start(SearchQuery query, Action<SearchResponse> callback) {
            IDictionary<string, SearchState> state = new ConcurrentDictionary<string, SearchState>();
            DoSearch(query, state, callback);
            return state;
        }

        static void DoSearch(SearchQuery query, object s, Action<SearchResponse> callback) {
            var states = (IDictionary<string, SearchState>)s;
            if (query.RelatedId != null)
                DoSearch(query.Credentials.Keys.Single(), query, states, callback);
            else {
                foreach (string typeId in Engines.Keys)
                    DoSearch(typeId, query, states, callback);
            }
        }

        static void DoSearch(string typeId, SearchQuery query, IDictionary<string, SearchState> states, Action<SearchResponse> callback) {
            ThreadPool.QueueUserWorkItem(_ => {
                try {
                    SearchState state;
                    if (!states.TryGetValue(typeId, out state))
                        state = new SearchState();
                    SearchEngine engine = Engines[typeId];
                    var results = engine.Execute(query, state);
                    states[typeId] = results.State;
                    callback(new SearchResponse(null, results.Results));
                } catch (Exception e) {
                    callback(new SearchResponse(e, null));
                }
            });
        }
    }
}
