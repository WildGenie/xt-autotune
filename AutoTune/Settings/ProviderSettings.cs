using System.Collections.Generic;
using System.IO;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class ProviderSettings {

        internal string EmbedFile { get; set; }
        internal string HttpReferer { get; set; }
        internal bool Enabled { get; set; } = true;
        internal List<string> FetchFiles { get; set; }
        internal string DownloadUrlPattern { get; set; }

        internal string GetEmbedFilePath() => Path.Combine(AppSettings.GetFolderPath(), EmbedFile);
        internal string GetDownloadUrl(string videoId) => string.Format(DownloadUrlPattern, videoId);
    }
}
