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
        internal bool PostProcessingKeepOriginal { get; set; } = false;
        internal string PostProcessingArguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";

        [YAXDictionary(EachPairName = "Provider", KeyName = "Id", ValueName = "Settings", SerializeKeyAs = YAXNodeTypes.Attribute)]
        internal Dictionary<string, ProviderSettings> Providers { get; set; } = new Dictionary<string, ProviderSettings> {
             { SearchEngine.LocalTypeId, new ProviderSettings {
                FetchFile = null,
                DownloadUrlPattern = "{0}",
                UrlPattern = "{0}",
                PlayUrlPattern = "{0}" } },
            { SearchEngine.VimeoTypeId, new ProviderSettings {
                FetchFile = "fetch-catchvideo.html",
                DownloadUrlPattern = "https://vimeo.com/{0}",
                UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
                PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1" } },
            { SearchEngine.YouTubeTypeId, new ProviderSettings {
                FetchFile = "fetch-catchvideo.html",
                DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
                UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
                PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white" } },
            { SearchEngine.DailyMotionTypeId, new ProviderSettings {
                FetchFile = "fetch-convert2mp3.html",
                DownloadUrlPattern = "https://www.dailymotion.com/video/{0}",
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
                if (!string.IsNullOrEmpty(entry.Value.FetchFile))
                    InitializeResource(Path.Combine(GetFolderPath(), entry.Value.FetchFile), "AutoTune." + entry.Value.FetchFile);
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
