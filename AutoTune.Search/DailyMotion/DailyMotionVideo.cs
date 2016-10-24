using Newtonsoft.Json;

namespace AutoTune.Search.DailyMotion {

    class DailyMotionVideo {

        [JsonProperty]
        internal string id;
        [JsonProperty]
        internal string url;
        [JsonProperty]
        internal string title;
        [JsonProperty]
        internal string embed_url;
        [JsonProperty]
        internal string description;
        [JsonProperty]
        internal string thumbnail_120_url;
    }
}
