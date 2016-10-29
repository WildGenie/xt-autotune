using AutoTune.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class AppSettings : SettingsBase<AppSettings> {

        internal static string NoImageAvailableBase64;
        internal static string FetchFilePath = Path.Combine(GetFolderPath(), "fetch.js");
        internal static string LogFilePath = Path.Combine(GetFolderPath(), "autotune.log");
        internal static string FetchExecutablePath = Path.Combine("fetch", "AutoTune.Fetch.exe");
        internal static string NoImageAvailablePath = Path.Combine(GetFolderPath(), "no-image-available.png");
        internal static ProviderSettings GetProvider(string typeId) => Instance.Providers.Single(p => p.Key.Equals(typeId)).Value;

        internal int FetchRetries { get; set; } = 55;
        internal int FetchDelay { get; set; } = 1000;
        internal int FetchTimeout { get; set; } = 60000;

        internal char TagSeparator = '-';
        internal int SearchPageSize { get; set; } = 10;
        internal int DownloadThreadCount { get; set; } = 0;
        internal bool ScrollToEndOnMoreResults { get; set; } = false;
        internal bool LoadMoreResultsOnScrollToEnd { get; set; } = true;
        internal int ScanLibraryInterval { get; set; } = 60 * 60 * 1000;
        internal bool EmbedThumbnailAfterPostProcessing { get; set; } = true;
        internal bool EmbedDescriptionAfterPostProcessing { get; set; } = true;

        internal bool PostProcessingEnabled { get; set; } = true;
        internal int PostProcessingThreadCount { get; set; } = 0;
        internal string PostProcessingExtension { get; set; } = "mp3";
        internal string PostProcessingCommand { get; set; } = "ffmpeg";
        internal bool PostProcessingKeepOriginal { get; set; } = true;
        internal string PostProcessingArguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";

        [YAXDictionary(EachPairName = "Provider", KeyName = "Id", ValueName = "Settings", SerializeKeyAs = YAXNodeTypes.Attribute)]
        internal Dictionary<string, ProviderSettings> Providers { get; set; } = new Dictionary<string, ProviderSettings> {
             { SearchEngine.LocalTypeId, new ProviderSettings {
                Enabled = true,
                HttpReferer = null,
                UrlPattern = "{0}",
                PlayUrlPattern = "{0}",
                DownloadUrlPattern = "{0}",
                FetchFiles = new List<string>() } } ,
            { SearchEngine.VimeoTypeId, new ProviderSettings {
                Enabled = true,
                HttpReferer = "http://www.vimeo.com",
                DownloadUrlPattern = "https://vimeo.com/{0}",
                FetchFiles = new List<string>() { "fetch-catchvideo.html" },
                UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
                PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1" } },
            { SearchEngine.YouTubeTypeId, new ProviderSettings {
                Enabled = true,
                HttpReferer = "http://www.youtube.com",
                DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
                UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
                PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white",
                FetchFiles = new List<string>() { "fetch-convert2mp3.html", "fetch-catchvideo.html" } } },
            { SearchEngine.DailyMotionTypeId, new ProviderSettings {
                Enabled = true,
                HttpReferer = "http://www.dailymotion.com",
                DownloadUrlPattern = "https://www.dailymotion.com/video/{0}",
                FetchFiles = new List<string>() { "fetch-convert2mp3.html" },
                UrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=false&sharing-enable=false",
                PlayUrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=true&sharing-enable=false"
            } }
        };

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
            InitializeResource(FetchFilePath, "AutoTune.fetch.js");
            InitializeResource(NoImageAvailablePath, "AutoTune.no-image-available.png");
            NoImageAvailableBase64 = Convert.ToBase64String(File.ReadAllBytes(NoImageAvailablePath));
            foreach (var entry in Providers)
                foreach (var fetchFile in entry.Value.FetchFiles)
                    InitializeResource(Path.Combine(GetFolderPath(), fetchFile), "AutoTune." + fetchFile);
        }

        static void InitializeResource(string path, string resourceName) {
            if (File.Exists(path))
                return;
            byte[] fileContents = null;
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (Stream resource = typeof(AppSettings).Assembly.GetManifestResourceStream(resourceName))
            using (BinaryReader reader = new BinaryReader(resource))
                fileContents = reader.ReadBytes((int)resource.Length);
            using (Stream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new BinaryWriter(file))
                writer.Write(fileContents);
        }
    }
}
