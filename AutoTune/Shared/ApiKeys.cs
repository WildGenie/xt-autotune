namespace AutoTune.Shared {

    public class ApiKeys : SettingsBase<ApiKeys> {

        private const string FileName = "apikeys.xml";

        public string YouTubeAPIKey { get; set; } = "?";
        public string VimeoClientId { get; set; } = "?";
        public string VimeoClientSecret { get; set; } = "?";

        public static void Save() {
            Save(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
        }
    }
}
