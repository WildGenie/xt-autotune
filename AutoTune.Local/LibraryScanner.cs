using AutoTune.Shared;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using TagLib;

namespace AutoTune.Local {

    public static class LibraryScanner {

        static bool forceUpdate = false;
        const double ProgressInterval = 0.05;
        static readonly object Lock = new object();

        public static void UpdateLibrary() {
            lock (Lock) {
                forceUpdate = true;
                Monitor.Pulse(Lock);
            }
        }

        public static void Start(string libraryFolder, char tagSeparator, int interval) {
            var thread = new Thread(() => Run(libraryFolder, tagSeparator, interval));
            thread.IsBackground = true;
            thread.Start();
        }

        static void Run(string libraryFolder, char tagSeparator, int interval) {
            while (true) {
                try {
                    using (Library library = new Library()) {
                        ScanNewTracks(library, libraryFolder, tagSeparator);
                        CleanOldTracks(library, libraryFolder);
                    }
                } catch (Exception e) {
                    Logger.Error(e, "Scanning library failed.");
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

        static TrackInfo ParseTrack(string path, char tagSeparator) {
            try {
                using (var file = TagLib.File.Create(path)) {
                    string title = file.Tag.Title;
                    string artist = file.Tag.FirstPerformer;
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    if (title == null && fileName.Count(c => c == tagSeparator) == 1) {
                        string[] parts = fileName.Split(tagSeparator);
                        artist = parts[0].Trim();
                        title = parts[1].Trim();
                    }
                    return new TrackInfo {
                        path = path,
                        title = title,
                        artist = artist,
                        album = file.Tag.Album,
                        comment = file.Tag.Comment,
                        genre = file.Tag.FirstGenre,
                        imageBase64 = file.Tag.Pictures.Length == 0 ? null : Convert.ToBase64String(file.Tag.Pictures[0].Data.Data)
                    };
                }
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
            library.Tracks.Add(new Track(info.path, info.title, info.comment, info.imageBase64, genre, album, artist));
            library.SaveChanges();
            counters.tracks++;
        }

        static void ScanNewTracks(Library library, string libraryFolder, char tagSeparator) {

            string path;
            double progress;
            double previousProgress = 0.0;

            var counters = new Counters();
            Logger.Info("Scanning new tracks...");
            var paths = Directory.GetFiles(libraryFolder, "*.*", SearchOption.AllDirectories);
            for (int i = 0; i < paths.Length; i++) {
                path = paths[i];
                if (library.Tracks.Where(t => t.Path.Equals(path)).Any())
                    continue;
                var track = ParseTrack(path, tagSeparator);
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

        static void CleanOldTracks(Library library, string libraryFolder) {

            int removedTracks = 0;
            Logger.Info("Cleaning old tracks...");
            var libraryDirectory = new DirectoryInfo(libraryFolder);
            foreach (var track in library.Tracks) {
                bool inLibrary = System.IO.File.Exists(track.Path);
                if (inLibrary) {
                    var trackDirectory = new FileInfo(track.Path).Directory;
                    while (trackDirectory != null && !trackDirectory.FullName.Equals(libraryDirectory.FullName))
                        trackDirectory = trackDirectory.Parent;
                    inLibrary = trackDirectory != null;
                }
                if (!inLibrary) {
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
            string format = "Finished cleaning old tracks. Removed {0} genres, {1} albums, {2} artists and {3} tracks.";
            Logger.Info(format, oldGenres.Count, oldAlbums.Count, oldArtists.Count, removedTracks);
        }
    }
}
