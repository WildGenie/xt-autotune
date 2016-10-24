using Newtonsoft.Json;
using System.Collections.Generic;

namespace AutoTune.Search.DailyMotion {

    class DailyMotionResponse {

        [JsonProperty]
        internal int page;
        [JsonProperty]
        internal int total;
        [JsonProperty]
        internal List<DailyMotionVideo> list;
    }
}
