using Newtonsoft.Json;

namespace AutoTune.Search.Similar {

    class Item {

        [JsonProperty]
        internal string id;
        [JsonProperty]
        internal string name;
        [JsonProperty]
        internal string decade;
        [JsonProperty]
        internal double similarity;
        [JsonProperty]
        internal string main_genre;
    }
}
