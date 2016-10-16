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

        public readonly Result result;
        public string State { get { return uiState.Text; } }

        public QueueItemView(Result result) {
            this.result = result;
            InitializeComponent();
            InitializeResult();
            InitializeColors();
        }

        public void SetState(string state) {
            Invoke(new Action(() => uiState.Text = state));
        }

        void InitializeResult() {
            uiTitle.Text = result.Title;
            uiState.Text = Queued;
        }

        void InitializeColors() {
            var theme = Settings.Instance.Theme;
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
