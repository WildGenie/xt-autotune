using AutoTune.Search;
using AutoTune.Search.Similar;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoTune.Local {

    static class SuggestionScanner {

        static bool forceUpdate = false;
        static readonly object Lock = new object();
        public static event EventHandler<EventArgs<List<SearchResult>>> Suggested;

        public static void Start(int delay, int interval) {
            var thread = new Thread(() => Run(delay, interval));
            thread.IsBackground = true;
            thread.Start();
        }

        public static SearchResult SuggestionToSearchResult(Suggestion s) {
            return new SearchResult {
                Local = false,
                Title = s.Title,
                TypeId = s.TypeId,
                VideoId = s.VideoId,
                Description = s.Comment,
                ThumbnailBase64 = s.ImageBase64
            };
        }

        public static void UpdateSuggestions() {
            lock (Lock) {
                forceUpdate = true;
                Monitor.Pulse(Lock);
            }
        }

        public static void SearchSimilar(string title, Action<SearchResponse> callback) {
            var app = AppSettings.Instance;
            ThreadPool.QueueUserWorkItem(_ => {
                string artist = title.IndexOf('-') == -1 ? title : title.Split('-')[0];
                artist = artist.Trim().ToLower();
                if (artist.Length == 0)
                    return;
                List<string> similarArtists;
                try {
                    similarArtists = FindSimilarArtists(artist, app.SimilarMinSimilarity, app.SimilarArtistLimit);
                } catch (Exception e) {
                    Logger.Error(e, "Failed to find similar artists for {0}.", artist);
                    callback(new SearchResponse(e, null));
                    return;
                }
                foreach (string similarArtist in similarArtists) {
                    ThreadPool.QueueUserWorkItem(__ => {
                        try {
                            callback(new SearchResponse(null, FindTracks(similarArtist, app.SearchPageSize)));
                        } catch (Exception e) {
                            Logger.Error(e, "Failed to find tracks for {0}.", similarArtist);
                            callback(new SearchResponse(e, null));
                        }
                    });
                }
            });
        }

        static void Run(int delay, int interval) {
            Thread.Sleep(delay);
            while (true) {
                try {
                    var similarArtists = FindSimilarArtistsForFavourites();
                    if (similarArtists.Any())
                        SearchSimilarTracks(similarArtists);
                } catch (Exception e) {
                    Logger.Error(e, "Searching similar tracks failed.");
                }
                forceUpdate = false;
                long now = Environment.TickCount;
                long start = Environment.TickCount;
                lock (Lock)
                    while (!forceUpdate && now - start < interval) {
                        Monitor.Wait(Lock, interval);
                        now = Environment.TickCount;
                    }
            }
        }

        static string Normalize(string q) {
            string result = string.Join("-", Utility.Normalize(q)
                .Replace('-', ' ')
                .Split(' ')
                .Where(t => !string.IsNullOrWhiteSpace(t)));
            return result;
        }

        static List<string> FindSimilarArtists(string artist, double minSimilarity, int limit) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            Logger.Debug("Searching similar artists for {0}.", artist);
            return SimilarEngine
                .Search(user.MusicGraphAPIKey, artist, app.SimilarArtistCount, limit)
                .Where(r => r.Similarity >= minSimilarity)
                .Select(s => s.Name)
                .ToList();
        }

        static List<SearchResult> FindTracks(string artist, int limit) {
            Logger.Debug("Searching tracks for similar artist {0}.", artist);
            var credentials = UserSettings.Instance.Credentials;
            var query = new SearchQuery(credentials, Normalize(artist), false, false, limit);
            return SearchEngine.Search(query);
        }

        static Dictionary<Artist, List<string>> FindSimilarArtistsForFavourites() {
            var app = AppSettings.Instance;
            Logger.Info("Searching similar artists...");
            Dictionary<Artist, List<string>> result = new Dictionary<Artist, List<string>>();
            var artists = Library.GetFavouriteArtistsWithoutPendingSuggestions(SearchEngine.LocalTypeId, app.SuggestionSearchArtistLimit);
            int i = 1;
            if (artists.Count == 0)
                Logger.Info("No similar artists without pending suggestions found.");
            foreach (var artist in artists) {
                try {
                    Logger.Debug("Searching similar artists {0}/{1}.", i++, artists.Count);
                    var similar = FindSimilarArtists(artist.Name, app.SuggestionMinSimilarity, app.SuggestionSimilarArtistLimit);
                    if (similar.Count > 0)
                        result.Add(artist, similar);
                    Logger.Debug("Found {0} similar artists for {1}.", similar.Count, artist.Name);
                } catch (Exception e) {
                    Logger.Error(e, "Searching similar artists failed for {0}.", artist.Name);
                }
            }
            return result;
        }

        static void SearchSimilarTracks(Dictionary<Artist, List<string>> similarArtists) {
            int i = 1;
            var app = AppSettings.Instance;
            HashSet<string> seen = new HashSet<string>();
            Logger.Info("Searching tracks for similar artists...");
            int total = similarArtists.Values.SelectMany(s => s).Count();
            foreach (var entry in similarArtists)
                foreach (var similarArtist in entry.Value)
                    if (seen.Add(similarArtist))
                        ThreadPool.QueueUserWorkItem(index => {
                            try {
                                Logger.Debug("Searching tracks ({0}/{1}).", (int)index, total);
                                var results = FindTracks(similarArtist, app.SuggestionSimilarTrackLimit);
                                string searchArtist = Normalize(similarArtist);
                                var filteredResults = new List<SearchResult>();
                                foreach (var result in results) {
                                    string searchTitle = Normalize(result.Title)
                                    .Replace(searchArtist, "")
                                    .Replace("--", "-");
                                    if (searchTitle.StartsWith("-"))
                                        searchTitle = searchTitle.Substring(1);
                                    if (searchTitle.EndsWith("-"))
                                        searchTitle = searchTitle.Substring(0, searchTitle.Length - 1);
                                    bool handled = Library.IsSuggestionHandled(result.TypeId, result.VideoId);
                                    bool hasResults = Library.HasSearchResults(searchArtist, searchTitle);
                                    if (!handled && !hasResults)
                                        filteredResults.Add(result);
                                }
                                Logger.Debug("Found {0} similar tracks for {1}, {2} new.", results.Count, similarArtist, filteredResults.Count);
                                Library.Suggest(entry.Key, filteredResults);
                                Suggested(typeof(SuggestionScanner), new EventArgs<List<SearchResult>>(filteredResults));
                            } catch (Exception e) {
                                Logger.Error(e, "Searching tracks failed for {0}.", similarArtist);
                            }
                        }, i++);
        }
    }
}
