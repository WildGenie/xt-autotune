using AutoTune.Local;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class ResultView : UserControl {

        internal event EventHandler<EventArgs<SearchResult>> PlayClicked;
        internal event EventHandler<EventArgs<SearchResult>> QueueClicked;
        internal event EventHandler<EventArgs<SearchResult>> RemoveClicked;
        internal event EventHandler<EventArgs<SearchResult>> RelatedClicked;
        internal event EventHandler<EventArgs<SearchResult>> SimilarClicked;
        internal event EventHandler<EventArgs<SearchResult>> DownloadClicked;

        bool playing;
        long clickTime;
        Rectangle clickArea;
        volatile bool firstClick = true;
        volatile bool doubleClick = false;
        internal ResultViewType Type { get; }

        private SearchResult result;
        internal SearchResult Result { get { return result; } }

        public ResultView() :
            this(ResultViewType.Search) {
        }

        public ResultView(ResultViewType type) {
            InitializeComponent();
            Type = type;
            if (DesignMode)
                return;
            InitializeColors();
            Resize += OnResize;
            switch (type) {
                case ResultViewType.Search:
                    uiTooltip.SetToolTip(uiImage, "Click to queue, double-click to play.");
                    uiTooltip.SetToolTip(uiDownload, "Send this track to the download queue.");
                    break;
                case ResultViewType.Suggestion:
                    uiTooltip.SetToolTip(uiImage, "Click to queue, double-click to play.");
                    uiTooltip.SetToolTip(uiRemove, "Decline suggestion (won't show up again).");
                    uiTooltip.SetToolTip(uiDownload, "Accept suggestion and send to the download queue.");
                    break;
                case ResultViewType.Playlist:
                    uiTooltip.SetToolTip(uiRemove, "Remove this track from the playlist.");
                    uiTooltip.SetToolTip(uiDownload, "Send this track to the download queue.");
                    uiTooltip.SetToolTip(uiImage, "Click to queue, double-click to play, drag to reorder.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (type == ResultViewType.Playlist) {
                AllowDrop = true;
                DragOver += OnDragOver;
                DragDrop += OnDragDrop;
            }
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
            UiUtility.SetLinkForeColors(uiRemove);
            UiUtility.SetLinkForeColors(uiRelated);
            UiUtility.SetLinkForeColors(uiSimilar);
            UiUtility.SetLinkForeColors(uiDownload);
            UiUtility.SetLinkForeColors(uiToggleFavourite);
        }

        void OnResize(object sender, EventArgs e) {
            uiText.Width = Width - uiText.Left;
        }

        long CurrentMillis() {
            return (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        internal void SetPlaying(bool playing) {
            this.playing = playing;
            uiType.Text = result.TypeId + (!playing ? "" : " (playing)");
        }

        internal void SetResult(SearchResult result) {
            var theme = ThemeSettings.Instance;
            this.result = result;
            uiDownload.Visible = !result?.Local ?? false;
            uiRemove.Visible = Type != ResultViewType.Search;
            uiText.Text = "";
            uiType.Text = result == null ? "" : result.TypeId + (!playing ? "" : " (playing)");
            if (result != null) {
                string text = "{\\rtf \\b " + result.Title + " \\b0 ";
                text += " \\line " + result.Description.Replace("\\", "\\\\") + " }";
                uiText.Rtf = text;
            }
            uiImage.Image = UiUtility.ImageFromBase64(result?.ThumbnailBase64 ?? AppSettings.NoImageAvailableBase64);
            bool isFavourite = Library.IsFavourite(result?.TypeId, result?.VideoId);
            SetFavouriteState(isFavourite);
        }

        void OnRemoveClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                RemoveClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnRelatedClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                RelatedClicked(this, new EventArgs<SearchResult>(result));
        }

        void OnSimilarClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (result != null)
                SimilarClicked(this, new EventArgs<SearchResult>(result));
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

        void OnImageMouseDown(object sender, MouseEventArgs e) {
            DoDragDrop(this, DragDropEffects.Move);
            if (firstClick) {
                firstClick = false;
                Interlocked.Exchange(ref clickTime, CurrentMillis());
                clickArea = new Rectangle(
                    e.X - (SystemInformation.DoubleClickSize.Width / 2),
                    e.Y - (SystemInformation.DoubleClickSize.Height / 2),
                    SystemInformation.DoubleClickSize.Width,
                    SystemInformation.DoubleClickSize.Height);
                Task.Delay(SystemInformation.DoubleClickTime).ContinueWith(_ => {
                    if (doubleClick && result != null)
                        BeginInvoke(new Action(() => PlayClicked(this, new EventArgs<SearchResult>(result))));
                    if (!doubleClick && result != null)
                        BeginInvoke(new Action(() => QueueClicked(this, new EventArgs<SearchResult>(result))));
                    firstClick = true;
                    doubleClick = false;
                });
            } else if (clickArea.Contains(e.Location) && CurrentMillis() - clickTime < SystemInformation.DoubleClickTime)
                doubleClick = true;
        }

        void SetFavouriteState(bool favourite) {
            var theme = ThemeSettings.Instance;
            var back = ColorTranslator.FromHtml(favourite ? theme.BackColor3 : theme.BackColor2);
            BackColor = back;
            uiText.BackColor = back;
            uiRemove.BackColor = back;
            uiRelated.BackColor = back;
            uiDownload.BackColor = back;
            uiToggleFavourite.BackColor = back;
            uiToggleFavourite.Text = favourite ? "Unlike" : "Like";
        }

        void OnDragOver(object sender, DragEventArgs e) {
            var other = e.Data.GetData(typeof(ResultView)) as ResultView;
            e.Effect = other == null ? DragDropEffects.None : DragDropEffects.Move;
        }

        void OnDragDrop(object sender, DragEventArgs e) {
            var other = e.Data.GetData(typeof(ResultView)) as ResultView;
            if (other == null)
                return;
            var panel = (FlowLayoutPanel)Parent;
            int index = panel.Controls.GetChildIndex(this);
            panel.Controls.SetChildIndex(other, index);
        }
    }
}
