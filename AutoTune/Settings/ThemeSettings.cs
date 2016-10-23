using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    internal class ThemeSettings : SettingsBase<ThemeSettings> {

        [YAXComment("Primary background color.")]
        internal string BackColor1 { get; set; } = "#000000";
        [YAXComment("Secondary background color.")]
        internal string BackColor2 { get; set; } = "#222222";
        [YAXComment("Primary foreground color.")]
        internal string ForeColor1 { get; set; } = "#cccccc";
        [YAXComment("Secondary foreground color.")]
        internal string ForeColor2 { get; set; } = "#00adef";

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }
    }
}
