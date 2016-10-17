using AutoTune.Queue;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class QueueView : UserControl {

        IQueue queue;
        public event EventHandler<EventArgs<Result>> Play;

        public QueueView() {
            InitializeComponent();
            InitializeColors();
        }

        public void Initialize(IQueue queue) {
            this.queue = queue;
            AddViews();
            ConnectEventHandlers();
            uiPause.Text = queue.Paused ? "Resume" : "Pause";
        }

        void ConnectEventHandlers() {
            queue.Error += OnError;
            queue.Started += OnStarted;
            queue.NotFound += OnNotFound;
            queue.Completed += OnCompleted;
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var fore2 = ColorTranslator.FromHtml(theme.ForeColor2);
            uiPause.LinkColor = fore2;
            uiPause.ActiveLinkColor = fore2;
            uiHideFinished.LinkColor = fore2;
            uiHideFinished.ActiveLinkColor = fore2;
            uiHideCompleted.LinkColor = fore2;
            uiHideCompleted.ActiveLinkColor = fore2;
            uiClearQueue.LinkColor = fore2;
            uiClearQueue.ActiveLinkColor = fore2;
        }

        void AddViews() {
            foreach (Result result in queue.Items)
                AddView(result);
        }

        public void Enqueue(Result result) {
            queue.Enqueue(result, () => AddView(result));
        }

        void AddView(Result result) {
            QueueItemView view = new QueueItemView();
            view.Play += (s, e) => Play(this, e);
            ResizeView(view);
            uiItems.Controls.Add(view);
            view.Initialize(result);
        }

        void OnClearQueueClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            queue.Clear();
            uiItems.Controls.Clear();
        }

        void OnPauseClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            uiPause.Text = queue.Paused ? "Pause" : "Resume";
            queue.Paused = !queue.Paused;
        }

        void OnHideCompletedClick(object sender, LinkLabelLinkClickedEventArgs e) {
            var controls = uiItems.Controls.Cast<QueueItemView>().ToList();
            foreach (var v in controls)
                if (QueueItemView.Done.Equals(v.State))
                    uiItems.Controls.Remove(v);
        }

        void OnHideFinishedClick(object sender, LinkLabelLinkClickedEventArgs e) {
            var controls = uiItems.Controls.Cast<QueueItemView>().ToList();
            foreach (var v in controls)
                if (!QueueItemView.Queued.Equals(v.State) && !QueueItemView.Started.Equals(v.State))
                    uiItems.Controls.Remove(v);
        }

        void OnCompleted(object sender, EventArgs<Result> e) {
            SetState(e.Data, QueueItemView.Done);
        }

        void OnError(object sender, EventArgs<Result> e) {
            SetState(e.Data, QueueItemView.Error);
        }

        void OnNotFound(object sender, EventArgs<Result> e) {
            SetState(e.Data, QueueItemView.Missing);
        }

        void OnStarted(object sender, EventArgs<Result> e) {
            SetState(e.Data, QueueItemView.Started);
        }

        void SetState(Result r, string state) {
            var views = FindViews(r);
            foreach (var v in views)
                v.SetState(state);
        }

        List<QueueItemView> FindViews(Result r) {
            return uiItems.Controls
                .Cast<QueueItemView>()
                .Where(v => v.result.Equals(r))
                .ToList();
        }

        void OnResized(object sender, EventArgs e) {
            foreach (QueueItemView v in uiItems.Controls)
                ResizeView(v);
        }

        void ResizeView(QueueItemView v) {
            v.Width = Width - 30;
        }
    }
}
