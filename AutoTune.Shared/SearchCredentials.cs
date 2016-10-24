using YAXLib;

namespace AutoTune.Shared {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    public class SearchCredentials {

        public string Key { get; set; } = "?";
        public string Secret { get; set; } = "?";
        public string ApplicationName { get; set; } = "?";
    }
}
