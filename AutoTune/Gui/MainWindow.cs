﻿using AutoTune.Search;
using AutoTune.Settings;
using AutoTune.Shared;
using AxWMPLib;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class MainWindow : Form {

        const int ShowLogMinWidth = 1250;
        const string AboutBlank = "about:blank";
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
            InitializeControls();
            InitializeColors();
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
            uiGroupSearch.ForeColor = fore1;
            uiDownloadGroup.ForeColor = fore1;
            uiPostProcessingGroup.ForeColor = fore1;
            uiBrowserPlayerContainer.ForeColor = fore1;
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
            uiBrowser = new ChromiumWebBrowser("");
            uiSplitBrowserPlayer.Panel1.Controls.Add(uiBrowser);
            uiBrowser.Dock = DockStyle.Fill;
            uiPlayer = new AxWindowsMediaPlayer();
            uiSplitBrowserPlayer.Panel2.Controls.Add(uiPlayer);
            uiPlayer.CreateControl();
            uiPlayer.Dock = DockStyle.Fill;
            ConnectResultViewEventHandlers(uiCurrentResult);
            uiLogLevel.DataSource = Enum.GetValues(typeof(LogLevel));
            uiDownloadQueue.Play += (s, e) => LoadResult(e.Data.Search, true);
            uiPostProcessingQueue.Play += (s, e) => LoadResult(e.Data.Search, true);
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
            if (ui.CurrentTrack != null)
                LoadResult(ui.CurrentTrack, false);
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
                        if (AppSettings.Instance.ScrollToEndOnMoreResults) {
                            appendingResult = true;
                            uiResults.ScrollControlIntoView(view);
                            appendingResult = false;
                        }
                    }));
                }
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
            if (uiQuery.Text.Trim().Length == 0)
                return;
            uiResults.Controls.Clear();
            searchRelated = null;
            searchQuery = uiQuery.Text.Trim();
            UiSettings.Instance.LastSearch = searchQuery;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            var query = new SearchQuery(credentials, searchQuery, pageSize);
            searchState = SearchEngine.Start(query, AppendResults);
        }

        void LoadResult(SearchResult result, bool start) {
            try {
                uiPlayer.Ctlcontrols.stop();
            } catch (AxHost.InvalidActiveXStateException) {

            }
            uiBrowser.Load(AboutBlank);
            uiCurrentResult.SetResult(result);
            UiSettings.Instance.CurrentTrack = result;
            uiSplitBrowserPlayer.Panel1Collapsed = false;
            uiSplitBrowserPlayer.Panel2Collapsed = false;
            Logger.Debug("Playing {0} in player.", result.Local ? result.VideoId : Utility.GetPlayUrl(result));
            if (result.Local) {
                uiSplitBrowserPlayer.Panel1Collapsed = true;
                uiPlayer.URL = result.VideoId;
                if (start)
                    uiPlayer.Ctlcontrols.play();
                else
                    uiPlayer.Ctlcontrols.stop();
            } else {
                uiSplitBrowserPlayer.Panel2Collapsed = true;
                if (start)
                    uiBrowser.Load(Utility.GetPlayUrl(result));
                else
                    uiBrowser.Load(Utility.GetUrl(result));
            }
        }

        void LoadMoreResults() {
            var typeId = searchRelated?.TypeId;
            SearchCredentials searchCredentials = null;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            if (typeId != null)
                credentials.TryGetValue(typeId, out searchCredentials);
            SearchQuery q = searchRelated == null ?
                new SearchQuery(credentials, searchQuery, pageSize) :
                new SearchQuery(typeId, searchCredentials, searchRelated.VideoId, pageSize);
            SearchEngine.Continue(q, searchState, AppendResults);
        }

        void ConnectResultViewEventHandlers(ResultView view) {
            view.PlayClicked += OnResultPlayClicked;
            view.RelatedClicked += OnResultRelatedClicked;
            view.DownloadClicked += OnResultDownloadClicked;
            view.DoubleClick += (s, e) => OnResultPlayClicked(s, new EventArgs<SearchResult>(((ResultView)s).Result));
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
    }
}
