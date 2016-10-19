using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class QueueItemView : UserControl {

        public const string Done = "Done";
        public const string Error = "Error";
        public const string Queued = "Queued";
        public const string Missing = "Missing";
        public const string Started = "Started";

        public event EventHandler<EventArgs<Result>> Play;

        public Result result;
        public string State { get { return uiState.Text; } }

        public QueueItemView() {
            InitializeComponent();
            InitializeColors();
            uiState.Text = Queued;
        }

        internal void SetTitleWidth(int width) {
            uiTitle.Width = width;
        }

        public void SetState(string state) {
            Invoke(new Action(() => uiState.Text = state));
        }

        public void Initialize(Result result) {
            this.result = result;
            Utility.WhenImageDownloaded(result?.ThumbnailUrl, i => Invoke(new Action(() => InitializeResult(i))));
        }

        void InitializeResult(Image image) {
            uiTitle.Text = result.Title;
            uiImage.Image = image;
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            var fore2 = ColorTranslator.FromHtml(theme.ForeColor2);
            BackColor = back2;
            uiTitle.ForeColor = fore1;
            uiState.ForeColor = fore1;
            uiPlay.LinkColor = fore2;
            uiPlay.ActiveLinkColor = fore2;
        }

        void OnPlayClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Play(this, new EventArgs<Result>(result));
        }
    }
}
