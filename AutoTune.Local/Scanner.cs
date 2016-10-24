using AutoTune.Shared;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using TagLib;

namespace AutoTune.Local {

    public class Scanner {

        static int running = 0;
        const double ProgressInterval = 0.05;
        static readonly object Lock = new object();

        public static void Start(string libraryFolder, int interval) {
            Interlocked.CompareExchange(ref running, 1, 0);
            new Thread(() => Run(libraryFolder, interval)).Start();
        }

        public static void Terminate() {
            Interlocked.CompareExchange(ref running, 0, 1);
            lock (Lock)
                Monitor.Pulse(Lock);
        }

        static void Run(string libraryFolder, int interval) {
            lock (Lock)
                while (running != 0) {
                    try {
                        using (Library library = new Library()) {
                            ScanNewTracks(library, libraryFolder);
                            CleanOldTracks(library);
                        }
                    } catch (Exception e) {
                        Logger.Error(e, "Scanning library failed.");
                    }
                    long start = Environment.TickCount;
                    long now = Environment.TickCount;
                    while (now - start < interval && running != 0) {
                        Monitor.Wait(Lock, interval);
                        now = Environment.TickCount;
                    }
                }
        }

        static TrackInfo ParseTrack(string path) {
            try {
                using (var file = TagLib.File.Create(path))
                    return new TrackInfo {
                        path = path,
                        title = file.Tag.Title,
                        album = file.Tag.Album,
                        genre = file.Tag.FirstGenre,
                        artist = file.Tag.FirstPerformer,
                        image = file.Tag.Pictures.Length == 0 ? null : file.Tag.Pictures[0].Data.Data
                    };
            } catch (UnsupportedFormatException) {
                Logger.Debug("Unsupported file: {0}.", path);
                return null;
            }
        }

        static T FindOrCreate<T>(Library library, DbSet<T> set, string name, ref int counter)
           where T : class, INamed, new() {

            if (name == null)
                return default(T);
            T result = set.SingleOrDefault(t => t.Name.Equals(name));
            if (result != null)
                return result;
            result = new T();
            result.Name = name;
            set.Add(result);
            library.SaveChanges();
            counter++;
            return result;
        }

        static void ImportTrack(Library library, TrackInfo info, Counters counters) {
            Genre genre = FindOrCreate(library, library.Genres, info.genre, ref counters.genres);
            Album album = FindOrCreate(library, library.Albums, info.album, ref counters.albums);
            Artist artist = FindOrCreate(library, library.Artists, info.artist, ref counters.artists);
            library.Tracks.Add(new Track(info.path, info.title, info.image, genre, album, artist));
            library.SaveChanges();
            counters.tracks++;
        }

        static void ScanNewTracks(Library library, string libraryFolder) {

            string path;
            double progress;
            double previousProgress = 0.0;

            var counters = new Counters();
            Logger.Info("Scanning new tracks...");
            var paths = Directory.GetFiles(libraryFolder, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < paths.Length; i++) {
                if (Interlocked.CompareExchange(ref running, 0, 0) == 0)
                    return;
                path = paths[i];
                if (library.Tracks.Where(t => t.Path.Equals(path)).Any())
                    continue;
                var track = ParseTrack(path);
                if (track != null)
                    ImportTrack(library, track, counters);
                progress = i / (double)paths.Length;
                if (progress > previousProgress + ProgressInterval) {
                    previousProgress = progress;
                    Logger.Debug("Scanning new tracks ({0}%).", (int)(progress * 100));
                }
            }
            string format = "Finished scanning new tracks. Inserted {0} genres, {1} albums, {2} artists and {3} tracks.";
            Logger.Info(format, counters.genres, counters.albums, counters.artists, counters.tracks);
        }

        static void CleanOldTracks(Library library) {

            int removedTracks = 0;
            Logger.Info("Cleaning old tracks...");
            foreach (Track track in library.Tracks) {
                if (Interlocked.CompareExchange(ref running, 0, 0) == 0)
                    return;
                if (!System.IO.File.Exists(track.Path)) {
                    library.Tracks.Remove(track);
                    library.SaveChanges();
                    removedTracks++;
                }
            }
            var oldGenres = library.Genres.Where(g => !library.Tracks.Any(t => t.Genre.Equals(g))).ToList();
            var oldAlbums = library.Albums.Where(a => !library.Tracks.Any(t => t.Album.Equals(a))).ToList();
            var oldArtists = library.Artists.Where(a => !library.Tracks.Any(t => t.Artist.Equals(a))).ToList();
            library.Albums.RemoveRange(oldAlbums);
            library.Genres.RemoveRange(oldGenres);
            library.Artists.RemoveRange(oldArtists);
            library.SaveChanges();
            string format = "Finished cleaning old tracks. Removed {0} genres, {1} albums, {2} artists and {2} tracks.";
            Logger.Info(format, oldGenres.Count, oldAlbums.Count, oldArtists.Count, removedTracks);
        }
    }
}
