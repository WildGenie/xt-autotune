using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoTune.Processing {

    public abstract class Queue<T> : SettingsBase<T>, IQueue where T : Queue<T>, new() {

        private readonly object Lock = new object();
        private readonly List<QueueItem> InProgress = new List<QueueItem>();

        public event EventHandler<EventArgs<QueueItem>> Error;
        public event EventHandler<EventArgs<QueueItem>> Started;
        public event EventHandler<EventArgs<QueueItem>> NotFound;
        public event EventHandler<EventArgs<QueueItem>> Completed;

        bool paused = false;
        public List<QueueItem> Items { get; private set; } = new List<QueueItem>();

        public bool Paused {
            get {
                lock (Lock)
                return paused;
            }
            set {
                lock (Lock) {
                    paused = value;
                    Monitor.PulseAll(Lock);
                }
            }
        }

        internal abstract string GetAction();
        internal abstract int GetThreadCount();
        internal abstract void ProcessItem(QueueItem item);

        internal override void OnInitialized() {
        }

        public void Clear() {
            lock (Lock) {
                Items.Clear();
                InProgress.Clear();
            }
        }

        internal override void OnTerminating() {
            lock (Instance.Lock)
                   Instance.Items.InsertRange(0, Instance.InProgress);
        }

        internal static void Start() {
            int threads = Instance.GetThreadCount();
            threads = threads == 0 ? Environment.ProcessorCount : threads;
            for (int i = 0; i < threads; i++) {
                Thread thread = new Thread(Instance.Run);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        public void Enqueue(QueueItem item, Action ifQueued) {
            lock (Lock)
                if (!Items.Contains(item)) {
                    Items.Add(item);
                    ifQueued();
                    Monitor.Pulse(Lock);
                }
        }

        void Run() {
            var sender = typeof(DownloadQueue);
            while (true) {
                QueueItem item;
                lock (Lock) {
                    while (Items.Count == 0 || paused)
                        Monitor.Wait(Lock);
                    item = Items[0];
                    Items.RemoveAt(0);
                    InProgress.Add(item);
                }
                var args = new EventArgs<QueueItem>(item);
                Logger.Info("Starting {0} of {1}.", GetAction(), item.Search.Title);
                Started(sender, args);
                try {
                    ProcessItem(item);
                    Logger.Info("Finished {0} of {1}.", GetAction(), item.Search.Title);
                    Completed(sender, args);
                } catch (Exception error) {
                    Logger.Error(error, "Error during {0} of {1}.", GetAction(), item.Search.Title);
                    Error(sender, args);
                }
                lock (Lock)
                    InProgress.Remove(item);
            }
        }
    }
}
