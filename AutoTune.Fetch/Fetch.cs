using CefSharp;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoTune.Fetch {

    public static class Fetch {

        static bool shown = false;
        static string result = null;
        static bool finished = false;
        static bool pathLoad = false;
        static bool blankLoad = false;

        static readonly object LoadLock = new object();
        static readonly object ResultLock = new object();
        static readonly object FinishLock = new object();
        static readonly object InitializedLock = new object();

        const string AboutBlank = "about:blank";
        const string AppTimeout = "app-timeout";
        const string ScriptError = "script-error";
        const string UserDataDir = "user-data-dir";
        const string ScriptTimeout = "script-timeout";
        const string DisableWebSecurity = "disable-web-security";
        const string BrowserSubProcess = "CefSharp.BrowserSubprocess.exe";

        static readonly string Arch = Environment.Is64BitProcess ? "x64" : "x86";
        static readonly string AppBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public class Callback {
            public void Accept(string link) {
                lock (ResultLock) {
                    result = link;
                    Monitor.Pulse(ResultLock);
                }
            }
        }

        [STAThread]
        public static void Main(string[] args) {
            var settings = new Settings();
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCef;
            if (args.Length != 6)
                Error("Expected arguments: path url delimiter timeout delay poll retries.");
            if (!File.Exists(settings.path = args[0]))
                Error("Path '" + args[0] + "'does not exist.");
            if (string.IsNullOrEmpty(settings.url = args[1]))
                Error("No url specified.");
            if (string.IsNullOrEmpty(settings.delimiter = args[2]))
                Error("No delimiter specified.");
            if (!int.TryParse(args[3], out settings.timeout) || settings.timeout < 1)
                Error("Invalid timeout specified.");
            if (!int.TryParse(args[4], out settings.delay) || settings.delay < 1)
                Error("Invalid delay specified.");
            if (!int.TryParse(args[5], out settings.retries) || settings.retries < 1)
                Error("Invalid retries specified.");
            Run(settings);
        }

        static ChromiumWebBrowser CreateBrowser(Settings settings) {
            var result = new ChromiumWebBrowser(AboutBlank);
            result.ConsoleMessage += OnConsoleMessage;
            result.RegisterJsObject("callback", new Callback());
            result.IsBrowserInitializedChanged += OnBrowserInitialized;
            result.FrameLoadEnd += (s, e) => OnFrameLoadEnd(settings, result, e);
            return result;
        }

        static CefSettings CreateCefSettings() {
            var result = new CefSettings();
            result.CefCommandLineArgs.Add(UserDataDir, UserDataDir);
            result.CefCommandLineArgs.Add(DisableWebSecurity, DisableWebSecurity);
            result.BrowserSubprocessPath = Path.Combine(AppBase, Arch, BrowserSubProcess);
            return result;
        }

        static Form CreateForm(Settings settings, ChromiumWebBrowser browser) {
            Form result = new Form();
            result.Shown += OnShown;
            result.ShowInTaskbar = false;
            result.Controls.Add(browser);
            result.WindowState = FormWindowState.Minimized;
            return result;
        }

        static void StartPollThread(Settings settings, ChromiumWebBrowser browser) {
            Thread thread = new Thread(() => Poll(settings, browser));
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Run(Settings settings) {
            Cef.Initialize(CreateCefSettings());
            var browser = CreateBrowser(settings);
            StartPollThread(settings, browser);
            Application.Run(CreateForm(settings, browser));
            Cef.Shutdown();
            Environment.Exit(ScriptError.Equals(result) || ScriptTimeout.Equals(result) || AppTimeout.Equals(result) ? 1 : 0);
        }

        static void Error(string message) {
            Console.WriteLine(message);
            Environment.Exit(1);
        }

        static void OnShown(object sender, EventArgs e) {
            lock (InitializedLock) {
                shown = true;
                Monitor.Pulse(InitializedLock);
            }
        }

        static Assembly ResolveCef(object sender, ResolveEventArgs args) {
            if (!args.Name.StartsWith("CefSharp"))
                return null;
            var name = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
            return Assembly.LoadFile(Path.Combine(AppBase, Arch, name));
        }

        static void OnConsoleMessage(object sender, ConsoleMessageEventArgs e) {
            lock (FinishLock) {
                if (finished)
                    return;
                string fmt = "Source: {0}, line: {1}, message: {2}.";
                Console.WriteLine(string.Format(fmt, e.Source, e.Line, e.Message));
            }
        }

        static void OnBrowserInitialized(object sender, IsBrowserInitializedChangedEventArgs e) {
            if (e.IsBrowserInitialized)
                lock (InitializedLock)
                    Monitor.Pulse(InitializedLock);
        }

        static void Poll(Settings settings, ChromiumWebBrowser browser) {
            long startTicks = Environment.TickCount;
            lock (InitializedLock) {
                while (!shown || !browser.IsBrowserInitialized) {
                    Monitor.Wait(InitializedLock, settings.timeout);
                    if (!shown && Environment.TickCount - startTicks >= settings.timeout)
                        result = AppTimeout;
                }
            }
            lock (ResultLock) {
                while (result == null) {
                    Monitor.Wait(ResultLock, settings.timeout);
                    if (result == null && Environment.TickCount - startTicks >= settings.timeout)
                        result = AppTimeout;
                }
            }
            lock (FinishLock)
                finished = true;
            Console.WriteLine(settings.delimiter + result + settings.delimiter);
            Application.Exit();
        }

        static void OnFrameLoadEnd(Settings settings, ChromiumWebBrowser browser, FrameLoadEndEventArgs e) {
            bool wasPathLoad = false;
            bool wasBlankLoad = false;
            string script = string.Format("fetchLink('{0}', {1}, {2})", settings.url, settings.delay, settings.retries);
            lock (LoadLock)
                if (!blankLoad) {
                    blankLoad = true;
                    wasBlankLoad = true;
                } else if (!pathLoad) {
                    pathLoad = true;
                    wasPathLoad = true;
                }
            lock (FinishLock)
                if (finished)
                    return;
                else if (wasBlankLoad)
                    browser.Load(settings.path);
                else if (wasPathLoad)
                    e.Browser.MainFrame.ExecuteJavaScriptAsync(script);
        }
    }
}
