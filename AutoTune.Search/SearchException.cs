using System;

namespace AutoTune.Search {

    public class SearchException : Exception {

        public SearchException(string message) :
            base(message) {
        }

        public SearchException(string message, Exception inner) :
            base(message, inner) {
        }
    }
}
