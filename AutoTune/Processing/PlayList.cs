using AutoTune.Settings;
using AutoTune.Shared;
using System.Collections.Generic;
using YAXLib;
using System;

namespace AutoTune.Processing {

    [YAXComment("Do not edit!")]
    class PlayList : SettingsBase<PlayList> {

        internal event EventHandler<EventArgs<SearchResult>> Play;
        internal PlayListMode Mode { get; set; } = PlayListMode.RepeatAll;
        internal List<SearchResult> Items { get; set; } = new List<SearchResult>();

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }

        internal void Stop() {
        }

        internal void Start() {
        }
    }
}
