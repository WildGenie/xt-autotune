using System;
using System.Linq;
using System.Net;

namespace AutoTune.Shared {

    public static class Utility {

        public static string Normalize(string s) {
            Func<char, bool> pred = c => c == '-' || c == ' ' || 'a' <= c && c <= 'z' || 'A' <= c && c <= 'Z' || '0' <= c && c <= '9';
            return new string(s.Where(pred).ToArray()).ToLower().Trim();
        }

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
