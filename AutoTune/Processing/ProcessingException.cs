using System;

namespace AutoTune.Processing {

    class ProcessingException : Exception {

        internal ProcessingException(string message) :
            base(message) {
        }

        internal ProcessingException(string message, Exception inner) :
            base(message, inner) {
        }
    }
}
