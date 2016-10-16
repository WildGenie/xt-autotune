using System;
using System.IO;

namespace AutoTune.Shared {

    public class LocalPaths : SettingsBase<LocalPaths> {

        const string FileName = "localpaths.xml";

        public string ExtractorExecutablePath { get; set; } = "AutoTune.Extractor";
        public string ProcessFolder { get; set; } = Path.Combine(GetFolderPath(), "process");
        public string DownloadFolder { get; set; } = Path.Combine(GetFolderPath(), "download");
        public string StartupFilePath { get; set; } = Path.Combine(GetFolderPath(), "startup.html");
        public string ExtractorFilePath { get; set; } = Path.Combine(GetFolderPath(), "extract.html");
        public string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public string BrowserCacheFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

        public static void Save() {
            Save(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
            Directory.CreateDirectory(Instance.TargetFolder);
            Directory.CreateDirectory(Instance.ProcessFolder);
            Directory.CreateDirectory(Instance.DownloadFolder);
            Directory.CreateDirectory(Instance.BrowserCacheFolder);
            InitializeResource(Instance.StartupFilePath, "AutoTune.startup.html");
            InitializeResource(Instance.ExtractorFilePath, "AutoTune.extract.html");
        }

        static void InitializeResource(string path, string resourceName) {
            if (File.Exists(path))
                return;
            string fileContents = null;
            Directory.CreateDirectory(Directory.GetParent(path).FullName);
            using (Stream resource = typeof(Settings).Assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(resource))
                fileContents = reader.ReadToEnd();
            using (Stream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(file))
                writer.Write(fileContents);
        }
    }
}
