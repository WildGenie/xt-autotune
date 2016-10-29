using System.Collections.Generic;
using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class ProviderSettings {

        internal string UrlPattern { get; set; }
        internal string HttpReferer { get; set; }
        internal bool Enabled { get; set; } = true;
        internal string PlayUrlPattern { get; set; }
        internal List<string> FetchFiles { get; set; }
        internal string DownloadUrlPattern { get; set; }
    }
}
