using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class ResultView : UserControl {

        internal event EventHandler<EventArgs<SearchResult>> PlayClicked;
        internal event EventHandler<EventArgs<SearchResult>> RelatedClicked;
        internal event EventHandler<EventArgs<SearchResult>> DownloadClicked;

        SearchResult result;
        internal SearchResult Result { get { return result; } }

        public ResultView() {
            InitializeComponent();
            if (DesignMode)
                return;
            InitializeColors();
            Resize += OnResize;
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            BackColor = back2;
            uiType.ForeColor = fore1;
            uiText.BackColor = back2;
            uiText.ForeColor = fore1;
            uiPlay.BackColor = back2;
            uiRelated.BackColor = back2;
            uiDownload.BackColor = back2;
            Utility.SetLinkForeColors(uiPlay);
            Utility.SetLinkForeColors(uiRelated);
            Utility.SetLinkForeColors(uiDownload);
        }

        void OnResize(object sender, EventArgs e) {
            uiText.Width = Width - uiText.Left;
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

        internal void SetResult(SearchResult result) {
            this.result = result;
            Action<Image> init = i => Invoke(new Action(() => InitializeResult(i)));
            Utility.WhenImageDownloaded(result?.ThumbnailUrl, init);
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
