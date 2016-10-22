using AutoTune.Drivers;
using AutoTune.Drivers.DailyMotion;
using AutoTune.Drivers.Vimeo;
using AutoTune.Drivers.YouTube;
using AutoTune.Shared;
using System.Collections.Generic;
using System.IO;

namespace AutoTune.Settings {

    public class AppSettings : SettingsBase<AppSettings> {

        public const string VimeoTypeId = "Vimeo";
        public const string YouTubeTypeId = "YouTube";
        public const string DailyMotionTypeId = "DailyMotion";

        public static string LogFilePath = Path.Combine(GetFolderPath(), "AutoTune.log");
        public static string StartupFilePath = Path.Combine(GetFolderPath(), "Startup.html");
        public static string FetchExecutablePath = Path.Combine("Fetch", "AutoTune.Fetch.exe");

        internal static readonly Dictionary<string, Search> Searchers = new Dictionary<string, Search>();
        internal static readonly Dictionary<string, SearchSettings> Searches = new Dictionary<string, SearchSettings>();

        internal static string GetFetchFilePath(string typeId) {
            return Path.Combine(GetFolderPath(), Searches[typeId].FetchFile);
        }

        public class SearchSettings {
            public string FetchFile { get; set; }
            public string UrlPattern { get; set; }
            public bool Enabled { get; set; } = true;
            public string PlayUrlPattern { get; set; }
            public string DownloadUrlPattern { get; set; }
        }

        public class PostProcessingSettings {
            public string Command = "ffmpeg";
            public int ThreadCount { get; set; } = 0;
            public bool Enabled { get; set; } = true;
            public string Extension { get; set; } = "mp3";
            public bool KeepOriginal { get; set; } = false;
            public string Arguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";
        }

        public int FetchTimeout = 60000;
        public int SearchPageSize { get; set; } = 10;
        public int DownloadThreadCount { get; set; } = 0;
        public bool PersistBrowserSessions { get; set; } = true;
        public bool AutoLoadMoreSearchResults { get; set; } = true;
        public PostProcessingSettings PostProcessing = new PostProcessingSettings();
        public SearchSettings DailyMotion = new SearchSettings {
            UrlPattern = "{0}",
            DownloadUrlPattern = "{0}",
            FetchFile = "Convert2Mp3.html",
            PlayUrlPattern = "{0}?autoplay=1"
        };
        public SearchSettings Vimeo = new SearchSettings {
            FetchFile = "CatchVideo.html",
            DownloadUrlPattern = "https://player.vimeo.com/video/{0}",
            UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
            PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1",
        };
        public SearchSettings YouTube = new SearchSettings {
            FetchFile = "CatchVideo.html",
            DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
            UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
            PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white"
        };

        protected override void OnTerminating() {
        }

        protected override void OnInitialized() {
            if (Vimeo.Enabled) {
                Searches.Add(VimeoTypeId, Vimeo);
                Searchers.Add(VimeoTypeId, new VimeoSearch());
            }
            if (YouTube.Enabled) {
                Searches.Add(YouTubeTypeId, YouTube);
                Searchers.Add(YouTubeTypeId, new YouTubeSearch());
            }
            if (DailyMotion.Enabled) {
                Searches.Add(DailyMotionTypeId, DailyMotion);
                Searchers.Add(DailyMotionTypeId, new DailyMotionSearch());
            }
            InitializeResource(StartupFilePath, "AutoTune.Startup.html");
            InitializeResource(Path.Combine(GetFolderPath(), Vimeo.FetchFile), "AutoTune." + Vimeo.FetchFile);
            InitializeResource(Path.Combine(GetFolderPath(), YouTube.FetchFile), "AutoTune." + YouTube.FetchFile);
            InitializeResource(Path.Combine(GetFolderPath(), DailyMotion.FetchFile), "AutoTune." + DailyMotion.FetchFile);
        }

        static void InitializeResource(string path, string resourceName) {
            if (File.Exists(path))
                return;
            string fileContents = null;
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (Stream resource = typeof(AppSettings).Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(resource))
                fileContents = reader.ReadToEnd();
            using (Stream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.Write(fileContents);
        }
    }
}
