using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace AutoTune.Drivers {

    static class Driver {

        const string AppTimeout = "app-timeout";
        const string ScriptError = "script-error";
        const string ContentType = "Content-Type";
        const string ScriptTimeout = "script-timeout";
        const string ContentDisposition = "Content-Disposition";
        const string AttachmentFileName = "attachment; filename=";
        static readonly Dictionary<string, string> ExtensionsByContentType = new Dictionary<string, string>();

        static string DoPostProcess(Result result) {
            var paths = LocalPaths.Instance;
            var processing = Settings.Instance.PostProcessing;
            string name = Guid.NewGuid().ToString();
            string path = Path.Combine(paths.ProcessFolder, name);
            string args = string.Format(processing.Arguments, result.DownloadPath, path, processing.Extension);
            ProcessStartInfo info = new ProcessStartInfo(processing.Command, args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            using (var processor = Process.Start(info)) {
                processor.WaitForExit();
                if (processor.ExitCode != 0) {
                    string format = "Post processing {0} exited with code {1}.";
                    var message = string.Format(format, result.Title, processor.ExitCode);
                    throw new DriverException(message);
                }
            }
            return path + "." + processing.Extension;
        }

        static void CopyToTarget(Result result, string fromPath) {
            var paths = LocalPaths.Instance;
            string fileName = result.FileName + Path.GetExtension(fromPath);
            string toPath = Path.Combine(paths.TargetFolder, fileName);
            int counter = 1;
            while (true)
                try {
                    using (FileStream from = File.Open(fromPath, FileMode.Open, FileAccess.Read))
                    using (FileStream to = File.Open(toPath, FileMode.CreateNew, FileAccess.Write))
                        from.CopyTo(to);
                    return;
                } catch (IOException) {
                    if (!File.Exists(toPath))
                        throw;
                    fileName = result.FileName + " (" + counter++ + ")" + Path.GetExtension(fromPath);
                    toPath = Path.Combine(paths.TargetFolder, fileName);
                }
        }

        public static void PostProcess(Result result) {
            if (!File.Exists(result.DownloadPath))
                throw new FileNotFoundException(string.Format("File not found: {0}.", result.DownloadPath));
            if (result.ShouldPostProcess) {
                string processPath = DoPostProcess(result);
                if (!File.Exists(processPath))
                    throw new FileNotFoundException(string.Format("File not found: {0}.", processPath));
                CopyToTarget(result, processPath);
                File.Delete(processPath);
            }
            if (!result.ShouldPostProcess || result.KeepOriginal)
                CopyToTarget(result, result.DownloadPath);
            File.Delete(result.DownloadPath);
        }

        public static void Download(Result result) {
            var paths = LocalPaths.Instance;
            var general = Settings.Instance.General;
            string delimiter = general.ExtractorDelimiter;
            string args = string.Format("\"{0}\" {1} {2} {3}", paths.ExtractorFilePath, result.DownloadUrl, delimiter, general.ExtractorTimeout);
            ProcessStartInfo info = new ProcessStartInfo(paths.ExtractorExecutablePath, args);
            string link = null;
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            using (Process extract = Process.Start(info)) {
                extract.WaitForExit();
                string output = extract.StandardOutput.ReadToEnd();
                link = output.Substring(output.IndexOf(delimiter) + delimiter.Length);
                link = link.Substring(0, link.IndexOf(delimiter));
                if (extract.ExitCode != 0) {
                    Logger.Debug("Extraction error: console output: {0}.", output);
                    string format = "Extraction of {0} returned with code: {1}.";
                    throw new DriverException(string.Format(format, result.Title, link));
                }
            }
            byte[] bytes;
            string fileName = null;
            string contentType = null;
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
            result.DownloadPath = Path.Combine(paths.DownloadFolder, tempName);
            File.WriteAllBytes(result.DownloadPath, bytes);
        }
    }
}
