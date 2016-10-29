using AutoTune.Processing;

namespace AutoTune.Web {

    public class VideoCallbacks {

        public void Stopped() {
            Playlist.Instance.Stopped();
        }
    }
}
