using AutoTune.Local;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class ResultView : UserControl {

        internal event EventHandler<EventArgs<SearchResult>> PlayClicked;
        internal event EventHandler<EventArgs<SearchResult>> QueueClicked;
        internal event EventHandler<EventArgs<SearchResult>> RemoveClicked;
        internal event EventHandler<EventArgs<SearchResult>> RelatedClicked;
        internal event EventHandler<EventArgs<SearchResult>> DownloadClicked;

        bool playing;
        private readonly bool playlist;
        private SearchResult result;
        internal SearchResult Result { get { return result; } }

        public ResultView() :
            this(false) {
        }

        public ResultView(bool playlist) {
            InitializeComponent();
            if (DesignMode)
                return;
            InitializeColors();
            Resize += OnResize;
            this.playlist = playlist;
        }

        internal void Reload() {
            SetResult(Result);
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            uiType.ForeColor = fore1;
            uiText.ForeColor = fore1;
            SetFavouriteState(false);
            Utility.SetLinkForeColors(uiPlay);
            Utility.SetLinkForeColors(uiQueue);
            Utility.SetLinkForeColors(uiRemove);
            Utility.SetLinkForeColors(uiRelated);
            Utility.SetLinkForeColors(uiDownload);
            Utility.SetLinkForeColors(uiToggleFavourite);
        }

        void OnResize(object sender, EventArgs e) {
            uiText.Width = Width - uiText.Left;
        }

        internal void SetPlaying(bool playing) {
            this.playing = playing;
            uiType.Text = result.TypeId + (!playing ? "" : " (playing)");
        }

        internal void SetResult(SearchResult result) {
            var theme = ThemeSettings.Instance;
            uiRemove.Visible = playlist;
            uiQueue.Visible = !playlist;
            uiRelated.Visible = !playlist;
            uiDownload.Visible = !playlist && (!result?.Local ?? false);
            this.result = result;
            uiText.Text = "";
            uiType.Text = result == null ? "" : result.TypeId + (!playing ? "" : " (playing)");
            if (result != null) {
                string text = "{\\rtf \\b " + result.Title + " \\b0 ";
                text += " \\line " + result.Description + " }";
                uiText.Rtf = text;
            }
            uiImage.Image = Utility.ImageFromBase64(result?.ThumbnailBase64 ?? AppSettings.NoImageAvailableBase64);
            bool isFavourite = Library.IsFavourite(result?.TypeId, result?.VideoId);
            SetFavouriteState(isFavourite);
        }


        void OnImageMouseClick(object sender, MouseEventArgs e) {
            if (result != null)
                if (e.Button == MouseButtons.Left)
                    PlayClicked(this, new EventArgs<SearchResult>(result));
                else if (e.Button == MouseButtons.Right)
                    DownloadClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnQueueClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                QueueClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnRemoveClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                RemoveClicked(this, new EventArgs<SearchResult>(result));
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

        void OnToggleFavouriteClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            bool isFavourite = Library.IsFavourite(result?.TypeId, result?.VideoId);
            Library.SetFavourite(result?.TypeId, result?.VideoId, !isFavourite);
            SetFavouriteState(!isFavourite);
        }

        void SetFavouriteState(bool favourite) {
            var theme = ThemeSettings.Instance;
            var back = ColorTranslator.FromHtml(favourite ? theme.BackColor3 : theme.BackColor2);
            BackColor = back;
            uiPlay.BackColor = back;
            uiText.BackColor = back;
            uiQueue.BackColor = back;
            uiRelated.BackColor = back;
            uiDownload.BackColor = back;
            uiToggleFavourite.BackColor = back;
            uiToggleFavourite.Text = favourite ? "Unfavourite" : "Favourite";
        }
    }
}
