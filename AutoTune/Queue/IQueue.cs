using AutoTune.Shared;
using System;
using System.Collections.Generic;

namespace AutoTune.Queue {

    public interface IQueue {

        void Clear();
        bool Paused { get; set; }
        List<Result> Items { get; }
        void Enqueue(Result result, Action ifQueued);

        event EventHandler<EventArgs<Result>> Error;
        event EventHandler<EventArgs<Result>> Started;
        event EventHandler<EventArgs<Result>> NotFound;
        event EventHandler<EventArgs<Result>> Completed;
    }
}
