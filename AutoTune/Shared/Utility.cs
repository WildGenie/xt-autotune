using System;
using System.Drawing;
using System.IO;
using System.Net;

namespace AutoTune.Shared {

    public static class Utility {

        public static void WhenImageDownloaded(string imageUrl, Action<Image> continue_) {
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
    }
}
