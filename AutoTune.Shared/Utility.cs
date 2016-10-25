using System;
using System.Net;

namespace AutoTune.Shared {

    public static class Utility {

        public static byte[] Download(string url) {
            if (url == null)
                return null;
            try {
                using (WebClient client = new WebClient())
                    return client.DownloadData(url);
            } catch (Exception e) {
                Logger.Error(e, "Failed to download data for {0}.", url);
                return null;
            }
        }
    }
}
