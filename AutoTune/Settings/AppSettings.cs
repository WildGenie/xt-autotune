using AutoTune.Shared;
using System.IO;
using System;

namespace AutoTune.Settings {

    public class AppSettings : SettingsBase<AppSettings> {

        public static string LogFilePath = Path.Combine(GetFolderPath(), "autotune.log");
        public static string FetchFilePath = Path.Combine(GetFolderPath(), "fetch.html");
        public static string StartupFilePath = Path.Combine(GetFolderPath(), "startup.html");
        public static string FetchExecutablePath = Path.Combine("Fetch", "AutoTune.Fetch.exe");
        public static string BrowserCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

        public class DriverSettings {
            public bool Enabled { get; set; } = true;
            public string UrlPattern { get; set; }
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
        public string AppName { get; set; } = "AutoTune";
        public bool PersistBrowserSessions { get; set; } = true;
        public bool AutoLoadMoreSearchResults { get; set; } = true;
        public PostProcessingSettings PostProcessing = new PostProcessingSettings();
        public DriverSettings Vimeo = new DriverSettings {
            DownloadUrlPattern = "https://player.vimeo.com/video/{0}",
            UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
            PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1",
        };
        public DriverSettings YouTube = new DriverSettings {
            DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
            UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
            PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white"
        };

        protected override void OnTerminating() {
        }

        protected override void OnInitialized() {
            InitializeResource(FetchFilePath, "AutoTune.fetch.html");
            InitializeResource(StartupFilePath, "AutoTune.startup.html");
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
