using Newtonsoft.Json;

namespace AutoTune.Search.Suggest {

    class Status {

        [JsonProperty]
        internal int code;
        [JsonProperty]
        internal string message;
    }
}
