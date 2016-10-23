namespace AutoTune.Search {

    public class SearchCredentials {

        public string Key { get; }
        public string Secret { get; }
        public string AppName { get; }

        public SearchCredentials(string appName, string key, string secret ) {
            Key = key;
            Secret = secret;
            AppName = appName;
        }
    }
}
