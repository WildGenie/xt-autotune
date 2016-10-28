using AutoTune.Search;
using AutoTune.Shared;
using YAXLib;

namespace AutoTune.Settings {

    [YAXComment("Do not edit!")]
    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class UiSettings : SettingsBase<UiSettings> {

        internal bool SearchCollapsed { get; set; }
        internal bool LogCollapsed { get; set; } = true;
        internal bool NotificationsCollapsed { get; set; }
        internal bool CurrentControlsCollapsed { get; set; }

        internal bool FullScreen { get; set; }
        internal bool PlayerFull { get; set; }
        internal string LastSearch { get; set; }
        internal bool SearchLocalOnly { get; set; }
        internal bool SearchFavouritesOnly { get; set; }
        internal SearchResult CurrentTrack { get; set; }
        internal LogLevel TraceLevel { get; set; } = LogLevel.Info;

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }
    }
}
