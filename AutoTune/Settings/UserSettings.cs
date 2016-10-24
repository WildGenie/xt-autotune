using AutoTune.Search;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class UserSettings : SettingsBase<UserSettings> {

        [YAXDontSerialize]
        internal string ProcessFolder => Path.Combine(TempFolder, "Process");
        [YAXDontSerialize]
        internal string DownloadFolder => Path.Combine(TempFolder, "Download");

        internal static SearchCredentials GetCredentials(string typeId) {
            return Instance.Credentials.Single(c => c.Key.Equals(typeId)).Value;
        }

        internal string ApplicationName { get; set; }
        internal string TempFolder { get; set; } = Path.Combine(GetFolderPath(), "Temp");
        internal string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        internal string LibraryFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        [YAXDictionary(EachPairName = "Provider", KeyName = "Id", ValueName = "Credentials", SerializeKeyAs = YAXNodeTypes.Attribute)]
        internal Dictionary<string, SearchCredentials> Credentials { get; set; } = new Dictionary<string, SearchCredentials>() {
            { SearchEngine.VimeoTypeId, new SearchCredentials() },
            { SearchEngine.YouTubeTypeId, new SearchCredentials() }
        };

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
            Directory.CreateDirectory(Instance.TempFolder);
            Directory.CreateDirectory(Instance.TargetFolder);
            Directory.CreateDirectory(Instance.ProcessFolder);
            Directory.CreateDirectory(Instance.DownloadFolder);
        }
    }
}
