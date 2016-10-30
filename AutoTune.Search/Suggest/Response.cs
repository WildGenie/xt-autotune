using Newtonsoft.Json;

namespace AutoTune.Search.Suggest {

    class Response {

        [JsonProperty]
        internal Item[] data;
        [JsonProperty]
        internal Status status;
    }
}
