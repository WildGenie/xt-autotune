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
        internal string ProcessFolder => Path.Combine(TempFolder, "process");
        [YAXDontSerialize]
        internal string DownloadFolder => Path.Combine(TempFolder, "download");

        internal static SearchCredentials GetCredentials(string typeId) {
            return Instance.Credentials.Single(c => c.Key.Equals(typeId)).Value;
        }

        internal string MusicGraphAPIKey { get; set; }
        internal string TempFolder { get; set; } = Path.Combine(GetFolderPath(), "temp");
        internal string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        internal string LibraryFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        internal string PlaylistDumpFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "AutoTunePlaylist");

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
            Directory.CreateDirectory(Instance.LibraryFolder);
            Directory.CreateDirectory(Instance.ProcessFolder);
            Directory.CreateDirectory(Instance.DownloadFolder);
            Directory.CreateDirectory(Instance.PlaylistDumpFolder);
        }
    }
}
