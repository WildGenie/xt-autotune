using System;
using AutoTune.Drivers;
using AutoTune.Shared;

namespace AutoTune.Queue {

    public class DownloadQueue : Queue<DownloadQueue> {

        private const string FileName = "downloads.xml";

        public static void Terminate() {
            Terminate(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
        }

        protected override string GetAction() {
            return "download";
        }

        protected override int GetThreadCount() {
            return Settings.Instance.General.DownloadThreads;
        }

        protected override void ProcessItem(Result result) {
            Driver.Download(result);
        }
    }
}
