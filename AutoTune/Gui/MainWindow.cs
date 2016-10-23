using AutoTune.Local;
using AutoTune.Processing;
using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AutoTune.Gui {

    internal partial class MainWindow : Form {

        const int ShowLogMinWidth = 1250;
        static readonly string UnicodeBlackLowerLeftTriangle = "\u25e3";
        static readonly string UnicodeBlackUpperRightTriangle = "\u25e5";
        static readonly string UnicodeWhiteLowerLeftTriangle = "\u25fa";
        static readonly string UnicodeWhiteUpperRightTriangle = "\u25f9";
        static readonly string UnicodeBlackUpPointingTriangle = "\u25b2";
        static readonly string UnicodeBlackDownPointingTriangle = "\u25bc";
        static readonly string UnicodeBlackLeftPointingTriangle = "\u25c0";
        static readonly string UnicodeBlackRightPointingTriangle = "\u25b6";
        static readonly string Arch = Environment.Is64BitProcess ? "x64" : "x86";
        static readonly string AppBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        [STAThread]
        static void Main() {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCef;
            Run();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Run() {
            UiSettings.Initialize();
            AppSettings.Initialize();
            UserSettings.Initialize();
            ThemeSettings.Initialize();
            var cef = new CefSettings();
            var proc = "CefSharp.BrowserSubprocess.exe";
            cef.BrowserSubprocessPath = Path.Combine(AppBase, Arch, proc);
            Cef.Initialize(cef);
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

        private TextWriter logger;
        private bool appendingResult;
        private bool initializing = true;
        private object searchState = null;
        private string searchQuery = null;
        private SearchResult searchRelated = null;
        private readonly ChromiumWebBrowser uiBrowser = new ChromiumWebBrowser(AppSettings.StartupFilePath);

        MainWindow() {
            InitializeComponent();
            InitializeControls();
            InitializeSettings();
            InitializeColors();
            InitializeLog();
            initializing = false;
        }

        void InitializeControls() {
            uiBrowser.Dock = DockStyle.Fill;
            uiBrowserContainer.Controls.Add(uiBrowser);
            ConnectResultViewEventHandlers(uiCurrentResult);
            uiLogLevel.DataSource = Enum.GetValues(typeof(LogLevel));
            uiDownloadQueue.Play += (s, e) => PlayResult(e.Data.Search);
            uiPostProcessingQueue.Play += (s, e) => PlayResult(e.Data.Search);
        }

        void InitializeSettings() {
            var ui = UiSettings.Instance;
            uiQuery.Text = ui.LastSearch;
            uiLogLevel.SelectedItem = ui.TraceLevel;
            ToggleFullScreen(ui.FullScreen);
            ToggleLog(UiSettings.Instance.LogCollapsed);
            TogglePlayerFull(UiSettings.Instance.PlayerFull);
            ToggleSearch(UiSettings.Instance.SearchCollapsed);
            ToggleNotifications(UiSettings.Instance.NotificationsCollapsed);
            ToggleCurrentControls(UiSettings.Instance.CurrentControlsCollapsed);
            if (ui.CurrentTrack != null) {
                var url = Utility.GetUrl(ui.CurrentTrack);
                Logger.Debug("Opening {0} in player.", url);
                uiBrowser.Load(url);
            }
        }

        void InitializeLog() {
            logger = new StreamWriter(AppSettings.LogFilePath, false);
            Logger.Trace += (s, e) => WriteLog(e.Level, e.Message);
            Logger.Trace += (s, e) => {
                logger.WriteLine(string.Format("{0}: {1}: {2}.", DateTime.Now.ToLongTimeString(), e.Level, e.Message));
                if (e.Level == LogLevel.Error)
                    logger.Flush();
            };
        }

        void InitializeColors() {
            var theme = ThemeSettings.Instance;
            var back1 = ColorTranslator.FromHtml(theme.BackColor1);
            var back2 = ColorTranslator.FromHtml(theme.BackColor2);
            var fore1 = ColorTranslator.FromHtml(theme.ForeColor1);
            var fore2 = ColorTranslator.FromHtml(theme.ForeColor2);
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
            uiGroupSearch.ForeColor = fore1;
            uiDownloadGroup.ForeColor = fore1;
            uiPostProcessingGroup.ForeColor = fore1;
            uiToggleLog.LinkColor = fore1;
            uiToggleLog.ActiveLinkColor = fore1;
            uiToggleSearch.LinkColor = fore1;
            uiToggleSearch.ActiveLinkColor = fore1;
            uiToggleNotifications.LinkColor = fore1;
            uiToggleNotifications.ActiveLinkColor = fore1;
            uiToggleCurrentControls.LinkColor = fore1;
            uiToggleFullScreen.LinkColor = fore1;
            uiToggleFullScreen.ActiveLinkColor = fore1;
            uiTogglePlayerFull.LinkColor = fore1;
            uiTogglePlayerFull.ActiveLinkColor = fore1;
            uiToggleCurrentControls.ActiveLinkColor = fore1;
            uiLoadMore.LinkColor = fore2;
            uiLoadMore.ActiveLinkColor = fore2;
        }

        void OnMainWindowShown(object sender, EventArgs e) {
            DownloadQueue.Initialize();
            PostProcessingQueue.Initialize();
            Database.Initialize(AppSettings.GetFolderPath());
            uiDownloadQueue.Initialize(DownloadQueue.Instance);
            uiPostProcessingQueue.Initialize(PostProcessingQueue.Instance);
            DownloadQueue.Instance.Completed += (s, evt) => Invoke(new Action(() => uiPostProcessingQueue.Enqueue(evt.Data.NewId())));
            DownloadQueue.Start();
            PostProcessingQueue.Start();
            Scanner.Start(UserSettings.Instance.LibraryFolder);
            StartSearch();
            uiCurrentResult.SetResult(UiSettings.Instance.CurrentTrack);
        }

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

        void OnLogLevelSelectionChanged(object sender, EventArgs e) {
            if (!initializing)
                UiSettings.Instance.TraceLevel = (LogLevel)uiLogLevel.SelectedItem;
        }

        void OnResultDownloadClicked(object sender, EventArgs<SearchResult> e) {
            uiDownloadQueue.Enqueue(new QueueItem(e.Data));
        }

        void OnQueryKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char)Keys.Return)
                return;
            StartSearch();
        }

        void OnResultRelatedClicked(object sender, EventArgs<SearchResult> e) {
            searchQuery = null;
            searchRelated = e.Data;
            uiResults.Controls.Clear();
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = Utility.GetSearchCredentials()[e.Data.TypeId];
            var query = new SearchQuery(e.Data.TypeId, credentials, e.Data.VideoId, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void StartSearch() {
            if (uiQuery.Text.Trim().Length == 0)
                return;
            uiResults.Controls.Clear();
            searchRelated = null;
            searchQuery = uiQuery.Text.Trim();
            UiSettings.Instance.LastSearch = searchQuery;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var query = new SearchQuery(Utility.GetSearchCredentials(), searchQuery, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void LoadMore() {
            var typeId = searchRelated?.TypeId;
            var credentials = Utility.GetSearchCredentials();
            var pageSize = AppSettings.Instance.SearchPageSize;
            SearchQuery q = searchRelated == null ? 
                new SearchQuery(credentials, searchQuery, pageSize) :
                new SearchQuery(typeId, credentials[typeId], searchRelated.VideoId, pageSize);
            SearchEngine.Continue(q, searchState, AppendResults);
        }

        void OnMainWindowResized(object sender, EventArgs e) {
            ToggleLog(UiSettings.Instance.LogCollapsed);
            uiToggleLog.Visible = Width >= ShowLogMinWidth;
        }

        void OnLoadMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (searchQuery != null || searchRelated != null)
                LoadMore();
        }

        void OnUiResultsScroll(object sender, ScrollEventArgs e) {
            var autoLoad = AppSettings.Instance.AutoLoadMoreSearchResults;
            if (searchState == null || appendingResult || !autoLoad)
                return;
            VScrollProperties properties = uiResults.VerticalScroll;
            if (e.NewValue != properties.Maximum - properties.LargeChange + 1)
                return;
            LoadMore();
        }

        void OnResultPlayClicked(object sender, EventArgs<SearchResult> e) {
            PlayResult(e.Data);
            UiSettings.Instance.CurrentTrack = e.Data;
            uiCurrentResult.SetResult(e.Data);
        }

        void PlayResult(SearchResult result) {
            uiBrowser.Load(Utility.GetPlayUrl(result));
            Logger.Debug("Playing {0} in player.", Utility.GetPlayUrl(result));
        }

        void OnToggleLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (Width < ShowLogMinWidth)
                return;
            UiSettings.Instance.LogCollapsed = !UiSettings.Instance.LogCollapsed;
            ToggleLog(UiSettings.Instance.LogCollapsed);
        }

        void ToggleLog(bool collapsed) {
            bool realCollapsed = collapsed || Width < ShowLogMinWidth;
            uiToggleLog.Text = realCollapsed ? UnicodeBlackLeftPointingTriangle : UnicodeBlackRightPointingTriangle;
            uiSplitNotificationsLog.Panel2Collapsed = realCollapsed;
        }

        void OnToggleFullScreenClick(object sender, LinkLabelLinkClickedEventArgs e) {
            UiSettings.Instance.FullScreen = !UiSettings.Instance.FullScreen;
            ToggleFullScreen(UiSettings.Instance.FullScreen);
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

        void ToggleSearchClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleSearch(!UiSettings.Instance.SearchCollapsed);
            UiSettings.Instance.SearchCollapsed = !UiSettings.Instance.SearchCollapsed;
        }

        void ToggleSearch(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleSearch.Text = realCollapsed ? UnicodeBlackRightPointingTriangle : UnicodeBlackLeftPointingTriangle;
            uiSplitSearch.Panel1Collapsed = realCollapsed;
        }

        void ToggleNotificationsClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleNotifications(!UiSettings.Instance.NotificationsCollapsed);
            UiSettings.Instance.NotificationsCollapsed = uiSplitNotifications.Panel2Collapsed;
        }

        void ToggleNotifications(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleNotifications.Text = realCollapsed ? UnicodeBlackUpPointingTriangle : UnicodeBlackDownPointingTriangle;
            uiSplitNotifications.Panel2Collapsed = realCollapsed;
        }

        void OnToggleCurrentControlsClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            ToggleCurrentControls(!UiSettings.Instance.CurrentControlsCollapsed);
            UiSettings.Instance.CurrentControlsCollapsed = uiSplitBrowserCurrentControls.Panel1Collapsed;
        }

        void ToggleCurrentControls(bool collapsed) {
            bool realCollapsed = collapsed || UiSettings.Instance.PlayerFull;
            uiToggleCurrentControls.Text = realCollapsed ? UnicodeBlackDownPointingTriangle : UnicodeBlackUpPointingTriangle;
            uiSplitBrowserCurrentControls.Panel1Collapsed = realCollapsed;
        }

        void OnTogglePlayerFullClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            var ui = UiSettings.Instance;
            ui.PlayerFull = !ui.PlayerFull;
            TogglePlayerFull(ui.PlayerFull);
        }

        void TogglePlayerFull(bool full) {
            var ui = UiSettings.Instance;
            ToggleSearch(ui.SearchCollapsed);
            ToggleNotifications(ui.NotificationsCollapsed);
            ToggleCurrentControls(ui.CurrentControlsCollapsed);
            uiToggleSearch.Visible = !full;
            uiToggleNotifications.Visible = !full;
            uiToggleCurrentControls.Visible = !full;
            uiTogglePlayerFull.Text = full ? UnicodeWhiteLowerLeftTriangle : UnicodeWhiteUpperRightTriangle;
        }

        void WriteLog(LogLevel level, string text) {
            Invoke(new Action(() => {
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

        void AppendResults(SearchResponse response) {
            if (response.Error != null) {
                Logger.Error(response.Error, "Search error.");
            } else
                foreach (SearchResult result in response.Results) {
                    BeginInvoke(new Action(() => {
                        var view = new ResultView();
                        ConnectResultViewEventHandlers(view);
                        uiResults.Controls.Add(view);
                        view.SetResult(result);
                        appendingResult = true;
                        uiResults.ScrollControlIntoView(view);
                        appendingResult = false;
                    }));
                }
        }

        void ConnectResultViewEventHandlers(ResultView view) {
            view.PlayClicked += OnResultPlayClicked;
            view.RelatedClicked += OnResultRelatedClicked;
            view.DownloadClicked += OnResultDownloadClicked;
            view.DoubleClick += (s, e) => OnResultPlayClicked(s, new EventArgs<SearchResult>(((ResultView)s).Result));
        }
    }
}
