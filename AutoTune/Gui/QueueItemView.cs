using AutoTune.Processing;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    internal partial class QueueItemView : UserControl {

        internal const string Done = "Done";
        internal const string Error = "Error";
        internal const string Queued = "Queued";
        internal const string Missing = "Missing";
        internal const string Started = "Started";

        internal event EventHandler<EventArgs<QueueItem>> Play;

        internal QueueItem item;
        internal string State { get { return uiState.Text; } }

        internal QueueItemView() {
            InitializeComponent();
            InitializeColors();
            uiState.Text = Queued;
        }

        internal void SetTitleWidth(int width) {
            uiTitle.Width = width;
        }

        internal void SetState(string state) {
            Invoke(new Action(() => uiState.Text = state));
        }

        internal void Initialize(QueueItem item) {
            this.item = item;
            Utility.WhenImageDownloaded(item.Search.ThumbnailUrl, i => Invoke(new Action(() => InitializeResult(i))));
        }

        void InitializeResult(Image image) {
            uiTitle.Text = item.Search.Title;
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
            Play(this, new EventArgs<QueueItem>(item));
        }
    }
}
