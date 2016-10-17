using AutoTune.Shared;

namespace AutoTune.Settings {

    public class ThemeSettings : SettingsBase<ThemeSettings> {

        public string BackColor1 { get; set; } = "#000000";
        public string BackColor2 { get; set; } = "#222222";
        public string ForeColor1 { get; set; } = "#cccccc";
        public string ForeColor2 { get; set; } = "#f12b24";

        protected override void OnTerminating() {
        }

        protected override void OnInitialized() {
        }
    }
}
