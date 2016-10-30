using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoTune.Local {

    static class SuggestionScanner {

        static int running = 0;
        static readonly object Lock = new object();

        public static void Start(int interval) {
            Interlocked.CompareExchange(ref running, 1, 0);
            new Thread(() => Run(interval)).Start();
        }

        public static void Terminate() {
            Interlocked.CompareExchange(ref running, 0, 1);
            lock (Lock)
                Monitor.Pulse(Lock);
        }

        static void Run(int interval) {
            lock (Lock)
                while (running != 0) {
                    try {
                        Scan();
                    } catch (Exception e) {
                        Logger.Error(e, "Searching suggestions failed.");
                    }
                    long now = Environment.TickCount;
                    long start = Environment.TickCount;
                    while (now - start < interval && running != 0) {
                        Monitor.Wait(Lock, interval);
                        now = Environment.TickCount;
                    }
                }
        }

        static void Scan() {
            Logger.Info("Searching suggestions...");
            int pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            var artists = Library.GetFavouritesWithoutPendingSuggestions(SearchEngine.LocalTypeId);
            int i = 1;
            foreach (var artist in artists) {
                try {
                    Logger.Debug("Searching suggestions {0}/{1}: {2}.", i++, artists.Count, artist.Name);
                    var query = new SearchQuery(credentials, artist.Name, false, false, pageSize);
                    var results = SearchEngine.Search(query);
                    Library.Suggest(artist, results);
                    Logger.Debug("Found {0} suggestions for {1}.", results.Count, artist.Name);
                } catch (Exception e) {
                    Logger.Error(e, "Searching suggestions failed for {0}.", artist.Name);
                }
            }
        }
    }
}
