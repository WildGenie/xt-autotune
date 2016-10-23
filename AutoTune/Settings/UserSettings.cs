using AutoTune.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    internal class UserSettings : SettingsBase<UserSettings> {

        internal static SearchCredentials GetCredentials(string typeId) => Instance.Credentials.Single(c => c.Key.Equals(typeId)).Value;

        [YAXDontSerialize]
        internal string ProcessFolder => Path.Combine(TempFolder, "Process");
        [YAXDontSerialize]
        internal string DownloadFolder => Path.Combine(TempFolder, "Download");

        [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
        internal class SearchCredentials {
            [YAXComment("API key or client id, must match the specified application name.")]
            internal string APIKey { get; set; }
            [YAXComment("Client secret, must match the specified application name.")]
            internal string ClientSecret { get; set; }
        }

        [YAXComment("Application name as specified when generating API keys.")]
        internal string ApplicationName { get; set; } = "registered-app-name";
        [YAXComment("Downloads and post-processing files are stored here.")]
        internal string TempFolder { get; set; } = Path.Combine(GetFolderPath(), "Temp");
        [YAXComment("Files are placed here after post-processing.")]
        internal string TargetFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        [YAXComment("Search provider credentials.")]
        [YAXDictionary(EachPairName = "Provider", KeyName = "Id", ValueName = "Credentials", SerializeKeyAs = YAXNodeTypes.Attribute)]
        internal Dictionary<string, SearchCredentials> Credentials { get; set; } = new Dictionary<string, SearchCredentials>() {
            { SearchEngine.VimeoTypeId, new SearchCredentials() {
                APIKey = "vimeo-client-id",
                ClientSecret = "vimeo-client-secret" } },
            { SearchEngine.YouTubeTypeId, new SearchCredentials() {
                APIKey = "youtube-api-key",
                ClientSecret = "not-needed" } },
            { SearchEngine.DailyMotionTypeId, new SearchCredentials() {
                APIKey = "not-needed",
                ClientSecret = "not-needed" } }
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
