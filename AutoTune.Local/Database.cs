using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace AutoTune.Local {

    public class Database : DbContext {

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

        public Database() : base(GetConnection(), true) {
            Configuration.AutoDetectChangesEnabled = false;
        }

        public static void Initialize(string dbFolder) {
            string[] statements;
            dbPath = Path.Combine(dbFolder, FileName);
            if (File.Exists(dbPath))
                return;
            SQLiteConnection.CreateFile(dbPath);
            using (var stream = typeof(Database).Assembly.GetManifestResourceStream("AutoTune.Local.db.sql"))
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
            using (var db = new Database()) {
                db.Configuration.LazyLoadingEnabled = false;
                return db.Tracks
                    .Where(t => t.Title != null && t.Title.ToLower().Contains(q) ||
                    t.Artist != null && t.Artist.Name.ToLower().Contains(q))
                    .Include(t => t.Genre)
                    .Include(t => t.Artist)
                    .ToList();
            }
        }
    }
}
