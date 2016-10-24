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
                foreach (string typeId in Engines.Keys)
                    DoSearch(typeId, query, paging, callback);
        }

        static void DoSearch(string typeId, SearchQuery query, IDictionary<string, string> paging, Action<SearchResponse> callback) {
            ThreadPool.QueueUserWorkItem(_ => {
                try {
                    string nextPage = null;
                    paging.TryGetValue(typeId, out nextPage);
                    var results = Engines[typeId].Execute(query, nextPage);
                    paging[typeId] = results.NextPage;
                    callback(new SearchResponse(null, results.Results));
                } catch (Exception e) {
                    callback(new SearchResponse(e, null));
                }
            });
        }
    }
}
