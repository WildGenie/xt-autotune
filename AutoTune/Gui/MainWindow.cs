using AutoTune.Processing;
using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using AutoTune.Web;
using AxWMPLib;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class MainWindow : Form {

        const int SbVert = 1;
        const int MediaEnded = 8;
        const int ShowLogMinWidth = 1250;
        const int TabIndexSearch = 1;
        const int TabIndexPlaylist = 0;
        const string UnicodeBlackLowerLeftTriangle = "\u25e3";
        const string UnicodeWhiteLowerLeftTriangle = "\u25fa";
        const string UnicodeBlackUpperRightTriangle = "\u25e5";
        const string UnicodeWhiteUpperRightTriangle = "\u25f9";
        const string UnicodeBlackUpPointingTriangle = "\u25b2";
        const string UnicodeBlackDownPointingTriangle = "\u25bc";
        const string UnicodeBlackLeftPointingTriangle = "\u25c0";
        const string UnicodeBlackRightPointingTriangle = "\u25b6";
        static readonly string Arch = Environment.Is64BitProcess ? "x64" : "x86";
        static readonly string AppBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        
        [STAThread]
        static void Main() {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCef;
            Run();
        }

        static void LoadSettings() {
            UiSettings.Initialize();
            AppSettings.Initialize();
            UserSettings.Initialize();
            ThemeSettings.Initialize();
        }

        static CefSettings CreateCefSettings() {
            var result = new CefSettings();
            var proc = "CefSharp.BrowserSubprocess.exe";
            result.BrowserSubprocessPath = Path.Combine(AppBase, Arch, proc);
            return result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Run() {
            LoadSettings();
            CefSharpSettings.WcfTimeout = new TimeSpan(0);
            Cef.Initialize(CreateCefSettings());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }

        static Assembly ResolveCef(object sender, ResolveEventArgs args) {
            if (!args.Name.StartsWith("CefSharp"))
                return null;
            var name = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            return Assembly.LoadFile(Path.Combine(AppBase, Arch, name));
        }

        private bool shutdown;
        private bool startPlaying;
        private TextWriter logger;
        private bool appendingResult;
        private bool initializing = true;
        private object searchState = null;
        private string searchQuery = null;
        private SearchResult searchRelated = null;
        private ChromiumWebBrowser uiBrowser;
        private AxWindowsMediaPlayer uiPlayer;
        private readonly object shutdownLock = new object();

        public MainWindow() {
            InitializeComponent();
            if (DesignMode)
                return;
            try {
                SuspendLayout();
                InitializeControls();
                InitializeColors();
            } finally {
                ResumeLayout();
            }
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var back1 = ColorTranslator.FromHtml(theme.BackColor1);
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            BackColor = back1;
            uiLog.ForeColor = fore1;
            uiLog.BackColor = back1;
            uiLogLevel.BackColor = back2;
            uiLogLevel.ForeColor = fore1;
            uiLogLevelLabel.ForeColor = fore1;
            uiQuery.ForeColor = fore1;
            uiQuery.BackColor = back2;
            uiLogGroup.ForeColor = fore1;
            uiCurrentGroup.ForeColor = fore1;
            uiLeftTabs.ForeColor = fore1;
            uiLeftTabs.BackColor = back1;
            uiLeftTabsSearch.ForeColor = fore1;
            uiLeftTabsSearch.BackColor = back1;
            uiLeftTabsPlaylist.ForeColor = fore1;
            uiLeftTabsPlaylist.BackColor = back1;
            uiDownloadGroup.ForeColor = fore1;
            uiPostProcessingGroup.ForeColor = fore1;
            uiBrowserPlayerContainer.ForeColor = fore1;
            uiPlaylistModeAll.ForeColor = fore1;
            uiPlaylistModeTrack.ForeColor = fore1;
            uiPlaylistModeRandom.ForeColor = fore1;
            Utility.SetLinkForeColors(uiPlaylistStop);
            Utility.SetLinkForeColors(uiPlaylistNext);
            Utility.SetLinkForeColors(uiPlaylistClear);
            Utility.SetLinkForeColors(uiLoadMore);
            Utility.SetToggleForeColors(uiToggleLog);
            Utility.SetToggleForeColors(uiToggleSearch);
            Utility.SetToggleForeColors(uiToggleFullScreen);
            Utility.SetToggleForeColors(uiTogglePlayerFull);
            Utility.SetToggleForeColors(uiToggleNotifications);
            Utility.SetToggleForeColors(uiToggleCurrentControls);
        }

        void InitializeControls() {
            uiSplitBrowserPlayer.Panel1Collapsed = true;
            uiSplitBrowserPlayer.Panel2Collapsed = true;
            uiBrowser = new ChromiumWebBrowser(AppSettings.EmptyHtmlFilePath);
            uiSplitBrowserPlayer.Panel1.Controls.Add(uiBrowser);
            if (AppSettings.Instance.LogBrowserConsole)
                uiBrowser.ConsoleMessage += (s, e) => Logger.Debug("Browser console: " + e.Message);
            uiBrowser.RegisterJsObject("videoCallbacks", new VideoCallbacks());
            uiBrowser.Dock = DockStyle.Fill;
            uiPlayer = new AxWindowsMediaPlayer();
            uiSplitBrowserPlayer.Panel2.Controls.Add(uiPlayer);
            uiPlayer.CreateControl();
            uiPlayer.Dock = DockStyle.Fill;
            uiPlayer.PlayStateChange += (s, e) => {
                if (e.newState == MediaEnded)
                    Playlist.Instance.Stopped();
            };
            ConnectResultViewEventHandlers(uiCurrentResult);
            uiLogLevel.DataSource = Enum.GetValues(typeof(LogLevel));
            uiDownloadQueue.Play += (s, e) => PlayResult(e.Data.Search);
            uiPostProcessingQueue.Play += (s, e) => PlayResult(e.Data.Search);
            uiBrowser.FrameLoadEnd += (s, e) => {
                var track = UiSettings.Instance.CurrentTrack;
                if (track == null)
                    return;
                var provider = AppSettings.GetProvider(track.TypeId);
                if (provider.EmbedFile == null || !e.Url.Equals(new Uri(provider.GetEmbedFilePath()).ToString()))
                    return;
                string script = string.Format("loadVideo('{0}', {1})", track.VideoId, startPlaying ? "true" : "false");
                uiBrowser.GetMainFrame().ExecuteJavaScriptAsync(script);
            };
        }

        void InitializeSettings() {
            var ui = UiSettings.Instance;
            try {
                SuspendLayout();
                uiQuery.Text = ui.LastSearch;
                uiLogLevel.SelectedItem = ui.TraceLevel;
                uiSearchLocalOnly.Checked = ui.SearchLocalOnly;
                uiSearchFavouriteOnly.Checked = ui.SearchFavouritesOnly;
                ToggleFullScreen(ui.FullScreen);
                ToggleLog(UiSettings.Instance.LogCollapsed);
                TogglePlayerFull(UiSettings.Instance.PlayerFull);
                ToggleSearch(UiSettings.Instance.SearchCollapsed);
                ToggleNotifications(UiSettings.Instance.NotificationsCollapsed);
                ToggleCurrentControls(UiSettings.Instance.CurrentControlsCollapsed);
            } finally {
                ResumeLayout();
            }
            if (ui.CurrentTrack != null)
                uiBrowser.IsBrowserInitializedChanged += (s, e) => {
                    if (e.IsBrowserInitialized)
                        BeginInvoke(new Action(() => LoadResult(ui.CurrentTrack, false)));
                };
        }

        void InitializeLog() {
            logger = new StreamWriter(AppSettings.LogFilePath, false);
            Logger.Trace += (s, e) => WriteLog(e.Level, e.Message);
            Logger.Trace += (s, e) => {
                lock (shutdownLock) {
                    string now = DateTime.Now.ToLongTimeString();
                    if (!shutdown) {
                        logger.WriteLine(string.Format("{0}: {1}: {2}.", now, e.Level, e.Message));
                        if (e.Level == LogLevel.Error)
                            logger.Flush();
                    }
                }
            };
        }

        void AddToPlaylist(SearchResult result) {
            if (Playlist.Instance.Add(result))
                AddPlaylistView(result);
        }

        void AddPlaylistView(SearchResult result) {
            var view = new ResultView(true);
            ConnectResultViewEventHandlers(view);
            uiPlaylist.Controls.Add(view);
            view.SetResult(result);
        }

        void AppendResults(SearchResponse response) {
            if (response.Error != null) {
                Logger.Error(response.Error, "Search error.");
            } else
                BeginInvoke(new Action(() => {
                    try {
                        SuspendLayout();
                        foreach (SearchResult result in response.Results) {
                            var view = new ResultView(false);
                            ConnectResultViewEventHandlers(view);
                            uiResults.Controls.Add(view);
                            view.SetResult(result);
                            if (AppSettings.Instance.ScrollToEndOnMoreResults) {
                                appendingResult = true;
                                uiResults.ScrollControlIntoView(view);
                                appendingResult = false;
                            }
                        }
                    } finally {
                        ResumeLayout();
                    }
                }));
        }

        void WriteLog(LogLevel level, string text) {
            BeginInvoke(new Action(() => {
                if (level < (LogLevel)uiLogLevel.SelectedItem)
                    return;
                var line = DateTime.Now.ToLongTimeString() + ": ";
                line += level + ": " + text + Environment.NewLine;
                var newText = uiLog.Text + line;
                if (newText.Length > 10000)
                    newText = newText.Substring(newText.Length - 10000);
                uiLog.Text = newText;
                uiLog.SelectionStart = uiLog.TextLength;
                uiLog.ScrollToCaret();
            }));
        }

        void StartSearch() {
            var ui = UiSettings.Instance;
            if (uiQuery.Text.Trim().Length == 0)
                return;
            uiResults.Controls.Clear();
            searchRelated = null;
            searchQuery = uiQuery.Text.Trim();
            UiSettings.Instance.LastSearch = searchQuery;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            var query = new SearchQuery(credentials, searchQuery, ui.SearchFavouritesOnly, ui.SearchLocalOnly, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void PlayResult(SearchResult result) {
            AddToPlaylist(result);
            Playlist.Instance.Start();
            Playlist.Instance.Play(result);
            SetPlaylistPlaying(result);
        }

        void SetPlaylistPlaying(SearchResult result) {
            try {
                SuspendLayout();
                foreach (ResultView view in uiPlaylist.Controls) {
                    view.SetPlaying(false);
                    if (result != null && view.Result.TypeId.Equals(result.TypeId) && view.Result.VideoId.Equals(result.VideoId))
                        view.SetPlaying(true);
                }
            } finally {
                ResumeLayout();
            }
        }

        void LoadResult(SearchResult result, bool start) {
            try {
                uiPlayer.Ctlcontrols.stop();
            } catch (AxHost.InvalidActiveXStateException) {

            }
            uiBrowser.Load(AppSettings.EmptyHtmlFilePath);
            if (result == null)
                return;
            uiCurrentResult.SetResult(result);
            uiSplitBrowserPlayer.Panel1Collapsed = false;
            uiSplitBrowserPlayer.Panel2Collapsed = false;
            if (result == null)
                return;
            UiSettings.Instance.CurrentTrack = result;
            Logger.Debug("Loading {0} ({1}: {2}) in player.", result.Title, result.TypeId, result.VideoId);
            if (result.Local) {
                LoadLocalResult(result, start);
            } else {
                LoadWebResult(result, start);
            }
        }

        void LoadLocalResult(SearchResult result, bool start) {
            uiSplitBrowserPlayer.Panel1Collapsed = true;
            uiPlayer.URL = result.VideoId;
            if (start)
                uiPlayer.Ctlcontrols.play();
            else
                uiPlayer.Ctlcontrols.stop();
        }

        void LoadWebResult(SearchResult result, bool start) {
            startPlaying = start;
            var provider = AppSettings.GetProvider(result.TypeId);
            uiSplitBrowserPlayer.Panel2Collapsed = true;
            uiBrowser.RequestHandler = new RefererRequestHandler(provider.HttpReferer);
            uiBrowser.Load(provider.GetEmbedFilePath());
        }

        void LoadMoreResults() {
            var ui = UiSettings.Instance;
            var typeId = searchRelated?.TypeId;
            SearchCredentials searchCredentials = null;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            if (typeId != null)
                credentials.TryGetValue(typeId, out searchCredentials);
            SearchQuery q = searchRelated == null ?
                new SearchQuery(credentials, searchQuery, ui.SearchFavouritesOnly, ui.SearchLocalOnly, pageSize) :
                new SearchQuery(typeId, searchCredentials, searchRelated.VideoId, pageSize);
            SearchEngine.Continue(q, searchState, AppendResults);
        }

        void ConnectResultViewEventHandlers(ResultView view) {
            view.PlayClicked += OnResultPlayClicked;
            view.QueueClicked += OnResultQueueClicked;
            view.RelatedClicked += OnResultRelatedClicked;
            view.DownloadClicked += OnResultDownloadClicked;
            view.RemoveClicked += (s, e) => RemoveFromPlaylist(view);
        }

        void RemoveFromPlaylist(ResultView view) {
            uiPlaylist.Controls.Remove(view);
            Playlist.Instance.Remove(view.Result);
        }

        void ToggleFullScreen(bool fullScreen) {
            if (fullScreen) {
                TopMost = true;
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                uiToggleFullScreen.Text = UnicodeBlackLowerLeftTriangle;
            } else {
                TopMost = false;
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.Sizable;
                uiToggleFullScreen.Text = UnicodeBlackUpperRightTriangle;
            }
        }

        void TogglePlayerFull(bool full) {
            var ui = UiSettings.Instance;
            ToggleSearch(ui.SearchCollapsed);
            ToggleNotifications(ui.NotificationsCollapsed);
            ToggleCurrentControls(ui.CurrentControlsCollapsed);
            uiToggleSearch.Visible = !full;
            uiToggleNotifications.Visible = !full;
            uiToggleCurrentControls.Visible = !full;
            uiBrowserPlayerContainer.BorderStyle = full ? BorderStyle.None : BorderStyle.FixedSingle;
            uiTogglePlayerFull.Text = full ? UnicodeWhiteLowerLeftTriangle : UnicodeWhiteUpperRightTriangle;
        }

        void ToggleLog(bool collapsed) {
            bool realCollapsed = collapsed || Width < ShowLogMinWidth;
            uiToggleLog.Text = realCollapsed ? UnicodeBlackLeftPointingTriangle : UnicodeBlackRightPointingTriangle;
            uiSplitNotificationsLog.Panel2Collapsed = realCollapsed;
        }

        void ToggleSearch(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleSearch.Text = realCollapsed ? UnicodeBlackRightPointingTriangle : UnicodeBlackLeftPointingTriangle;
            uiSplitSearch.Panel1Collapsed = realCollapsed;
        }

        void ToggleNotifications(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleNotifications.Text = realCollapsed ? UnicodeBlackUpPointingTriangle : UnicodeBlackDownPointingTriangle;
            uiSplitNotifications.Panel2Collapsed = realCollapsed;
        }

        void ToggleCurrentControls(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleCurrentControls.Text = realCollapsed ? UnicodeBlackDownPointingTriangle : UnicodeBlackUpPointingTriangle;
            uiSplitBrowserCurrentControls.Panel1Collapsed = realCollapsed;
        }

        private void OnPlaylistStartClicked(object sender, LinkLabelLinkClickedEventArgs e) {

        }
    }
}
