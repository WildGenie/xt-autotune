using AutoTune.Shared;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class ResultView : UserControl {

        public event EventHandler<EventArgs<Result>> PlayClicked;
        public event EventHandler<EventArgs<Result>> SimilarClicked;
        public event EventHandler<EventArgs<Result>> DownloadClicked;

        readonly Result result;

        public ResultView(Result result, byte[] imageData) {
            this.result = result;
            InitializeComponent();
            InitializeResult(imageData);
            InitializeColors();
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

        void InitializeResult(byte[] imageData) {
            uiType.Text = result.Type;
            if (imageData != null)
                using (var stream = new MemoryStream(imageData))
                    uiImage.Image = Image.FromStream(stream);
            string text = "{\\rtf \\b " + result.Title + " \\b0 ";
            text += " \\line " + result.Description + " }";
            uiText.Rtf = text;
        }

        void OnPlayClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            PlayClicked(this, new EventArgs<Result>(result));
        }

        void OnSimilarClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            SimilarClicked(this, new EventArgs<Result>(result));
        }

        void OnDownloadClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            DownloadClicked(this, new EventArgs<Result>(result));
        }
    }
}
