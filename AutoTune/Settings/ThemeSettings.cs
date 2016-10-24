﻿using YAXLib;

namespace AutoTune.Settings {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    class ThemeSettings : SettingsBase<ThemeSettings> {

        internal string BackColor1 { get; set; } = "#000000";
        internal string BackColor2 { get; set; } = "#222222";
        internal string ForeColor1 { get; set; } = "#cccccc";
        internal string ForeColor2 { get; set; } = "#00adef";

        internal override void OnTerminating() {
        }

        internal override void OnInitialized() {
        }
    }
}
