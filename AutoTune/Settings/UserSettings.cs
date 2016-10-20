using AutoTune.Shared;
using System;
using System.IO;
using System.Xml.Serialization;

namespace AutoTune.Settings {

    public class UserSettings : SettingsBase<UserSettings> {

        [XmlIgnore]
        public string ProcessFolder => Path.Combine(TempFolder, "Process");
        [XmlIgnore]
        public string DownloadFolder => Path.Combine(TempFolder, "Download");

        public string AppName { get; set; } = "?";
        public string YouTubeAPIKey { get; set; } = "?";
        public string VimeoClientId { get; set; } = "?";
        public string VimeoClientSecret { get; set; } = "?";
        public string TempFolder { get; set; } = Path.Combine(GetFolderPath(), "Temp");
        public string BrowserCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        public string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        protected override void OnTerminating() {
        }

        protected override void OnInitialized() {
            Directory.CreateDirectory(Instance.TempFolder);
            Directory.CreateDirectory(Instance.TargetFolder);
            Directory.CreateDirectory(Instance.ProcessFolder);
            Directory.CreateDirectory(Instance.DownloadFolder);
            Directory.CreateDirectory(Instance.BrowserCacheFolder);
        }
    }
}
