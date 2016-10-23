using System;

namespace AutoTune.Shared {

    public class EventArgs<T>: EventArgs {

        public T Data { get; }
        
        public EventArgs(T data) {
            Data = data;
        }
    }
}
