using System;

namespace AutoTune.Drivers {

    public class DriverException : Exception {

        public DriverException(string message) :
            base(message) {
        }

        public DriverException(string message, Exception inner) :
            base(message, inner) {
        }
    }
}
