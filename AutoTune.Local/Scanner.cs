using AutoTune.Shared;
using System;
using System.IO;
using System.Linq;
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
            double progress;
            string genreName;
            string artistName;
            int addedGenres = 0;
            int addedTracks = 0;
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
                            genreName = file.Tag.FirstGenre;
                            artistName = file.Tag.FirstPerformer;
                        }
                    } catch (UnsupportedFormatException) {
                        Logger.Debug("Unsupported file: {0}.", path);
                        continue;
                    }
                    Genre genre = FindOrCreateGenre(db, genreName, ref addedGenres);
                    Artist artist = FindOrCreateArtist(db, artistName, ref addedArtists);
                    db.Tracks.Add(new Track(paths[i], title, genre, artist));
                    addedTracks++;
                    progress = i / (double)paths.Length;
                    if (progress > previousProgress + ProgressInterval) {
                        previousProgress = progress;
                        Logger.Debug("Scanning new tracks ({0}%).", (int)(progress * 100));
                    }
                }
            }
            string format = "Finished scanning new tracks. Inserted {0} genres, {1} artists and {2} tracks.";
            Logger.Info(format, addedGenres, addedArtists, addedTracks);
        }

        static void CleanOldTracks(Database db) {

            int removedTracks = 0;
            Logger.Info("Cleaning old tracks...");
            foreach(Track track in db.Tracks)
                if(!System.IO.File.Exists(track.Path)) {
                    db.Tracks.Remove(track);
                    db.SaveChanges();
                    removedTracks++;
                }
            var oldGenres = db.Genres.Where(g => !db.Tracks.Any(t => t.Genre.Equals(g))).ToList();
            var oldArtists = db.Artists.Where(a => !db.Tracks.Any(t => t.Artist.Equals(a))).ToList();
            db.Genres.RemoveRange(oldGenres);
            db.Artists.RemoveRange(oldArtists);
            db.SaveChanges();
            string format = "Finished cleaning old tracks. Removed {0} genres, {1} artists and {2} tracks.";
            Logger.Info(format, oldGenres.Count, oldArtists.Count, removedTracks);
        }

        static Genre FindOrCreateGenre(Database db, string name, ref int addedGenres) {
            if (name == null)
                return null;
            Genre result = db.Genres.SingleOrDefault(g => g.Name.Equals(name));
            if (result != null)
                return result;
            result = new Genre(name);
            db.Genres.Add(result);
            db.SaveChanges();
            addedGenres++;
            return result;
        }

        static Artist FindOrCreateArtist(Database db, string name, ref int addedArtists) {
            if (name == null)
                return null;
            Artist result = db.Artists.SingleOrDefault(g => g.Name.Equals(name));
            if (result != null)
                return result;
            result = new Artist(name);
            db.Artists.Add(result);
            db.SaveChanges();
            addedArtists++;
            return result;
        }
    }
}
