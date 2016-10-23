using AutoTune.Shared;
using System;
using System.Collections.Generic;

namespace AutoTune.Processing {

    interface IQueue {

        void Clear();
        bool Paused { get; set; }
        List<QueueItem> Items { get; }
        void Enqueue(QueueItem item, Action ifQueued);

        event EventHandler<EventArgs<QueueItem>> Error;
        event EventHandler<EventArgs<QueueItem>> Started;
        event EventHandler<EventArgs<QueueItem>> NotFound;
        event EventHandler<EventArgs<QueueItem>> Completed;
    }
}
