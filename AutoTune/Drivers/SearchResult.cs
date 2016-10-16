using AutoTune.Shared;
using System.Collections.Generic;

namespace AutoTune.Drivers {

    class SearchResult {

        public SearchState State { get; set; }
        public IEnumerable<Result> Results { get; set; }
    }
}
