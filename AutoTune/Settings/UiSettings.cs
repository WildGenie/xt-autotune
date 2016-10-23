using AutoTune.Search;
using AutoTune.Shared;
using YAXLib;

namespace AutoTune.Settings {

    [YAXComment("Do not modify! These values are overwritten when the application is closed.")]
    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    internal class UiSettings : SettingsBase<UiSettings> {

        internal string LastSearch { get; set; } = "";
        internal bool FullScreen { get; set; } = false;
        internal bool PlayerFull { get; set; } = false;
        internal SearchResult CurrentTrack { get; set; }
        internal bool LogCollapsed { get; set; } = true;
        internal bool SearchCollapsed { get; set; } = false;
        internal bool NotificationsCollapsed { get; set; } = false;
        internal bool CurrentControlsCollapsed { get; set; } = false;
        internal LogLevel TraceLevel { get; set; } = LogLevel.Info;
        
        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }
    }
}
