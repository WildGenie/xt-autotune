using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class ProviderSettings {

        internal string FetchFile { get; set; }
        internal string UrlPattern { get; set; }
        internal string PlayUrlPattern { get; set; }
        internal string DownloadUrlPattern { get; set; }
    }
}
