using AutoTune.Search;
using AutoTune.Search.Suggest;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        SearchSuggestions(SuggestArtists());
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

        static Dictionary<Artist, List<string>> SuggestArtists() {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            Logger.Info("Searching suggested artists...");
            Dictionary<Artist, List<string>> result = new Dictionary<Artist, List<string>>();
            var artists = Library.GetFavouriteArtistsWithoutPendingSuggestions(SearchEngine.LocalTypeId);
            int i = 1;
            foreach (var artist in artists) {
                try {
                    result.Add(artist, new List<string>());
                    Logger.Debug("Searching suggested artists {0}/{1}: {2}.", i++, artists.Count, artist.Name);
                    var suggestions = SuggestionEngine
                        .Search(user.MusicGraphAPIKey, artist.Name, app.SuggestionArtistCount)
                        .Where(r => r.Similarity >= app.SuggestionMinSimilarity)
                        .ToList();
                    foreach (var suggestion in suggestions)
                        result[artist].Add(suggestion.Name);
                    Logger.Debug("Found {0} suggested artists for {1}.", suggestions.Count, artist.Name);
                } catch (Exception e) {
                    Logger.Error(e, "Searching suggested artists failed for {0}.", artist.Name);
                }
            }
            return result;
        }

        static void SearchSuggestions(Dictionary<Artist, List<string>> suggestions) {
            int i = 1;
            HashSet<string> seen = new HashSet<string>();
            int pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            Logger.Info("Searching tracks for suggested artists...");
            int total = suggestions.Values.SelectMany(s => s).Count();
            foreach (var entry in suggestions)
                foreach (var suggestion in entry.Value) {
                    if (seen.Add(suggestion)) {
                        try {
                            Logger.Debug("Searching tracks for {0} ({1}/{2}).", suggestion, i, total);
                            var query = new SearchQuery(credentials, suggestion, false, false, pageSize);
                            var results = SearchEngine.Search(query);
                            var filteredResults = results.Where(r => !Library.HasSearchResults(suggestion, r.Title)).ToList();
                            Logger.Debug("Found {0} suggested tracks for {1}, {2} new.", results.Count, suggestion, filteredResults.Count);
                            Library.Suggest(entry.Key, filteredResults);
                        } catch (Exception e) {
                            Logger.Error(e, "Searching tracks failed for {0}.", suggestion);
                        }
                    }
                    i++;
                }
        }
    }
}
