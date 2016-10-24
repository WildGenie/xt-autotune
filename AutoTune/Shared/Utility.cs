using AutoTune.Search;
using AutoTune.Settings;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace AutoTune.Shared {

    static class Utility {

        internal static void SetLinkForeColors(LinkLabel link) {
            var theme = ThemeSettings.Instance;
            link.LinkColor = ColorTranslator.FromHtml(theme.ForeColor2);
            link.ActiveLinkColor = ColorTranslator.FromHtml(theme.ForeColor2);
        }

        static void WhenImageDownloadedAsync(string imageUrl, Action<Image> continue_) {
            using (WebClient client = new WebClient()) {
                client.DownloadDataCompleted += (s, evt) => {
                    using (MemoryStream stream = new MemoryStream(evt.Result))
                        continue_(Image.FromStream(stream));
                };
                client.DownloadDataAsync(new Uri(imageUrl));
            }
        }

        internal static void WhenImageDownloaded(string imageUrl, Action<Image> continue_) {
            if (imageUrl == null)
                continue_(null);
            else
                WhenImageDownloadedAsync(imageUrl, continue_);
        }

        internal static string GetUrl(SearchResult result) {
            return string.Format(AppSettings.GetProvider(result.TypeId).UrlPattern, result.VideoId);
        }

        internal static string GetPlayUrl(SearchResult result) {
            return string.Format(AppSettings.GetProvider(result.TypeId).PlayUrlPattern, result.VideoId);
        }
    }
}
