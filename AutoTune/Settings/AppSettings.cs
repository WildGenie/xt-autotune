using AutoTune.Search;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    internal class AppSettings : SettingsBase<AppSettings> {

        internal static string FetchFilePath = Path.Combine(GetFolderPath(), "Fetch.js");
        internal static string LogFilePath = Path.Combine(GetFolderPath(), "AutoTune.log");
        internal static string StartupFilePath = Path.Combine(GetFolderPath(), "Startup.html");
        internal static string FetchExecutablePath = Path.Combine("Fetch", "AutoTune.Fetch.exe");
        internal static ProviderSettings GetProvider(string typeId) => Instance.Providers.Single(p => p.Key.Equals(typeId)).Value;

        [YAXComment("Paging size for search requests.")]
        internal int SearchPageSize { get; set; } = 10;
        [YAXComment("Number of download threads, 0 = auto.")]
        internal int DownloadThreadCount { get; set; } = 0;
        [YAXComment("Automatically load more search results when scrolling to the end of the result list.")]
        internal bool AutoLoadMoreSearchResults { get; set; } = true;

        [YAXComment("Number of retries when executing the fetch script.")]
        internal int FetchRetries { get; set; } = 55;
        [YAXComment("Millisecond delay between retries when executing the fetch script.")]
        internal int FetchDelay { get; set; } = 1000;
        [YAXComment("Millisecond timeout before the fetch script is aborted.")]
        internal int FetchTimeout { get; set; } = 60000;

        [YAXComment("Enables/disables post processing.")]
        internal bool PostProcessingEnabled { get; set; } = true;
        [YAXComment("Number of post-processing threads, 0 = auto.")]
        internal int PostProcessingThreadCount { get; set; } = 0;
        [YAXComment("Post processing command.")]
        internal string PostProcessingCommand { get; set; } = "ffmpeg";
        [YAXComment("Extension of the post-processed file.")]
        internal string PostProcessingExtension { get; set; } = "mp3";
        [YAXComment("If true, keep the downloaded video in the target folder as well as the post-processed file.")]
        internal bool PostProcessingKeepOriginal { get; set; } = false;
        [YAXComment("Post processor command line options. 0 = full path of input, 1 = full path of output without extension, 2 = output extension.")]
        internal string PostProcessingArguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";

        [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
        internal class ProviderSettings {
            [YAXComment("The file that should be used to fetch a download link from a video url.")]
            internal string FetchFile { get; set; }
            [YAXComment("Embedded player url with autoplay disabled. 0 = Video id.")]
            internal string UrlPattern { get; set; }
            [YAXComment("Embedded player url with autoplay enabled. 0 = Video id.")]
            internal string PlayUrlPattern { get; set; }
            [YAXComment("Generic url used to fetch download link. 0 = Video id.")]
            internal string DownloadUrlPattern { get; set; }
        }

        [YAXComment("Search provider settings.")]
        [YAXDictionary(EachPairName = "Provider", KeyName = "Id", ValueName = "Settings", SerializeKeyAs = YAXNodeTypes.Attribute)]
        internal Dictionary<string, ProviderSettings> Providers { get; set; } = new Dictionary<string, ProviderSettings> {
             { SearchEngine.LocalTypeId, new ProviderSettings {
                FetchFile = null,
                DownloadUrlPattern = "{0}",
                UrlPattern = "{0}",
                PlayUrlPattern = "{0}" } },
            { SearchEngine.VimeoTypeId, new ProviderSettings {
                FetchFile = "FetchCatchVideo.html",
                DownloadUrlPattern = "https://vimeo.com/{0}",
                UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0",
                PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1" } },
            { SearchEngine.YouTubeTypeId, new ProviderSettings {
                FetchFile = "FetchCatchVideo.html",
                DownloadUrlPattern = "https://www.youtube.com/watch?v={0}",
                UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&color=white",
                PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&color=white" } },
            { SearchEngine.DailyMotionTypeId, new ProviderSettings {
                FetchFile = "FetchConvert2Mp3.html",
                DownloadUrlPattern = "https://www.dailymotion.com/video/{0}",
                UrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=false&sharing-enable=false",
                PlayUrlPattern = "https://www.dailymotion.com/embed/video/{0}?autoplay=true&sharing-enable=false"
            } }
        };

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
            InitializeResource(FetchFilePath, "AutoTune.Fetch.js");
            InitializeResource(StartupFilePath, "AutoTune.Startup.html");
            foreach (var entry in Providers)
                if (!string.IsNullOrEmpty(entry.Value.FetchFile))
                    InitializeResource(Path.Combine(GetFolderPath(), entry.Value.FetchFile), "AutoTune." + entry.Value.FetchFile);
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
