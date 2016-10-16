using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AutoTune.Queue {

    public abstract class Queue<T> : SettingsBase<T>, IQueue where T : Queue<T>, new() {

        private readonly object Lock = new object();
        private readonly List<Result> InProgress = new List<Result>();

        public event EventHandler<EventArgs<Result>> Started;
        public event EventHandler<EventArgs<Result>> NotFound;
        public event EventHandler<EventArgs<Result>> Completed;
        public event EventHandler<EventArgs<Result>> Error;

        public List<Result> Items { get; private set; } = new List<Result>();
        bool paused = false;

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

        protected abstract string GetAction();
        protected abstract int GetThreadCount();
        protected abstract void ProcessItem(Result result);

        public void Clear() {
            lock (Lock) {
                Items.Clear();
                InProgress.Clear();
            }
        }

        public static void Start() {
            int threads = Instance.GetThreadCount();
            threads = threads == 0 ? Environment.ProcessorCount : threads;
            for (int i = 0; i < threads; i++) {
                Thread thread = new Thread(Instance.Run);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        protected static void Terminate(string fileName) {
            lock (Instance.Lock)
                    Instance.Items.InsertRange(0, Instance.InProgress);
            Save(fileName);
        }

        public void Enqueue(Result result, Action ifQueued) {
            lock (Lock)
                if (!Items.Contains(result)) {
                    Items.Add(result);
                    ifQueued();
                    Monitor.Pulse(Lock);
                }
        }

        void Run() {
            var sender = typeof(DownloadQueue);
            while (true) {
                Result result = null;
                lock (Lock) {
                    while (Items.Count == 0 || paused)
                        Monitor.Wait(Lock);
                    result = Items[0];
                    Items.RemoveAt(0);
                    InProgress.Add(result);
                }
                var args = new EventArgs<Result>(result);
                Logger.Info("Starting {0} of {1}.", GetAction(), result.Title);
                Started(sender, args);
                try {
                    ProcessItem(result);
                    Logger.Info("Finished {0} of {1}.", GetAction(), result.Title);
                    Completed(sender, args);
                } catch (Exception error) {
                    Logger.Error(error, "Error during {0} of {1}.", GetAction(), result.Title);
                    var data = new Tuple<Result, Exception>(result, error);
                    Error(sender, args);
                }
                lock (Lock)
                    InProgress.Remove(result);
            }
        }
    }
}
