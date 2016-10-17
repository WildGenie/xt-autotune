using AutoTune.Drivers;
using AutoTune.Settings;
using AutoTune.Shared;

namespace AutoTune.Queue {

    public class PostProcessingQueue : Queue<PostProcessingQueue> {

        protected override string GetAction() {
            return "post processing";
        }

        protected override void ProcessItem(Result result) {
            Driver.PostProcess(result);
        }

        protected override int GetThreadCount() {
            return AppSettings.Instance.PostProcessing.ThreadCount;
        }
    }
}
