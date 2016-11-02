using AutoTune.Processing;
using AutoTune.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class QueueView : UserControl {

        IQueue queue;
        internal event EventHandler<EventArgs<QueueItem>> Play;

        public QueueView() {
            InitializeComponent();
            if (DesignMode)
                return;
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
            UiUtility.SetLinkForeColors(uiPause);
            UiUtility.SetLinkForeColors(uiClearQueue);
            UiUtility.SetLinkForeColors(uiHideCompleted);
        }

        void AddViews() {
            foreach (QueueItem item in queue.Items)
                AddView(item);
        }

        internal void Enqueue(QueueItem item) {
            queue.Enqueue(item, () => AddView(item));
        }

        void AddView(QueueItem item) {
            QueueItemView view = new QueueItemView();
            ResizeView(view);
            uiItems.Controls.Add(view);
            view.Initialize(item);
            uiItems.ScrollControlIntoView(view);
        }

        void OnPauseClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            uiPause.Text = queue.Paused ? "Pause" : "Resume";
            queue.Paused = !queue.Paused;
        }

        void OnClearQueueClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            queue.Clear();
            UiUtility.ClearContainer<QueueItemView>(uiItems);
        }

        void OnHideCompletedClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var controls = uiItems.Controls.Cast<QueueItemView>().ToList();
            foreach (var v in controls)
                if (QueueItemView.Done.Equals(v.State)) {
                    v.Dispose();
                    uiItems.Controls.Remove(v);
                }
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
                .Where(v => v.item?.Equals(item) == true)
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
