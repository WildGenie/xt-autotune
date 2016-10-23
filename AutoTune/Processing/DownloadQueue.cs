﻿using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace AutoTune.Processing {

    public class DownloadQueue : Queue<DownloadQueue> {

        const string AppTimeout = "app-timeout";
        const string ScriptError = "script-error";
        const string ContentType = "Content-Type";
        const string ScriptTimeout = "script-timeout";
        const string ContentDisposition = "Content-Disposition";
        const string AttachmentFileName = "attachment; filename=";
        static readonly Dictionary<string, string> ExtensionsByContentType = new Dictionary<string, string>();

        internal override string GetAction() {
            return "download";
        }

        internal override int GetThreadCount() {
            return AppSettings.Instance.DownloadThreadCount;
        }

        internal override void ProcessItem(QueueItem item) {
            Download(item, FetchDownloadLink(item.Search));
        }

        static void ValidateFileName(string title, string fileName, string contentType) {
            if (fileName == null && contentType == null) {
                string format = "No file name or content type found for {0}.";
                throw new ProcessingException(string.Format(format, title));
            }
            if (fileName == null && contentType.IndexOf("/") == -1) {
                string format = "Unknown content type found for {0}: {1}.";
                throw new ProcessingException(string.Format(format, title, contentType));
            }
        }

        static void Download(QueueItem item, string link) {

            byte[] data = null;
            string fileName = null;
            string contentType = null;
            Logger.Debug("Downloading {0} ({1}) ...", item.Search.Title, link);
            using (WebClient client = new WebClient()) {
                data = client.DownloadData(link);
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

            ValidateFileName(item.Search.Title, fileName, contentType);
            if (fileName == null) 
                fileName = item.Search.Title + "." + contentType.Substring(contentType.LastIndexOf("/") + 1);
            item.BaseFileName = Path.GetFileNameWithoutExtension(fileName);
            string tempName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
            item.DownloadPath = Path.Combine(UserSettings.Instance.DownloadFolder, tempName);
            File.WriteAllBytes(item.DownloadPath, data);
        }

        static string FetchDownloadLink(SearchResult item) {

            string delimiter = Guid.NewGuid().ToString();
            var fetch = AppSettings.Instance.Fetch;
            var provider = AppSettings.GetProvider(item.TypeId);
            var executable = Path.Combine("Fetch", "AutoTune.Fetch.exe");
            string fetchFilePath = Path.Combine(AppSettings.GetFolderPath(), provider.FetchFile);
            string url = string.Format(provider.DownloadUrlPattern, item.VideoId);
            string args = string.Format("\"{0}\" {1} {2} {3} {4} {5}", fetchFilePath, url, delimiter, fetch.Timeout, fetch.Delay, fetch.Retries);

            string link = null;
            ProcessStartInfo info = new ProcessStartInfo(executable, args);
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            Logger.Debug("Fetching {0} ({1}) ...", item.Title, url);
            using (Process process = Process.Start(info)) {
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                link = output.Substring(output.IndexOf(delimiter) + delimiter.Length);
                link = link.Substring(0, link.IndexOf(delimiter));
                if (process.ExitCode != 0) {
                    Logger.Debug("Fetch error: console output: {0}.", output);
                    string format = "Fetching {0} returned with code: {1}.";
                    throw new ProcessingException(string.Format(format, url, link));
                }
                return link;
            }
        }
    }
}
