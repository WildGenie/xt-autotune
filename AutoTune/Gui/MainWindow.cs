using AutoTune.Drivers;
using AutoTune.Queue;
using AutoTune.Shared;
using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace AutoTune.Gui {

    public partial class MainWindow : Form {

        static readonly string UnicodeUp = "\u25b2";
        static readonly string UnicodeDown = "\u25bc";
        static readonly string UnicodeLeft = "\u25c0";
        static readonly string UnicodeRight = "\u25b6";
        static readonly string Arch = Environment.Is64BitProcess ? "x64" : "x86";
        static readonly string AppBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        [STAThread]
        static void Main() {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCef;
            Run();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Run() {
            LocalPaths.Initialize();
            ApiKeys.Initialize();
            Settings.Initialize();
            var cef = new CefSettings();
            var settings = Settings.Instance;
            var proc = "CefSharp.BrowserSubprocess.exe";
            cef.CachePath = LocalPaths.Instance.BrowserCacheFolder;
            cef.PersistSessionCookies = settings.General.PersistSessions;
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
        private string searchQuery = null;
        private Result searchSimilar = null;
        private IDictionary<string, SearchState> searchState;
        private readonly ChromiumWebBrowser uiBrowser = new ChromiumWebBrowser(LocalPaths.Instance.StartupFilePath);

        MainWindow() {
            InitializeComponent();
            InitializeControls();
            InitializeSettings();
            InitializeColors();
            InitializeQueues();
            InitializeLog();
            initializing = false;
        }

        void InitializeQueues() {
            DownloadQueue.Initialize();
            PostProcessingQueue.Initialize();
            uiDownloadQueue.Initialize(DownloadQueue.Instance);
            uiPostProcessingQueue.Initialize(PostProcessingQueue.Instance);
            DownloadQueue.Instance.Completed += (s, e) => Invoke(new Action(() => uiPostProcessingQueue.Enqueue(e.Data.NewId())));
        }

        void InitializeControls() {
            uiBrowser.Dock = DockStyle.Fill;
            uiToggleSearch.Text = UnicodeLeft;
            uiToggleNotifications.Text = UnicodeDown;
            uiBrowserContainer.Controls.Add(uiBrowser);
            ConnectResultViewEventHandlers(uiCurrentResult);
            uiLogLevel.DataSource = Enum.GetValues(typeof(LogLevel));
            uiDownloadQueue.Play += (s, e) => uiBrowser.Load(e.Data.PlayUrl);
            uiPostProcessingQueue.Play += (s, e) => uiBrowser.Load(e.Data.PlayUrl);
        }

        void InitializeSettings() {
            var ui = Settings.Instance.UI;
            var current = Settings.Instance.General.CurrentTrack;
            uiQuery.Text = ui.LastSearch;
            uiLogLevel.SelectedItem = ui.TraceLevel;
            uiSplitSearch.Panel1Collapsed = ui.SearchCollapsed;
            uiToggleSearch.Text = ui.SearchCollapsed ? UnicodeRight : UnicodeLeft;
            uiSplitNotificationsLog.Panel2Collapsed = ui.LogCollapsed;
            uiToggleLog.Text = ui.LogCollapsed ? UnicodeLeft : UnicodeRight;
            uiSplitNotifications.Panel2Collapsed = ui.NotificationsCollapsed;
            uiToggleNotifications.Text = ui.NotificationsCollapsed ? UnicodeUp : UnicodeDown;
            uiCurrentResult.SetResult(current);
            if (current != null)
                uiBrowser.Load(current.Url);
        }

        void InitializeLog() {
            logger = new StreamWriter(Path.Combine(Settings.GetFolderPath(), "autotune.log").ToString(), false);
            Logger.Trace += (s, e) => WriteLog(e.Level, e.Message);
            Logger.Trace += (s, e) => {
                logger.WriteLine(string.Format("{0}: {1}: {2}.", DateTime.Now.ToLongTimeString(), e.Level, e.Message));
                if (e.Level == LogLevel.Error)
                    logger.Flush();
            };
        }

        void InitializeColors() {
            var theme = Settings.Instance.Theme;
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
            uiLoadMore.LinkColor = fore2;
            uiLoadMore.ActiveLinkColor = fore2;
        }

        void OnMainWindowShown(object sender, EventArgs e) {
            DownloadQueue.Start();
            PostProcessingQueue.Start();
            StartSearch();
        }

        void OnMainWindowClosed(object sender, FormClosedEventArgs e) {
            Cef.Shutdown();
            Settings.Save();
            DownloadQueue.Terminate();
            PostProcessingQueue.Terminate();
            logger.Flush();
            logger.Dispose();
        }

        void OnLogLevelSelectionChanged(object sender, EventArgs e) {
            if (!initializing)
                Settings.Instance.UI.TraceLevel = (LogLevel)uiLogLevel.SelectedItem;
        }

        void OnResultDownloadClicked(object sender, EventArgs<Result> e) {
            uiDownloadQueue.Enqueue(e.Data.NewId());
        }

        void OnQueryKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char)Keys.Return)
                return;
            StartSearch();
        }

        void OnResultSimilarClicked(object sender, EventArgs<Result> e) {
            searchQuery = null;
            searchSimilar = e.Data;
            uiResults.Controls.Clear();
            searchState = Search.Start(null, e.Data, AppendResults);
        }

        void StartSearch() {
            if (uiQuery.Text.Trim().Length == 0)
                return;
            uiResults.Controls.Clear();
            searchSimilar = null;
            searchQuery = uiQuery.Text.Trim();
            Settings.Instance.UI.LastSearch = searchQuery;
            searchState = Search.Start(searchQuery, null, AppendResults);
        }

        void OnLoadMoreClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (searchQuery != null || searchSimilar != null)
                Search.Continue(searchState, searchQuery, searchSimilar, AppendResults);
        }

        void OnUiResultsScroll(object sender, ScrollEventArgs e) {
            if (searchState == null || appendingResult || !Settings.Instance.General.AutoLoadMore)
                return;
            VScrollProperties properties = uiResults.VerticalScroll;
            if (e.NewValue != properties.Maximum - properties.LargeChange + 1)
                return;
            Search.Continue(searchState, searchQuery, searchSimilar, AppendResults);
        }

        void OnResultPlayClicked(object sender, EventArgs<Result> e) {
            uiBrowser.Load(e.Data.PlayUrl);
            Settings.Instance.General.CurrentTrack = e.Data;
            uiCurrentResult.SetResult(e.Data);
        }

        void OnToggleLogClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            uiToggleLog.Text = uiSplitNotificationsLog.Panel2Collapsed ? UnicodeRight : UnicodeLeft;
            uiSplitNotificationsLog.Panel2Collapsed = !uiSplitNotificationsLog.Panel2Collapsed;
            Settings.Instance.UI.LogCollapsed = uiSplitNotificationsLog.Panel2Collapsed;
        }

        void ToggleSearchClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            uiToggleSearch.Text = uiSplitSearch.Panel1Collapsed ? UnicodeLeft : UnicodeRight;
            uiSplitSearch.Panel1Collapsed = !uiSplitSearch.Panel1Collapsed;
            Settings.Instance.UI.SearchCollapsed = uiSplitSearch.Panel1Collapsed;
        }

        void ToggleNotificationsClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            uiToggleNotifications.Text = uiSplitNotifications.Panel2Collapsed ? UnicodeDown : UnicodeUp;
            uiSplitNotifications.Panel2Collapsed = !uiSplitNotifications.Panel2Collapsed;
            Settings.Instance.UI.NotificationsCollapsed = uiSplitNotifications.Panel2Collapsed;
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

        void AppendResults(Results results) {
            if (results.Error != null) {
                Logger.Error(results.Error, "Search error.");
            } else
                foreach (Result result in results.Items) {
                    BeginInvoke(new Action(() => {
                        var view = new ResultView();
                        ConnectResultViewEventHandlers(view);
                        view.SetResult(result);
                        uiResults.Controls.Add(view);
                        appendingResult = true;
                        uiResults.ScrollControlIntoView(view);
                        appendingResult = false;
                    }));
                }
        }

        void ConnectResultViewEventHandlers(ResultView view) {
            view.PlayClicked += OnResultPlayClicked;
            view.SimilarClicked += OnResultSimilarClicked;
            view.DownloadClicked += OnResultDownloadClicked;
            view.DoubleClick += (s, e) => OnResultPlayClicked(s, new EventArgs<Result>(((ResultView)s).Result));
        }
    }
}
