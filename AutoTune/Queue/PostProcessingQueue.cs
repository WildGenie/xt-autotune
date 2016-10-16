using AutoTune.Drivers;
using AutoTune.Shared;

namespace AutoTune.Queue {

    public class PostProcessingQueue : Queue<PostProcessingQueue> {

        private const string FileName = "processing.xml";

        public static void Terminate() {
            Terminate(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
        }

        protected override string GetAction() {
            return "post processing";
        }

        protected override int GetThreadCount() {
            return Settings.Instance.PostProcessing.Threads;
        }

        protected override void ProcessItem(Result result) {
            Driver.PostProcess(result);
        }
    }
}
