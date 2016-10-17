using AutoTune.Shared;

namespace AutoTune.Settings {

    public class UiSettings : SettingsBase<UiSettings> {

        public Result CurrentTrack { get; set; }
        public string LastSearch { get; set; } = "";
        public bool LogCollapsed { get; set; } = true;
        public bool SearchCollapsed { get; set; } = false;
        public bool NotificationsCollapsed { get; set; } = false;
        public bool CurrentControlsCollapsed { get; set; } = false;
        public LogLevel TraceLevel { get; set; } = LogLevel.Info;

        protected override void OnTerminating() {
        }

        protected override void OnInitialized() {
        }
    }
}
