using AutoTune.Drivers;
using AutoTune.Settings;
using AutoTune.Shared;

namespace AutoTune.Queue {

    public class DownloadQueue : Queue<DownloadQueue> {

        protected override string GetAction() {
            return "download";
        }

        protected override int GetThreadCount() {
            return AppSettings.Instance.DownloadThreadCount;
        }

        protected override void ProcessItem(Result result) {
            Driver.Download(result);
        }
    }
}
