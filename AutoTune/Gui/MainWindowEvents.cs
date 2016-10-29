using AutoTune.Local;
using System.Linq;
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
            lock (shutdownLock) {
                if (shutdown)
                    return;
                shutdown = true;
            }
            Cef.Shutdown();
            logger.Flush();
            logger.Dispose();
            Scanner.Terminate();
            Playlist.Terminate();
            UiSettings.Terminate();
            AppSettings.Terminate();
            ThemeSettings.Terminate();
            DownloadQueue.Terminate();
            PostProcessingQueue.Terminate();
        }

        void OnMainWindowShown(object sender, EventArgs e) {
            if (DesignMode)
                return;
            var app = AppSettings.Instance;
            InitializeLog();
            SearchEngine.Initialize(app.Providers.Keys.Where(k => app.Providers[k].Enabled));
            Library.Initialize(AppSettings.GetFolderPath());
            InitializeSettings();
            Playlist.Initialize();
            DownloadQueue.Initialize();
            PostProcessingQueue.Initialize();
            uiDownloadQueue.Initialize(DownloadQueue.Instance);
            uiPostProcessingQueue.Initialize(PostProcessingQueue.Instance);
            Action<QueueItem> enqueue = r => uiPostProcessingQueue.Enqueue(r.NewId());
            DownloadQueue.Instance.Completed += (s, evt) => BeginInvoke(new Action(() => enqueue(evt.Data)));
            if (app.UpdateLibraryAfterDownload)
                PostProcessingQueue.Instance.Completed += (s, evt) => Scanner.UpdateLibrary();
            uiCurrentResult.SetResult(UiSettings.Instance.CurrentTrack);
            StartSearch();
            DownloadQueue.Start();
            PostProcessingQueue.Start();
            InitializePlaylist();
            Scanner.Start(UserSettings.Instance.LibraryFolder, app.TagSeparator, app.ScanLibraryInterval);
            initializing = false;
        }

        void InitializePlaylist() {
            Playlist.Instance.Next += OnPlaylistNext;
            uiPlaylistModeAll.Checked = Playlist.Instance.Mode == PlaylistMode.RepeatAll;
            uiPlaylistModeRandom.Checked = Playlist.Instance.Mode == PlaylistMode.Random;
            uiPlaylistModeTrack.Checked = Playlist.Instance.Mode == PlaylistMode.RepeatTrack;
            foreach (var item in Playlist.Instance.Items)
                AddPlaylistView(item);
        }

        void OnPlaylistNext(object sender, EventArgs<SearchResult> e) {
            BeginInvoke(new Action(() => {
                LoadResult(e.Data, true);
                SetPlaylistPlaying(e.Data);
            }));
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

        void OnLeftTabsSelectedIndexChanged(object sender, EventArgs e) {
            try {
                SuspendLayout();
                if (uiLeftTabs.SelectedIndex == 0)
                    foreach (ResultView view in uiResults.Controls)
                        view.Reload();
                if (uiLeftTabs.SelectedIndex == 1)
                    foreach (ResultView view in uiPlaylist.Controls)
                        view.Reload();
            } finally {
                ResumeLayout();
            }
        }

        void OnPlaylistClearClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.Clear();
            uiPlaylist.Controls.Clear();
        }

        void OnPlaylistStartClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.Start();
        }

        void OnPlaylistNextClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.PlayNext();
        }

        void OnPlaylistStopClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            LoadResult(null, false);
            SetPlaylistPlaying(null);
            Playlist.Instance.Stop();
        }

        void OnPlaylistModeAllCheckedChanged(object sender, EventArgs e) {
            if (uiPlaylistModeAll.Checked)
                Playlist.Instance.Mode = PlaylistMode.RepeatAll;
        }

        void OnPlaylistModeTrackCheckedChanged(object sender, EventArgs e) {
            if (uiPlaylistModeTrack.Checked)
                Playlist.Instance.Mode = PlaylistMode.RepeatTrack;
        }

        void OnPlaylistModeRandomCheckedChanged(object sender, EventArgs e) {
            if (uiPlaylistModeRandom.Checked)
                Playlist.Instance.Mode = PlaylistMode.Random;
        }

        void OnResultPlayClicked(object sender, EventArgs<SearchResult> e) {
            PlayResult(e.Data);
        }

        void OnResultQueueClicked(object sender, EventArgs<SearchResult> e) {
            AddToPlaylist(e.Data);
        }

        void OnResultDownloadClicked(object sender, EventArgs<SearchResult> e) {
            uiDownloadQueue.Enqueue(new QueueItem(e.Data));
        }

        void OnLoadMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (searchQuery != null || searchRelated != null)
                LoadMoreResults();
        }

        void OnLogLevelSelectionChanged(object sender, EventArgs e) {
            if (!initializing)
                UiSettings.Instance.TraceLevel = (LogLevel)uiLogLevel.SelectedItem;
        }

        void OnResultsScroll(object sender, ScrollEventArgs e) {
            var autoLoad = AppSettings.Instance.LoadMoreResultsOnScrollToEnd;
            if (searchState == null || appendingResult || !autoLoad)
                return;
            VScrollProperties properties = uiResults.VerticalScroll;
            if (e.NewValue != properties.Maximum - properties.LargeChange + 1)
                return;
            LoadMoreResults();
        }

        void OnResultRelatedClicked(object sender, EventArgs<SearchResult> e) {
            searchQuery = null;
            searchRelated = e.Data;
            uiResults.Controls.Clear();
            SearchCredentials searchCredentials;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            credentials.TryGetValue(e.Data.TypeId, out searchCredentials);
            var query = new SearchQuery(e.Data.TypeId, searchCredentials, e.Data.VideoId, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void OnToggleLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (Width < ShowLogMinWidth)
                return;
            UiSettings.Instance.LogCollapsed = !UiSettings.Instance.LogCollapsed;
            ToggleLog(UiSettings.Instance.LogCollapsed);
        }

        void OnSearchFavouriteOnlyCheckedChanged(object sender, EventArgs e) {
            UiSettings.Instance.SearchFavouritesOnly = uiSearchFavouriteOnly.Checked;
            StartSearch();
        }

        void OnSearchLocalOnlyCheckedChanged(object sender, EventArgs e) {
            UiSettings.Instance.SearchLocalOnly = uiSearchLocalOnly.Checked;
            StartSearch();
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
