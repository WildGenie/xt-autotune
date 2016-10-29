using AutoTune.Local;
using AutoTune.Search.DailyMotion;
using AutoTune.Search.Local;
using AutoTune.Search.Vimeo;
using AutoTune.Search.YouTube;
using AutoTune.Shared;
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

        static readonly Dictionary<string, SearchEngine> ActiveEngines = new Dictionary<string, SearchEngine>();
        static readonly Dictionary<string, SearchEngine> Engines = new Dictionary<string, SearchEngine> {
            { LocalTypeId, new LocalEngine() },
            { VimeoTypeId, new VimeoEngine() },
            { YouTubeTypeId, new YouTubeEngine() },
            { DailyMotionTypeId, new DailyMotionEngine() }
        };

        public static void Initialize(IEnumerable<string> activeEngines) {
            foreach (string active in activeEngines) {
                if (Engines.ContainsKey(active))
                    ActiveEngines.Add(active, Engines[active]);
                else
                    Logger.Debug("Unknown search engine '{0}'.", active);
            }
            foreach (string engine in Engines.Keys)
                Logger.Debug("Search engine '{0}' enabled: {1}.", engine, ActiveEngines.ContainsKey(engine));
        }

        internal abstract SearchResults Execute(SearchQuery query, string currentPage);

        public static object Start(SearchQuery query, Action<SearchResponse> callback) {
            var paging = new ConcurrentDictionary<string, string>();
            DoSearch(query, paging, callback);
            return paging;
        }

        public static void Continue(SearchQuery query, object paging, Action<SearchResponse> callback) {
            DoSearch(query, paging, callback);
        }

        static void DoSearch(SearchQuery query, object p, Action<SearchResponse> callback) {
            var paging = (IDictionary<string, string>)p;
            if (query.RelatedId != null)
                DoSearch(query.Credentials.Keys.Single(), query, paging, callback);
            else
                foreach (string typeId in ActiveEngines.Keys)
                    if (!query.Local || LocalTypeId.Equals(typeId))
                        DoSearch(typeId, query, paging, callback);
        }

        static void DoSearch(string typeId, SearchQuery query, IDictionary<string, string> paging, Action<SearchResponse> callback) {
            ThreadPool.QueueUserWorkItem(_ => {
                try {
                    SearchEngine engine = null;
                    if (!ActiveEngines.TryGetValue(typeId, out engine)) {
                        Logger.Debug("Search engine '{0}' not found or not enabled.", typeId);
                        return;
                    }
                    string nextPage = null;
                    paging.TryGetValue(typeId, out nextPage);
                    var results = engine.Execute(query, nextPage);
                    paging[typeId] = results.NextPage;
                    callback(new SearchResponse(null, results.Results));
                } catch (Exception e) {
                    callback(new SearchResponse(e, null));
                }
            });
        }
    }
}
