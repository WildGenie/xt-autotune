using AutoTune.Processing;
using AutoTune.Settings;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AutoTune.Gui {

    internal partial class QueueView : UserControl {

        IQueue queue;
        internal event EventHandler<EventArgs<QueueItem>> Play;

        internal QueueView() {
            InitializeComponent();
            InitializeColors();
        }

        internal void Initialize(IQueue queue) {
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
            uiHideCompleted.LinkColor = fore2;
            uiHideCompleted.ActiveLinkColor = fore2;
            uiClearQueue.LinkColor = fore2;
            uiClearQueue.ActiveLinkColor = fore2;
        }

        void AddViews() {
            foreach (QueueItem item in queue.Items)
                AddView(item);
        }

        public void Enqueue(QueueItem item) {
            queue.Enqueue(item, () => AddView(item));
        }

        void AddView(QueueItem item) {
            QueueItemView view = new QueueItemView();
            view.Play += (s, e) => Play(this, e);
            ResizeView(view);
            uiItems.Controls.Add(view);
            view.Initialize(item);
            uiItems.ScrollControlIntoView(view);
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

        void OnCompleted(object sender, EventArgs<QueueItem> e) {
            SetState(e.Data, QueueItemView.Done);
        }

        void OnError(object sender, EventArgs<QueueItem> e) {
            SetState(e.Data, QueueItemView.Error);
        }

        void OnNotFound(object sender, EventArgs<QueueItem> e) {
            SetState(e.Data, QueueItemView.Missing);
        }

        void OnStarted(object sender, EventArgs<QueueItem> e) {
            SetState(e.Data, QueueItemView.Started);
        }

        void SetState(QueueItem item, string state) {
            var views = FindViews(item);
            foreach (var v in views)
                v.SetState(state);
        }

        List<QueueItemView> FindViews(QueueItem item) {
            return uiItems.Controls
                .Cast<QueueItemView>()
                .Where(v => v.item.Equals(item))
                .ToList();
        }

        void OnResized(object sender, EventArgs e) {
            foreach (QueueItemView v in uiItems.Controls)
                ResizeView(v);
        }

        void ResizeView(QueueItemView v) {
            v.Width = Width - 30;
            v.SetTitleWidth(Width - 80);
        }
    }
}
