using AutoTune.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AutoTune.Settings {

    public class UserSettings : SettingsBase<UserSettings> {

        internal static SearchCredentials GetCredentials(string typeId) => Instance.Credentials.Single(c => c.Id.Equals(typeId)).Item;

        [XmlIgnore]
        internal string ProcessFolder => Path.Combine(TempFolder, "Process");
        [XmlIgnore]
        internal string DownloadFolder => Path.Combine(TempFolder, "Download");

        public class SearchCredentials {
            public string APIKey { get; set; }
            public string ClientSecret { get; set; }
        }

        public string AppName { get; set; } = "registered-app-name";
        public string TempFolder { get; set; } = Path.Combine(GetFolderPath(), "Temp");
        public string BrowserCacheFolder = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        public List<Entry<SearchCredentials>> Credentials { get; set; } = new List<Entry<SearchCredentials>>();
        public string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
            Directory.CreateDirectory(Instance.TempFolder);
            Directory.CreateDirectory(Instance.TargetFolder);
            Directory.CreateDirectory(Instance.ProcessFolder);
            Directory.CreateDirectory(Instance.DownloadFolder);
            Directory.CreateDirectory(Instance.BrowserCacheFolder);
            if (Credentials.Count != 0)
                return;
            Credentials.Add(new Entry<SearchCredentials>(SearchEngine.VimeoTypeId, new SearchCredentials() {
                APIKey = "vimeo-client-id",
                ClientSecret = "vimeo-client-secret"
            }));
            Credentials.Add(new Entry<SearchCredentials>(SearchEngine.YouTubeTypeId, new SearchCredentials() {
                APIKey = "youtube-api-key",
                ClientSecret = "not-needed"
            }));
            Credentials.Add(new Entry<SearchCredentials>(SearchEngine.DailyMotionTypeId, new SearchCredentials() {
                APIKey = "not-needed",
                ClientSecret = "not-needed"
            }));
        }
    }
}
