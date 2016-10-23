using AutoTune.Search;
using AutoTune.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;

namespace AutoTune.Shared {

    internal static class Utility {

        internal static void WhenImageDownloaded(string imageUrl, Action<Image> continue_) {
            if (imageUrl == null)
                continue_(null);
            else
                using (WebClient client = new WebClient()) {
                    client.DownloadDataCompleted += (s, evt) => {
                        using (MemoryStream stream = new MemoryStream(evt.Result))
                            continue_(Image.FromStream(stream));
                    };
                    client.DownloadDataAsync(new Uri(imageUrl));
                }
        }

        internal static string GetUrl(SearchResult result) {
            return string.Format(AppSettings.GetProvider(result.TypeId).UrlPattern, result.VideoId);
        }

        internal static string GetPlayUrl(SearchResult result) {
            return string.Format(AppSettings.GetProvider(result.TypeId).PlayUrlPattern, result.VideoId);
        }

        internal static Dictionary<string, SearchCredentials> GetSearchCredentials() {
            var result = new Dictionary<string, SearchCredentials>();
            foreach (var entry in UserSettings.Instance.Credentials)
                result.Add(entry.Id, new SearchCredentials(UserSettings.Instance.AppName, entry.Item.APIKey, entry.Item.ClientSecret));
            return result;
        }
    }
}
