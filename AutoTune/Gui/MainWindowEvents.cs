using AutoTune.Local;
using System.Linq;
using AutoTune.Processing;
using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using CefSharp;
using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Threading;

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
            string[] stopList = File.ReadAllLines(AppSettings.StopListPath)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();
            Library.Initialize(AppSettings.GetFolderPath(), stopList);
            InitializeSettings();
            Playlist.Initialize();
            DownloadQueue.Initialize();
            PostProcessingQueue.Initialize();
            uiDownloadQueue.Initialize(DownloadQueue.Instance);
            uiPostProcessingQueue.Initialize(PostProcessingQueue.Instance);
            Action<QueueItem> enqueue = r => uiPostProcessingQueue.Enqueue(r.NewId());
            DownloadQueue.Instance.Completed += (s, evt) => BeginInvoke(new Action(() => enqueue(evt.Data)));
            if (app.UpdateLibraryAfterDownload)
                PostProcessingQueue.Instance.Completed += (s, evt) => LibraryScanner.UpdateLibrary();
            uiCurrentResult.SetResult(UiSettings.Instance.CurrentTrack);
            StartSearch();
            DownloadQueue.Start();
            PostProcessingQueue.Start();
            InitializePlaylist();
            InitializeSuggestions();
            SuggestionScanner.Suggested += OnScannerSuggestions;
            SuggestionScanner.Start(app.DelaySuggestionsScan, app.ScanSuggestionsInterval);
            LibraryScanner.Start(UserSettings.Instance.LibraryFolder, app.TagSeparator, app.DelayLibraryScan, app.ScanLibraryInterval);
            ShowScrollBar(uiResults.Handle, SbVert, true);
            ShowScrollBar(uiPlaylist.Handle, SbVert, true);
            initializing = false;
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
            this.WithLayoutSuspended(() => {
                ShowScrollBar(uiResults.Handle, SbVert, true);
                ShowScrollBar(uiPlaylist.Handle, SbVert, true);
                ShowScrollBar(uiSuggestions.Handle, SbVert, true);
                if (uiLeftTabs.SelectedIndex == TabIndexSearch)
                    foreach (ResultView view in uiResults.Controls)
                        view.Reload();
                if (uiLeftTabs.SelectedIndex == TabIndexPlaylist)
                    foreach (ResultView view in uiPlaylist.Controls)
                        view.Reload();
                if (uiLeftTabs.SelectedIndex == TabIndexSuggestions)
                    foreach (ResultView view in uiSuggestions.Controls)
                        view.Reload();
            });
        }

        void OnPlaylistClearClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.Clear();
            UiUtility.ClearContainer<ResultView>(uiPlaylist);
        }

        void OnPlaylistNextClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.PlayNext();
        }

        void OnPlaylistPreviousClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.PlayPrevious();
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

        void OnSearchUpdateLibraryClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            LibraryScanner.UpdateLibrary();
        }

        void OnResultPlayClicked(object sender, EventArgs<SearchResult> e) {
            PlayResult(e.Data);
        }

        void OnResultQueueClicked(object sender, EventArgs<SearchResult> e) {
            AddToPlaylist(e.Data);
        }

        void OnSearchReplacePlaylistClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ReplacePlaylist(uiResults);
        }

        void OnSuggestionsReplacePlayistClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ReplacePlaylist(uiSuggestions);
        }

        void OnSuggestionsClearHistoryClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            UiUtility.ClearContainer<ResultView>(uiSuggestions);
            Library.ForgetSuggestions();
        }

        void OnSuggestionsDownloadAllClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            this.WithLayoutSuspended(() => {
                foreach (var view in uiSuggestions.Controls.Cast<ResultView>().ToList())
                    AcceptSuggestion(view);
            });
        }

        void OnSuggestionsRemoveAllClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            this.WithLayoutSuspended(() => {
                foreach (var view in uiSuggestions.Controls.Cast<ResultView>().ToList())
                    DeclineSuggestion(view);
            });
        }

        void OnResultDownloadClicked(object sender, EventArgs<SearchResult> e) {
            DownloadResult(e.Data);
        }

        void OnLoadMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (searchQuery != null || searchRelated != null)
                LoadMoreResults();
        }

        void OnLogLevelSelectionChanged(object sender, EventArgs e) {
            if (!initializing)
                UiSettings.Instance.TraceLevel = (LogLevel)uiLogLevel.SelectedItem;
        }

        void OnScannerSuggestions(object sender, EventArgs<SearchResponse> e) {
            HandleSuggestions(e.Data);
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

        void OnSuggestionsIgnoreAllClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Library.ClearOpenSuggestions();
            UiUtility.ClearContainer<ResultView>(uiSuggestions);
        }

        void OnSuggestionsSearchMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            SuggestionScanner.UpdateSuggestions(uiSuggestionsFromLocation.SelectedItem.ToString());
        }

        void OnResultFavouriteChanged(object sender, EventArgs<SearchResult> e) {
            Func<SearchResult, SearchResult, bool> same = (l, r) =>
                l?.TypeId?.Equals(r?.TypeId) == true && l?.VideoId?.Equals(r?.VideoId) == true;
            bool favourite = Library.IsFavourite(e.Data?.TypeId, e.Data?.VideoId);
            this.WithLayoutSuspended(() => {
                uiCurrentResult.SetFavouriteState(favourite);
                foreach (var v in uiPlaylist.Controls.Cast<ResultView>())
                    if (same(v.Result, e.Data))
                        v.SetFavouriteState(favourite);
                foreach (ResultView v in uiResults.Controls)
                    if (same(v.Result, e.Data))
                        v.SetFavouriteState(favourite);
                foreach (ResultView v in uiSuggestions.Controls)
                    if (same(v.Result, e.Data))
                        v.SetFavouriteState(favourite);
            });
        }

        void OnSuggestionsFromLocationDropDown(object sender, EventArgs e) {
            string root = UserSettings.Instance.LibraryFolder;
            List<string> allFolders = new List<string>();
            allFolders.Add(root);
            allFolders.AddRange(Directory.GetDirectories(root));
            uiSuggestionsFromLocation.WithLayoutSuspended(() => {
                foreach (var folder in allFolders)
                    if (!uiSuggestionsFromLocation.Items.Contains(folder))
                        uiSuggestionsFromLocation.Items.Add(folder);
                foreach (var item in uiSuggestionsFromLocation.Items)
                    if (!allFolders.Contains(item.ToString()))
                        uiSuggestionsFromLocation.Items.Remove(item);
            });
        }

        void OnPlaylistRandomizeClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Playlist.Instance.Randomize();
            this.WithLayoutSuspended(() => {
                UiUtility.ClearContainer<ResultView>(uiPlaylist);
                foreach (var item in Playlist.Instance.Items)
                    AddToResultsViews(uiPlaylist, item, ResultViewType.Playlist);
                Playlist.Instance.PlayNext();
            });
        }

        void OnPlaylistDumpClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var user = UserSettings.Instance;
            List<SearchResult> results = uiPlaylist.Controls
                .Cast<ResultView>()
                .Select(v => v.Result)
                .Where(v => v.Local)
                .ToList();
            ThreadPool.QueueUserWorkItem(_ => {
                for (int i = 0; i < results.Count; i++) {
                    string fileName = Path.GetFileName(results[i].VideoId);
                    File.Copy(results[i].VideoId, Path.Combine(user.PlaylistDumpFolder, fileName), true);
                    Logger.Info("Copying playlistfile {0} of {1}: {2}.", i + 1, results.Count, fileName);
                }
            });
        }

        void OnResultSimilarClicked(object sender, EventArgs<SearchResult> e) {
            searchQuery = null;
            searchRelated = null;
            UiUtility.ClearContainer<ResultView>(uiResults);
            uiLeftTabs.SelectedIndex = TabIndexSuggestions;
            Library.ClearOpenSuggestions();
            UiUtility.ClearContainer<ResultView>(uiSuggestions);
            SuggestionScanner.SearchSimilar(e.Data.Title, HandleSuggestions);
        }

        void OnResultRelatedClicked(object sender, EventArgs<SearchResult> e) {
            searchQuery = null;
            searchRelated = e.Data;
            UiUtility.ClearContainer<ResultView>(uiResults);
            uiLeftTabs.SelectedIndex = TabIndexSearch;
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
            if (initializing)
                return;
            UiSettings.Instance.SearchFavouritesOnly = uiSearchFavouriteOnly.Checked;
            StartSearch();
        }

        void OnSearchLocalOnlyCheckedChanged(object sender, EventArgs e) {
            if (initializing)
                return;
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
