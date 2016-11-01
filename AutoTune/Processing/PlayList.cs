using AutoTune.Settings;
using System.Linq;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using YAXLib;
using System.Threading;

namespace AutoTune.Processing {

    [YAXComment("Do not edit!")]
    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AttributedFieldsOnly)]
    class Playlist : SettingsBase<Playlist> {

        private int current;
        private bool running;
        private bool playing;
        private bool terminated;
        private readonly object Lock = new object();
        internal event EventHandler<EventArgs<SearchResult>> Next;

        [YAXSerializableField]
        internal PlaylistMode Mode { get; set; } = PlaylistMode.RepeatAll;
        [YAXSerializableField]
        internal List<SearchResult> Items { get; set; } = new List<SearchResult>();

        internal override void OnTerminating() {
            lock (Lock) {
                terminated = true;
                Monitor.Pulse(Lock);
            }
        }

        internal override void OnInitialized() {
            Thread thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Start();
        }

        internal void Stop() {
            lock (Lock) {
                running = false;
                playing = false;
            }
        }

        internal void Start() {
            lock (Lock) {
                running = true;
                playing = false;
                Monitor.Pulse(Lock);
            }
        }

        internal void PlayNext() {
            lock (Lock) {
                if (Items.Count != 0)
                    Next(this, new EventArgs<SearchResult>(GetNext()));
            }
        }

        internal void Clear() {
            lock (Lock) {
                Items.Clear();
                running = false;
                playing = false;
            }
        }

        internal void Randomize() {
            lock (Lock) {
                var random = new Random();
                Items = new List<SearchResult>(Items.OrderBy(_ => random.NextDouble()));
            }
        }

        internal void Stopped() {
            lock (Lock) {
                playing = false;
                Monitor.Pulse(Lock);
            }
        }

        internal void Play(SearchResult result) {
            lock (Lock) {
                int index = Items.FindIndex(i => i.TypeId.Equals(result.TypeId) && i.VideoId.Equals(result.VideoId));
                current = index;
                playing = true;
                Next(this, new EventArgs<SearchResult>(GetNext()));
            }
        }

        internal void Remove(SearchResult result) {
            lock (Lock)
                Items.RemoveAll(i => i.TypeId.Equals(result.TypeId) && i.VideoId.Equals(result.VideoId));
        }

        internal bool Add(SearchResult result) {
            lock (Lock) {
                if (Items.Any(i => i.TypeId.Equals(result.TypeId) && i.VideoId.Equals(result.VideoId)))
                    return false;
                Items.Add(result);
                return true;
            }
        }

        void Run() {
            while (true)
                try {
                    lock (Lock) {
                        while (!terminated && (playing || !running || Items.Count == 0))
                            Monitor.Wait(Lock);
                        if (terminated)
                            return;
                        playing = true;
                        Next(this, new EventArgs<SearchResult>(GetNext()));
                    }
                } catch (Exception e) {
                    Logger.Error(e, "Error during playlist processing.");
                }
        }

        SearchResult GetNext() {
            int newCurrent;
            if (current >= Items.Count || current < 0)
                current = 0;
            var result = Items[current];
            switch (Mode) {
                case PlaylistMode.RepeatTrack:
                    break;
                case PlaylistMode.RepeatAll:
                    current++;
                    break;
                case PlaylistMode.Random:
                    newCurrent = new Random().Next(0, Items.Count);
                    while (newCurrent == current && Items.Count != 1)
                        newCurrent = new Random().Next(0, Items.Count);
                    current = newCurrent;
                    break;
            }
            return result;
        }
    }
}
