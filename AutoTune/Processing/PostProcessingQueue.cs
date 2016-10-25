using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Diagnostics;
using System.IO;
using YAXLib;

namespace AutoTune.Processing {

    [YAXComment("Do not edit!")]
    internal class PostProcessingQueue : Queue<PostProcessingQueue> {

        internal override string GetAction() {
            return "post processing";
        }

        internal override int GetThreadCount() {
            return AppSettings.Instance.PostProcessingThreadCount;
        }

        internal override void ProcessItem(QueueItem item) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            if (!File.Exists(item.DownloadPath))
                throw new FileNotFoundException(string.Format("File not found: {0}.", item.DownloadPath));
            if (app.PostProcessingEnabled) {
                string processPath = DoPostProcess(item);
                if (!File.Exists(processPath))
                    throw new FileNotFoundException(string.Format("File not found: {0}.", processPath));
                string processedPath = CopyToTarget(processPath, user.TargetFolder, item.BaseFileName);
                EmbedMetaData(item, processedPath);
                File.Delete(processPath);
            }
            if (!app.PostProcessingEnabled || app.PostProcessingKeepOriginal) {
                string originalPath = CopyToTarget(item.DownloadPath, user.TargetFolder, item.BaseFileName);
                EmbedMetaData(item, originalPath);
            }
            File.Delete(item.DownloadPath);
        }

        static void EmbedMetaData(QueueItem item, string path) {
            var app = AppSettings.Instance;
            if (app.EmbedDescriptionAfterPostProcessing && item.Search.Description != null ||
                app.EmbedThumbnailAfterPostProcessing && item.Search.ThumbnailBase64 != null)
                try {
                    using (var file = TagLib.File.Create(path)) {
                        if (app.EmbedDescriptionAfterPostProcessing && item.Search.Description != null)
                            file.Tag.Comment = item.Search.Description;
                        if (app.EmbedThumbnailAfterPostProcessing && item.Search.ThumbnailBase64 != null)
                            file.Tag.Pictures = new TagLib.Picture[] {
                                new TagLib.Picture(new TagLib.ByteVector(Convert.FromBase64String(item.Search.ThumbnailBase64)))
                            };
                        file.Save();
                    }
                } catch (Exception e) {
                    Logger.Error(e, "Failed to embed metadata in {0}.", path);
                }
        }

        static string DoPostProcess(QueueItem item) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            string name = Guid.NewGuid().ToString();
            string path = Path.Combine(user.ProcessFolder, name);
            string args = string.Format(app.PostProcessingArguments, item.DownloadPath, path, app.PostProcessingExtension);
            ProcessStartInfo info = new ProcessStartInfo(app.PostProcessingCommand, args);
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            using (var processor = Process.Start(info)) {
                processor.WaitForExit();
                if (processor.ExitCode != 0) {
                    string format = "Post processing {0} exited with code {1}.";
                    var message = string.Format(format, item.Search.Title, processor.ExitCode);
                    throw new ProcessingException(message);
                }
            }
            return path + "." + app.PostProcessingExtension;
        }

        static string CopyToTarget(string fromPath, string targetFolder, string baseFileName) {
            string fileName = baseFileName + Path.GetExtension(fromPath);
            string toPath = Path.Combine(targetFolder, fileName);
            int counter = 1;
            while (true)
                try {
                    using (FileStream from = File.Open(fromPath, FileMode.Open, FileAccess.Read))
                    using (FileStream to = File.Open(toPath, FileMode.CreateNew, FileAccess.Write))
                        from.CopyTo(to);
                    return toPath;
                } catch (IOException) {
                    if (!File.Exists(toPath))
                        throw;
                    fileName = baseFileName + " (" + counter++ + ")" + Path.GetExtension(fromPath);
                    toPath = Path.Combine(targetFolder, fileName);
                }
        }
    }
}
