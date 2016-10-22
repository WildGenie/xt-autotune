using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Diagnostics;
using System.IO;

namespace AutoTune.Drivers {

    static class PostProcess {

        static string DoPostProcess(Result result) {
            var user = UserSettings.Instance;
            var processing = AppSettings.Instance.PostProcessing;
            string name = Guid.NewGuid().ToString();
            string path = Path.Combine(user.ProcessFolder, name);
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
            var user = UserSettings.Instance;
            string fileName = result.FileName + Path.GetExtension(fromPath);
            string toPath = Path.Combine(user.TargetFolder, fileName);
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
                    toPath = Path.Combine(user.TargetFolder, fileName);
                }
        }

        public static void Execute(Result result) {
            var postProcessing = AppSettings.Instance.PostProcessing;
            if (!File.Exists(result.DownloadPath))
                throw new FileNotFoundException(string.Format("File not found: {0}.", result.DownloadPath));
            if (postProcessing.Enabled) {
                string processPath = DoPostProcess(result);
                if (!File.Exists(processPath))
                    throw new FileNotFoundException(string.Format("File not found: {0}.", processPath));
                CopyToTarget(result, processPath);
                File.Delete(processPath);
            }
            if (!postProcessing.Enabled || postProcessing.KeepOriginal)
                CopyToTarget(result, result.DownloadPath);
            File.Delete(result.DownloadPath);
        }
    }
}
