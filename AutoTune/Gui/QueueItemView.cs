using AutoTune.Processing;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class QueueItemView : UserControl {

        internal const string Done = "Done";
        internal const string Error = "Error";
        internal const string Queued = "Queued";
        internal const string Missing = "Missing";
        internal const string Started = "Started";

        internal event EventHandler<EventArgs<QueueItem>> Play;

        internal QueueItem item;
        internal string State { get { return uiState.Text; } }

        public QueueItemView() {
            InitializeComponent();
            if (DesignMode)
                return;
            InitializeColors();
            uiState.Text = Queued;
        }

        void InitializeResult(Image image) {
            uiTitle.Text = item.Search.Title;
            uiImage.Image = image;
        }

        internal void SetTitleWidth(int width) {
            uiTitle.Width = width;
        }

        internal void SetState(string state) {
            Invoke(new Action(() => uiState.Text = state));
        }

        void InitializeColors() {
            Utility.SetLinkForeColors(uiPlay);
            var theme = ThemeSettings.Instance;
            BackColor = ColorTranslator.FromHtml(theme.BackColor2);
            uiTitle.ForeColor = ColorTranslator.FromHtml(theme.ForeColor1);
            uiState.ForeColor = ColorTranslator.FromHtml(theme.ForeColor1);
        }

        void OnPlayClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Play(this, new EventArgs<QueueItem>(item));
        }

        internal void Initialize(QueueItem item) {
            this.item = item;
            Action<Image> init = i => Invoke(new Action(() => InitializeResult(i)));
            Utility.WhenImageDownloaded(item.Search.ThumbnailUrl, init);
        }
    }
}
