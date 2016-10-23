using AutoTune.Shared;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using TagLib;

namespace AutoTune.Local {

    public class Scanner {

        static bool running = true;
        const double ProgressInterval = 0.05;
        static readonly object Lock = new object();

        public static void Start(string libraryFolder) {
            new Thread(() => Run(libraryFolder)).Start();
        }

        public static void Terminate() {
            lock (Lock) {
                running = false;
                Monitor.Pulse(Lock);
            }
        }

        static void Run(string libraryFolder) {
            try {
                using (Database db = new Database()) {
                    ScanNewTracks(db, libraryFolder);
                    CleanOldTracks(db);
                }
            } catch (Exception e) {
                Logger.Error(e, "Scanning library failed.");
            }
            lock (Lock) {
                while (!running)
                    Monitor.Wait(Lock);
            }
        }

        static void ScanNewTracks(Database db, string libraryFolder) {

            string path;
            string title;
            byte[] image;
            double progress;
            string genreName;
            string albumName;
            string artistName;
            int addedGenres = 0;
            int addedTracks = 0;
            int addedAlbums = 0;
            int addedArtists = 0;
            double previousProgress = 0.0;

            var paths = Directory.GetFiles(libraryFolder, "*.*", SearchOption.AllDirectories);
            Logger.Info("Scanning new tracks...");
            for (int i = 0; i < paths.Length; i++) {
                path = paths[i];
                if (!db.Tracks.Where(t => t.Path.Equals(path)).Any()) {
                    try {
                        using (var file = TagLib.File.Create(path)) {
                            title = file.Tag.Title;
                            albumName = file.Tag.Album;
                            genreName = file.Tag.FirstGenre;
                            artistName = file.Tag.FirstPerformer;
                            image = file.Tag.Pictures.Length == 0 ? null : file.Tag.Pictures[0].Data.Data;
                        }
                    } catch (UnsupportedFormatException) {
                        Logger.Debug("Unsupported file: {0}.", path);
                        continue;
                    }
                    Genre genre = FindOrCreate(db, db.Genres, genreName, ref addedGenres, n => new Genre(n), g => g.Name.Equals(genreName));
                    Album album = FindOrCreate(db, db.Albums, albumName, ref addedAlbums, n => new Album(n), a => a.Name.Equals(albumName));
                    Artist artist = FindOrCreate(db, db.Artists, artistName, ref addedArtists, n => new Artist(n), a => a.Name.Equals(artistName));
                    db.Tracks.Add(new Track(paths[i], title, image, genre, album, artist));
                    db.SaveChanges();
                    addedTracks++;
                    progress = i / (double)paths.Length;
                    if (progress > previousProgress + ProgressInterval) {
                        previousProgress = progress;
                        Logger.Debug("Scanning new tracks ({0}%).", (int)(progress * 100));
                    }
                }
            }
            string format = "Finished scanning new tracks. Inserted {0} genres, {1} albums, {2} artists and {3} tracks.";
            Logger.Info(format, addedGenres, addedAlbums, addedArtists, addedTracks);
        }

        static void CleanOldTracks(Database db) {

            int removedTracks = 0;
            Logger.Info("Cleaning old tracks...");
            foreach (Track track in db.Tracks)
                if (!System.IO.File.Exists(track.Path)) {
                    db.Tracks.Remove(track);
                    db.SaveChanges();
                    removedTracks++;
                }
            var oldGenres = db.Genres.Where(g => !db.Tracks.Any(t => t.Genre.Equals(g))).ToList();
            var oldAlbums = db.Albums.Where(a => !db.Tracks.Any(t => t.Album.Equals(a))).ToList();
            var oldArtists = db.Artists.Where(a => !db.Tracks.Any(t => t.Artist.Equals(a))).ToList();
            db.Albums.RemoveRange(oldAlbums);
            db.Genres.RemoveRange(oldGenres);
            db.Artists.RemoveRange(oldArtists);
            db.SaveChanges();
            string format = "Finished cleaning old tracks. Removed {0} genres, {1} albums, {2} artists and {2} tracks.";
            Logger.Info(format, oldGenres.Count, oldAlbums.Count, oldArtists.Count, removedTracks);
        }

        static T FindOrCreate<T>(Database db, DbSet<T> set, string name, ref int added,
            Func<string, T> create, Expression<Func<T, bool>> predicate) where T : class {

            if (name == null)
                return default(T);
            T result = set.SingleOrDefault(predicate);
            if (result != null)
                return result;
            result = create(name);
            set.Add(result);
            db.SaveChanges();
            added++;
            return result;
        }
    }
}
