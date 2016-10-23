using AutoTune.Settings;
using System;
using System.Diagnostics;
using System.IO;

namespace AutoTune.Processing {

    public class PostProcessingQueue : Queue<PostProcessingQueue> {

        internal override string GetAction() {
            return "post processing";
        }

        internal override int GetThreadCount() {
            return AppSettings.Instance.PostProcessingThreadCount;
        }

        internal override void ProcessItem(QueueItem item) {
            var user = UserSettings.Instance;
            var process = AppSettings.Instance.PostProcessing;
            if (!File.Exists(item.DownloadPath))
                throw new FileNotFoundException(string.Format("File not found: {0}.", item.DownloadPath));
            if (process.Enabled) {
                string processPath = DoPostProcess(item);
                if (!File.Exists(processPath))
                    throw new FileNotFoundException(string.Format("File not found: {0}.", processPath));
                CopyToTarget(processPath, user.TargetFolder, item.BaseFileName);
                File.Delete(processPath);
            }
            if (!process.Enabled || process.KeepOriginal)
                CopyToTarget(item.DownloadPath, user.TargetFolder, item.BaseFileName);
            File.Delete(item.DownloadPath);
        }

        static string DoPostProcess(QueueItem item) {
            var user = UserSettings.Instance;
            string name = Guid.NewGuid().ToString();
            var process = AppSettings.Instance.PostProcessing;
            string path = Path.Combine(user.ProcessFolder, name);
            string args = string.Format(process.Arguments, item.DownloadPath, path, process.Extension);
            ProcessStartInfo info = new ProcessStartInfo(process.Command, args);
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
            return path + "." + process.Extension;
        }

        static void CopyToTarget(string fromPath, string targetFolder, string baseFileName) {
            string fileName = baseFileName + Path.GetExtension(fromPath);
            string toPath = Path.Combine(targetFolder, fileName);
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
                    fileName = baseFileName + " (" + counter++ + ")" + Path.GetExtension(fromPath);
                    toPath = Path.Combine(targetFolder, fileName);
                }
        }
    }
}
