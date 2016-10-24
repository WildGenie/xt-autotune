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

    public partial class MainWindow : Form {

        const int ShowLogMinWidth = 1250;
        static readonly string UnicodeBlackLowerLeftTriangle = "\u25e3";
        static readonly string UnicodeWhiteLowerLeftTriangle = "\u25fa";
        static readonly string UnicodeBlackUpperRightTriangle = "\u25e5";
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

        private TextWriter logger;
        private bool appendingResult;
        private bool initializing = true;
        private object searchState = null;
        private string searchQuery = null;
        private SearchResult searchRelated = null;
        private readonly ChromiumWebBrowser uiBrowser;

        public MainWindow() {
            InitializeComponent();
            if (DesignMode)
                return;
            uiBrowser = new ChromiumWebBrowser(AppSettings.StartupFilePath);
            InitializeControls();
            InitializeSettings();
            InitializeColors();
            initializing = false;
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
            Utility.SetLinkForeColors(uiLoadMore);
            Utility.SetToggleForeColors(uiToggleLog);
            Utility.SetToggleForeColors(uiToggleSearch);
            Utility.SetToggleForeColors(uiToggleFullScreen);
            Utility.SetToggleForeColors(uiTogglePlayerFull);
            Utility.SetToggleForeColors(uiToggleNotifications);
            Utility.SetToggleForeColors(uiToggleCurrentControls);
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
                string now = DateTime.Now.ToLongTimeString();
                logger.WriteLine(string.Format("{0}: {1}: {2}.", now, e.Level, e.Message));
                if (e.Level == LogLevel.Error)
                    logger.Flush();
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
                        appendingResult = true;
                        uiResults.ScrollControlIntoView(view);
                        appendingResult = false;
                    }));
                }
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

        void PlayResult(SearchResult result) {
            uiBrowser.Load(Utility.GetPlayUrl(result));
            Logger.Debug("Playing {0} in player.", Utility.GetPlayUrl(result));
        }

        void LoadMoreResults() {
            var typeId = searchRelated?.TypeId;
            var pageSize = AppSettings.Instance.SearchPageSize;
            var credentials = UserSettings.Instance.Credentials;
            SearchQuery q = searchRelated == null ?
                new SearchQuery(credentials, searchQuery, pageSize) :
                new SearchQuery(typeId, credentials[typeId], searchRelated.VideoId, pageSize);
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
