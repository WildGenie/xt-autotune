using AutoTune.Search;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace AutoTune.Settings {

    public class AppSettings : SettingsBase<AppSettings> {

        internal static string FetchFilePath = Path.Combine(GetFolderPath(), "Fetch.js");
        internal static string LogFilePath = Path.Combine(GetFolderPath(), "AutoTune.log");
        internal static string StartupFilePath = Path.Combine(GetFolderPath(), "Startup.html");
        internal static string FetchExecutablePath = Path.Combine("Fetch", "AutoTune.Fetch.exe");
        internal static ProviderSettings GetProvider(string typeId) => Instance.Providers.Single(p => p.Id.Equals(typeId)).Item;

        public class ProviderSettings {
            public string FetchFile { get; set; }
            public string UrlPattern { get; set; }
            public string PlayUrlPattern { get; set; }
            public string DownloadUrlPattern { get; set; }
        }

        public class PostProcessingSettings {
            public string Command = "ffmpeg";
            public bool Enabled { get; set; } = true;
            public string Extension { get; set; } = "mp3";
            public bool KeepOriginal { get; set; } = false;
            public string Arguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";
        }

        public class FetchSettings {
            public int Retries { get; set; } = 55;
            public int Delay { get; set; } = 1000;
            public int Timeout { get; set; } = 60000;
        }

        public int SearchPageSize { get; set; } = 10;
        public int DownloadThreadCount { get; set; } = 0;
        public int PostProcessingThreadCount { get; set; } = 0;
        public bool PersistBrowserSessions { get; set; } = true;
        public bool AutoLoadMoreSearchResults { get; set; } = true;
        public FetchSettings Fetch { get; set; } = new FetchSettings();
        public List<Entry<ProviderSettings>> Providers = new List<Entry<ProviderSettings>>();
        public PostProcessingSettings PostProcessing { get; set; } = new PostProcessingSettings();

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
            InitializeResource(FetchFilePath, "AutoTune.Fetch.js");
            InitializeResource(StartupFilePath, "AutoTune.Startup.html");
            foreach (var entry in Providers)
                InitializeResource(Path.Combine(GetFolderPath(), entry.Item.FetchFile), "AutoTune." + entry.Item.FetchFile);
            if (Providers.Count != 0)
                return;
            Providers.Add(new Entry<ProviderSettings>(SearchEngine.VimeoTypeId, new ProviderSettings {
                FetchFile = "CatchVideo.html",
                DownloadUrlPattern = "https://vimeo.com/{0}",
                UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
                PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1"
            }));
            Providers.Add(new Entry<ProviderSettings>(SearchEngine.YouTubeTypeId, new ProviderSettings {
                FetchFile = "CatchVideo.html",
                DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
                UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
                PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white"
            }));
            Providers.Add(new Entry<ProviderSettings>(SearchEngine.DailyMotionTypeId, new ProviderSettings {
                FetchFile = "Convert2Mp3.html",
                DownloadUrlPattern = "https://www.dailymotion.com/video/{0}",
                UrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=false&sharing-enable=false",
                PlayUrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=true&sharing-enable=false"
            }));
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
