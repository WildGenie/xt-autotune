using AutoTune.Shared;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class ResultView : UserControl {

        public event EventHandler<EventArgs<Result>> PlayClicked;
        public event EventHandler<EventArgs<Result>> SimilarClicked;
        public event EventHandler<EventArgs<Result>> DownloadClicked;

        Result result;
        public Result Result {
            get { return result; }
        }

        public ResultView() {
            InitializeComponent();
            InitializeColors();
            Resize += OnResize;
        }

        void InitializeColors() {
            var theme = Settings.Instance.Theme;
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            var fore2 = ColorTranslator.FromHtml(theme.ForeColor2);
            BackColor = back2;
            uiType.ForeColor = fore1;
            uiText.BackColor = back2;
            uiText.ForeColor = fore1;
            uiPlay.BackColor = back2;
            uiPlay.LinkColor = fore2;
            uiPlay.ActiveLinkColor = fore2;
            uiDownload.BackColor = back2;
            uiDownload.LinkColor = fore2;
            uiDownload.ActiveLinkColor = fore2;
            uiSimilar.BackColor = back2;
            uiSimilar.LinkColor = fore2;
            uiSimilar.ActiveLinkColor = fore2;
        }

        public void SetResult(Result result) {
            this.result = result;
            if (result?.ThumbnailUrl == null)
                InitializeResult(null);
            else
                using (WebClient client = new WebClient()) {
                    client.DownloadDataCompleted += (s, evt) => Invoke(new Action(() => InitializeResult(evt.Result)));
                    client.DownloadDataAsync(new Uri(result.ThumbnailUrl));
                }
        }

        void InitializeResult(byte[] imageData) {
            uiText.Text = "";
            uiType.Text = result == null ? "" : result.Type;
            if (imageData != null)
                using (var stream = new MemoryStream(imageData))
                    uiImage.Image = Image.FromStream(stream);
            if (result != null) {
                string text = "{\\rtf \\b " + result.Title + " \\b0 ";
                text += " \\line " + result.Description + " }";
                uiText.Rtf = text;
            }
        }

        void OnResize(object sender, EventArgs e) {
            uiText.Width = Width - uiText.Left;
        }

        void OnPlayClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                PlayClicked(this, new EventArgs<Result>(result));
        }

        void OnSimilarClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                SimilarClicked(this, new EventArgs<Result>(result));
        }

        void OnDownloadClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                DownloadClicked(this, new EventArgs<Result>(result));
        }
    }
}
