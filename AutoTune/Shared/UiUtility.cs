using AutoTune.Settings;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AutoTune.Shared {

    public static class UiUtility {

        internal static void SetToggleForeColors(LinkLabel link) {
            var theme = ThemeSettings.Instance;
            link.LinkColor = ColorTranslator.FromHtml(theme.ForeColor1);
            link.ActiveLinkColor = ColorTranslator.FromHtml(theme.ForeColor1);
        }

        internal static void SetLinkForeColors(LinkLabel link) {
            var theme = ThemeSettings.Instance;
            link.LinkColor = ColorTranslator.FromHtml(theme.ForeColor2);
            link.ActiveLinkColor = ColorTranslator.FromHtml(theme.ForeColor2);
        }

        internal static Image ImageFromBase64(string base64) {
            if (base64 == null)
                return null;
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64)))
                return Image.FromStream(stream);
        }

        public static void WithLayoutSuspended(this Control control, Action action) {
            try {
                control.SuspendLayout();
                action();
            } finally {
                control.ResumeLayout();
            }
        }
    }
}
