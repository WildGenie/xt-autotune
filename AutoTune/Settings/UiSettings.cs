using AutoTune.Search;
using AutoTune.Shared;

namespace AutoTune.Settings {

    public class UiSettings : SettingsBase<UiSettings> {

        public string LastSearch { get; set; } = "";
        public bool FullScreen { get; set; } = false;
        public bool PlayerFull { get; set; } = false;
        public SearchResult CurrentTrack { get; set; }
        public bool LogCollapsed { get; set; } = true;
        public bool SearchCollapsed { get; set; } = false;
        public bool NotificationsCollapsed { get; set; } = false;
        public bool CurrentControlsCollapsed { get; set; } = false;
        public LogLevel TraceLevel { get; set; } = LogLevel.Info;
        
        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }
    }
}
