using System.Data.Entity;
using System.Data.SQLite;
using System.IO;

namespace AutoTune.Local {

    public class Database : DbContext {

        static string dbPath;
        const string FileName = "Db.sqlite";
        const string ConnectionString = "Data Source=\"{0}\";Version=3;";

        static SQLiteConnection GetConnection() {
            SQLiteConnection result = new SQLiteConnection(string.Format(ConnectionString, dbPath));
            result.Open();
            return result;
        }

        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
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
            using (var stream = typeof(Database).Assembly.GetManifestResourceStream("AutoTune.Local.Db.sql"))
            using (var reader = new StreamReader(stream))
                statements = reader.ReadToEnd().Split(';');
            using (var conn = GetConnection())
            using (var command = conn.CreateCommand())
                for (int i = 0; i < statements.Length - 1; i++) {
                    command.CommandText = statements[i];
                    command.ExecuteNonQuery();
                }
        }
    }
}
