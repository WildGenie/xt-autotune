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

        static SQLiteConnection GetConnection() {
            SQLiteConnection result = new SQLiteConnection(string.Format(ConnectionString, dbPath));
            result.Open();
            return result;
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }

        internal Library() : base(GetConnection(), true) {
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
        }

        public static List<Track> Search(string query, int page, int pageSize) {
            string q = query.ToLower();
            using (var library = new Library()) {
                library.Configuration.LazyLoadingEnabled = false;
                return library.Tracks
                    .Where(t => t.Title != null && (t.Title.ToLower().Contains(q) || q.Contains(t.Title.ToLower())) ||
                    t.Album != null && (t.Album.Name.ToLower().Contains(q) || q.Contains(t.Album.Name.ToLower())) ||
                    t.Artist != null && (t.Artist.Name.ToLower().Contains(q)) || q.Contains(t.Artist.Name.ToLower()))
                    .OrderBy(t => t.Title)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Include(t => t.Genre)
                    .Include(t => t.Artist)
                    .ToList();
            }
        }
    }
}
