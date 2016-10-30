using AutoTune.Shared;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AutoTune.Local {

    public class Library : DbContext {

        static string dbPath;
        const string FileName = "db.sqlite";
        const string ConnectionString = "Data Source=\"{0}\";Version=3;";
        static readonly string[] StopList = { "the", "and", "feat", "featuring", "vs" };

        static SQLiteConnection GetConnection() {
            SQLiteConnection result = new SQLiteConnection(string.Format(ConnectionString, dbPath));
            result.Open();
            return result;
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        internal Library() : base(GetConnection(), true) {
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
        }

        public static void Initialize(string dbFolder) {
            string[] statements;
            dbPath = Path.Combine(dbFolder, FileName);
            if (File.Exists(dbPath))
                return;
            SQLiteConnection.CreateFile(dbPath);
            using (var stream = typeof(Library).Assembly.GetManifestResourceStream("AutoTune.Local.db.sql"))
            using (var reader = new StreamReader(stream))
                statements = reader.ReadToEnd().Split(';');
            using (var conn = GetConnection())
            using (var command = conn.CreateCommand())
                for (int i = 0; i < statements.Length - 1; i++) {
                    command.CommandText = statements[i];
                    command.ExecuteNonQuery();
                }
            using (var library = new Library()) {
                Logger.Debug("Library contents: {0} tracks.", library.Tracks.Count());
                Logger.Debug("Library contents: {0} genres.", library.Genres.Count());
                Logger.Debug("Library contents: {0} albums.", library.Albums.Count());
                Logger.Debug("Library contents: {0} artists.", library.Artists.Count());
                Logger.Debug("Library contents: {0} favourites.", library.Favourites.Count());
                Logger.Debug("Library contents: {0} suggestions.", library.Suggestions.Count());
            }
        }

        public static List<Suggestion> GetOpenSuggestions() {
            using (var library = new Library()) {
                return library.Suggestions.Where(s => !s.Declined && !s.Accepted).ToList();
            }
        }

        public static bool HasSearchResults(string artist, string title) {
            return Find(artist + " " + title, false, 0, 1).Count > 0;
        }

        public static void HandleSuggestion(Suggestion suggestion, bool accept) {
            using (var library = new Library()) {
                foreach (var existing in library.Suggestions
                    .Where(s => s.TypeId.Equals(suggestion.TypeId))
                    .Where(s => s.VideoId.Equals(suggestion.VideoId))) {
                    existing.Accepted = accept;
                    existing.Declined = !accept;
                }
                library.SaveChanges();
            }
        }

        public static List<Artist> GetFavouriteArtistsWithoutPendingSuggestions(string localTypeId) {
            using (var library = new Library()) {
                return library.Tracks
                    .Join(library.Favourites, t => t.Path, f => f.VideoId,
                        (t, f) => new { Track = t, Favourite = f })
                    .Where(tf => !library.Suggestions.Where(s => s.Artist.Id == tf.Track.Artist.Id && !s.Declined && !s.Accepted).Any())
                    .Where(tsf => tsf.Favourite.TypeId.Equals(localTypeId))
                    .Select(tsf => tsf.Track.Artist)
                    .Distinct()
                    .ToList();
            }
        }

        public static void Suggest(Artist artist, IEnumerable<SearchResult> results) {
            using (var library = new Library()) {
                var a = library.Artists.Find(artist.Id);
                foreach (var result in results)
                    if (!library.Suggestions.Any(s => s.TypeId.Equals(result.TypeId) && s.VideoId.Equals(result.VideoId)))
                        library.Suggestions.Add(new Suggestion() {
                            Artist = a,
                            Title = result.Title,
                            TypeId = result.TypeId,
                            VideoId = result.VideoId,
                            Comment = result.Description,
                            ImageBase64 = result.ThumbnailBase64,
                        });
                library.SaveChanges();
            }
        }

        public static bool IsFavourite(string typeId, string videoId) {
            using (var library = new Library()) {
                return library.Favourites.Any(f => f.TypeId.Equals(typeId) && f.VideoId.Equals(videoId));
            }
        }

        public static List<SearchResult> FilterFavourites(IEnumerable<SearchResult> items) {
            using (var library = new Library()) {
                var favourites = new HashSet<Favourite>(library.Favourites.ToList().Select(f => new Favourite(f.TypeId, f.VideoId)));
                var favouriteItems = new Dictionary<Favourite, SearchResult>();
                foreach (var item in items)
                    favouriteItems.Add(new Favourite(item.TypeId, item.VideoId), item);
                foreach (var favouriteItem in favouriteItems.Keys.ToArray())
                    if (!favourites.Contains(favouriteItem))
                        favouriteItems.Remove(favouriteItem);
                return favouriteItems.Values.ToList();
            }
        }

        public static void SetFavourite(string typeId, string videoId, bool isFavourite) {
            using (var library = new Library()) {
                var favourite = library.Favourites.FirstOrDefault(f => f.TypeId.Equals(typeId) && f.VideoId.Equals(videoId));
                if ((favourite == null) == (!isFavourite))
                    return;
                if (!isFavourite)
                    library.Favourites.Remove(favourite);
                else
                    library.Favourites.Add(new Favourite(typeId, videoId));
                library.SaveChanges();
            }
        }

        public static List<Track> FindRelated(string path, int page, int pageSize) {
            using (var library = new Library()) {
                var track = library.Tracks
                    .Include(t => t.Album)
                    .Include(t => t.Artist)
                    .SingleOrDefault(t => t.Path.Equals(path));
                if (track == null)
                    return new List<Track>();
                var albumId = track?.Album?.Id;
                var artistId = track?.Artist?.Id;
                return ExecuteQuery(library.Tracks
                    .Where(t => t.Album != null && t.Album.Id == albumId ||
                      t.Artist != null && t.Artist.Id == artistId), page, pageSize);
            }
        }

        public static List<Track> Find(string query, bool favourite, int page, int pageSize) {
            var qLower = query.ToLower();
            var terms = qLower.Split(' ').Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct();
            terms = terms.Where(t => t.All(c => 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z'));
            terms = terms.Where(t => !StopList.Contains(t)).ToArray();
            Logger.Debug("Q = " + string.Join(", ", terms));
            using (var library = new Library()) {
                IQueryable<Track> q = library.Tracks;
                if (favourite)
                    q = q.Join(library.Favourites,
                        t => new { TypeId = "Local", VideoId = t.Path },
                        f => new { TypeId = f.TypeId, VideoId = f.VideoId },
                        (t, f) => t);
                return ExecuteQuery(q.Where(t => t.Path.ToLower().Contains(qLower) || terms.All(tm =>
                     ((t.Title == null ? "" : t.Title) +
                     (t.Album == null ? "" : t.Album.Name) +
                     (t.Artist == null ? "" : t.Artist.Name)).ToLower().Contains(tm))), page, pageSize);
            }
        }

        static List<Track> ExecuteQuery(IQueryable<Track> query, int page, int pageSize) {
            return query.OrderBy(t => t.Title)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Include(t => t.Genre)
                    .Include(t => t.Artist)
                    .ToList();
        }
    }
}
