using System;
using System.IO;

namespace AutoTune.Shared {

    public class Folders : SettingsBase<Folders> {

        const string FileName = "folders.xml";

        public string Process { get; set; } = Path.Combine(GetFolderPath(), "process");
        public string Download { get; set; } = Path.Combine(GetFolderPath(), "download");
        public string Target { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public string BrowserCache { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);

        public static void Save() {
            Save(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
            Directory.CreateDirectory(Instance.Target);
            Directory.CreateDirectory(Instance.Process);
            Directory.CreateDirectory(Instance.Download);
            Directory.CreateDirectory(Instance.BrowserCache);
        }
    }
}
