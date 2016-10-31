using Newtonsoft.Json;

namespace AutoTune.Search.Similar {

    class Response {

        [JsonProperty]
        internal Item[] data;
        [JsonProperty]
        internal Status status;
    }
}
