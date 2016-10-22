using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace AutoTune.Drivers {

    static class Download {

        const string AppTimeout = "app-timeout";
        const string ScriptError = "script-error";
        const string ContentType = "Content-Type";
        const string ScriptTimeout = "script-timeout";
        const string ContentDisposition = "Content-Disposition";
        const string AttachmentFileName = "attachment; filename=";
        static readonly Dictionary<string, string> ExtensionsByContentType = new Dictionary<string, string>();

        static string Fetch(Result result) {
            var app = AppSettings.Instance;
            string delimiter = Guid.NewGuid().ToString();
            string fetchFilePath = AppSettings.GetFetchFilePath(result.Type);
            Logger.Debug("Downloading " + result.DownloadUrl + ".");
            string args = string.Format("\"{0}\" {1} {2} {3}", fetchFilePath, result.DownloadUrl, delimiter, app.FetchTimeout);
            ProcessStartInfo info = new ProcessStartInfo(AppSettings.FetchExecutablePath, args);
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            Logger.Info("Fetching " + result.Title + "...");
            using (Process fetch = Process.Start(info)) {
                fetch.WaitForExit();
                string output = fetch.StandardOutput.ReadToEnd();
                string link = output.Substring(output.IndexOf(delimiter) + delimiter.Length);
                link = link.Substring(0, link.IndexOf(delimiter));
                if (fetch.ExitCode != 0) {
                    Logger.Debug("Fetch error: console output: {0}.", output);
                    string format = "Fetching {0} returned with code: {1}.";
                    throw new DriverException(string.Format(format, result.Title, link));
                }
                return link;
            }
        }

        public static void Execute(Result result) {
            byte[] bytes;
            string fileName = null;
            string contentType = null;
            string link = Fetch(result);
            var user = UserSettings.Instance;
            Logger.Info("Downloading " + result.Title + "...");
            using (WebClient client = new WebClient()) {
                bytes = client.DownloadData(link);
                for (int i = 0; i < client.ResponseHeaders.Count; i++) {
                    string value = client.ResponseHeaders.Get(i);
                    string key = client.ResponseHeaders.GetKey(i);
                    if (ContentType.Equals(key))
                        contentType = value;
                    if (ContentDisposition.Equals(key) && value != null && value.StartsWith(AttachmentFileName)) {
                        fileName = value.Substring(AttachmentFileName.Length);
                        if (fileName.StartsWith("\""))
                            fileName = fileName.Substring(1);
                        if (fileName.EndsWith("\""))
                            fileName = fileName.Substring(0, fileName.Length - 1);
                    }
                }
            }
            if (fileName == null && contentType == null) {
                string format = "No file name or content type found for {0}.";
                throw new DriverException(string.Format(format, result.Title));
            }
            if (fileName == null) {
                if (contentType.IndexOf("/") == -1) {
                    string format = "Unknown content type found for {0}: {1}.";
                    throw new DriverException(string.Format(format, result.Title, contentType));
                }
                fileName = result.Title + "." + contentType.Substring(contentType.LastIndexOf("/") + 1);
            }
            result.FileName = Path.GetFileNameWithoutExtension(fileName);
            string tempName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            result.DownloadPath = Path.Combine(user.DownloadFolder, tempName);
            File.WriteAllBytes(result.DownloadPath, bytes);
        }
    }
}
