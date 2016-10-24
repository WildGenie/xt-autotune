using AutoTune.Local;
using AutoTune.Processing;
using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using CefSharp;
using System;
using System.Windows.Forms;

namespace AutoTune.Gui {

    partial class MainWindow : Form {

        void OnMainWindowClosed(object sender, FormClosedEventArgs e) {
            Cef.Shutdown();
            logger.Flush();
            logger.Dispose();
            Scanner.Terminate();
            UiSettings.Terminate();
            AppSettings.Terminate();
            UserSettings.Terminate();
            ThemeSettings.Terminate();
            DownloadQueue.Terminate();
            PostProcessingQueue.Terminate();
        }

        void OnMainWindowShown(object sender, EventArgs e) {
            if (DesignMode)
                return;
            InitializeLog();
            DownloadQueue.Initialize();
            PostProcessingQueue.Initialize();
            Database.Initialize(AppSettings.GetFolderPath());
            uiDownloadQueue.Initialize(DownloadQueue.Instance);
            uiPostProcessingQueue.Initialize(PostProcessingQueue.Instance);
            Action<QueueItem> enqueue = r => uiPostProcessingQueue.Enqueue(r.NewId());
            DownloadQueue.Instance.Completed += (s, evt) => Invoke(new Action(() => enqueue(evt.Data)));
            DownloadQueue.Start();
            PostProcessingQueue.Start();
            Scanner.Start(UserSettings.Instance.LibraryFolder);
            StartSearch();
            uiCurrentResult.SetResult(UiSettings.Instance.CurrentTrack);
        }

        void OnMainWindowResized(object sender, EventArgs e) {
            ToggleLog(UiSettings.Instance.LogCollapsed);
            uiToggleLog.Visible = Width >= ShowLogMinWidth;
        }

        void OnQueryKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char)Keys.Return)
                return;
            StartSearch();
        }

        void OnResultPlayClicked(object sender, EventArgs<SearchResult> e) {
            PlayResult(e.Data);
            UiSettings.Instance.CurrentTrack = e.Data;
            uiCurrentResult.SetResult(e.Data);
        }

        void OnResultDownloadClicked(object sender, EventArgs<SearchResult> e) {
            uiDownloadQueue.Enqueue(new QueueItem(e.Data));
        }

        void OnLoadMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (searchQuery != null || searchRelated != null)
                LoadMore();
        }

        void OnLogLevelSelectionChanged(object sender, EventArgs e) {
            if (!initializing)
                UiSettings.Instance.TraceLevel = (LogLevel)uiLogLevel.SelectedItem;
        }

        void OnResultsScroll(object sender, ScrollEventArgs e) {
            var autoLoad = AppSettings.Instance.LoadMoreResultsOnScrollEnd;
            if (searchState == null || appendingResult || !autoLoad)
                return;
            VScrollProperties properties = uiResults.VerticalScroll;
            if (e.NewValue != properties.Maximum - properties.LargeChange + 1)
                return;
            LoadMore();
        }

        void OnResultRelatedClicked(object sender, EventArgs<SearchResult> e) {
            searchQuery = null;
            searchRelated = e.Data;
            uiResults.Controls.Clear();
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            var query = new SearchQuery(e.Data.TypeId, credentials[e.Data.TypeId], e.Data.VideoId, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void OnToggleLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (Width < ShowLogMinWidth)
                return;
            UiSettings.Instance.LogCollapsed = !UiSettings.Instance.LogCollapsed;
            ToggleLog(UiSettings.Instance.LogCollapsed);
        }

        void OnToggleSearchClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleSearch(!UiSettings.Instance.SearchCollapsed);
            UiSettings.Instance.SearchCollapsed = !UiSettings.Instance.SearchCollapsed;
        }

        void OnTogglePlayerFullClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var ui = UiSettings.Instance;
            ui.PlayerFull = !ui.PlayerFull;
            TogglePlayerFull(ui.PlayerFull);
        }

        void OnToggleFullScreenClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            UiSettings.Instance.FullScreen = !UiSettings.Instance.FullScreen;
            ToggleFullScreen(UiSettings.Instance.FullScreen);
        }

        void OnToggleNotificationsClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleNotifications(!UiSettings.Instance.NotificationsCollapsed);
            UiSettings.Instance.NotificationsCollapsed = uiSplitNotifications.Panel2Collapsed;
        }

        void OnToggleCurrentControlsClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleCurrentControls(!UiSettings.Instance.CurrentControlsCollapsed);
            UiSettings.Instance.CurrentControlsCollapsed = uiSplitBrowserCurrentControls.Panel1Collapsed;
        }
    }
}
