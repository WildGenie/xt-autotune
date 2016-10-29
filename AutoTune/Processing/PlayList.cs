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
        internal event EventHandler<EventArgs<SearchResult>> Play;

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
            }
        }

        internal void Start() {
            lock (Lock) {
                running = true;
                Monitor.Pulse(Lock);
            }
        }

        internal void Clear() {
            lock (Lock) {
                Items.Clear();
                running = false;
                playing = false;
            }
        }

        internal void Stopped() {
            lock (Lock) {
                playing = false;
                Monitor.Pulse(Lock);
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
                lock (Lock) {
                    while (!terminated && (playing || !running || Items.Count == 0))
                        Monitor.Wait(Lock);
                    if (terminated)
                        return;
                    playing = true;
                    Play(this, new EventArgs<SearchResult>(Next()));
                }
        }

        SearchResult Next() {
            if (current >= Items.Count)
                current = 0;
            return Items[current++];
        }
    }
}
