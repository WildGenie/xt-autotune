using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    internal partial class ResultView : UserControl {

        internal event EventHandler<EventArgs<SearchResult>> PlayClicked;
        internal event EventHandler<EventArgs<SearchResult>> RelatedClicked;
        internal event EventHandler<EventArgs<SearchResult>> DownloadClicked;

        SearchResult result;
        internal SearchResult Result { get { return result; } }

        internal ResultView() {
            InitializeComponent();
            InitializeColors();
            Resize += OnResize;
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
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
            uiRelated.BackColor = back2;
            uiRelated.LinkColor = fore2;
            uiRelated.ActiveLinkColor = fore2;
        }

        public void SetResult(SearchResult result) {
            this.result = result;
            Utility.WhenImageDownloaded(result?.ThumbnailUrl, i => Invoke(new Action(() => InitializeResult(i))));
        }

        void InitializeResult(Image image) {
            uiText.Text = "";
            uiImage.Image = image;
            uiType.Text = result == null ? "" : result.TypeId;
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
                PlayClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnRelatedClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                RelatedClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnDownloadClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                DownloadClicked(this, new EventArgs<SearchResult>(result));
        }
    }
}
