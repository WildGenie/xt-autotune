using Newtonsoft.Json;

namespace AutoTune.Search.Similar {

    class Status {

        [JsonProperty]
        internal int code;
        [JsonProperty]
        internal string message;
    }
}
