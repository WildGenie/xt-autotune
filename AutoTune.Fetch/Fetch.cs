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
        const string AboutBlank = "about:blank";
        const string AppTimeout = "app-timeout";
        const string ScriptError = "script-error";
        const string UserDataDir = "user-data-dir";
        const string ScriptTimeout = "script-timeout";
        static readonly object LoadLock = new object();
        static readonly object ResultLock = new object();
        static readonly object FinishLock = new object();
        static readonly object InitializedLock = new object();
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
            int timeout;
            AppDomain.CurrentDomain.AssemblyResolve += ResolveCef;
            if (args.Length != 4)
                Error("Expected arguments: path url delimiter timeout.");
            if (!File.Exists(args[0]))
                Error("Path '" + args[0] + "'does not exist.");
            if (string.IsNullOrEmpty(args[1]))
                Error("No url specified.");
            if (string.IsNullOrEmpty(args[2]))
                Error("No delimiter specified.");
            if (!int.TryParse(args[3], out timeout) || timeout < 1)
                Error("Invalid timeout specified.");
            Run(args[0], args[1], args[2], timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void Run(string path, string url, string delimiter, int timeout) {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add(UserDataDir, UserDataDir);
            settings.CefCommandLineArgs.Add(DisableWebSecurity, DisableWebSecurity);
            settings.BrowserSubprocessPath = Path.Combine(AppBase, Arch, BrowserSubProcess);
            Cef.Initialize(settings);
            Form form = new Form();
            form.Shown += OnShown;
            form.ShowInTaskbar = false;
            form.WindowState = FormWindowState.Minimized;
            ChromiumWebBrowser browser = new ChromiumWebBrowser(AboutBlank);
            browser.ConsoleMessage += OnConsoleMessage;
            browser.RegisterJsObject("callback", new Callback());
            browser.IsBrowserInitializedChanged += OnBrowserInitialized;
            browser.FrameLoadEnd += (s, e) => OnFrameLoadEnd(browser, path, url, e);
            form.Controls.Add(browser);
            Thread finish = new Thread(() => Finish(browser, delimiter, timeout));
            finish.SetApartmentState(ApartmentState.STA);
            finish.IsBackground = true;
            finish.Start();
            Application.Run(form);
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

        static void Finish(ChromiumWebBrowser browser, string delimiter, int timeout) {
            long startTicks = Environment.TickCount;
            lock (InitializedLock) {
                while (!shown || !browser.IsBrowserInitialized) {
                    Monitor.Wait(InitializedLock, timeout);
                    if (!shown && Environment.TickCount - startTicks >= timeout)
                        result = AppTimeout;
                }
            }
            lock (ResultLock) {
                while (result == null) {
                    Monitor.Wait(ResultLock, timeout);
                    if (result == null && Environment.TickCount - startTicks >= timeout)
                        result = AppTimeout;
                }
            }
            lock (FinishLock)
                finished = true;
            Console.WriteLine(delimiter + result + delimiter);
            Application.Exit();
        }

        static void OnFrameLoadEnd(ChromiumWebBrowser browser, string path, string url, FrameLoadEndEventArgs e) {
            bool wasPathLoad = false;
            bool wasBlankLoad = false;
            string script = "fetchLink('" + url + "')";
            lock (LoadLock) {
                if (!blankLoad) {
                    blankLoad = true;
                    wasBlankLoad = true;
                } else if (!pathLoad) {
                    pathLoad = true;
                    wasPathLoad = true;
                }
            }
            lock (FinishLock) {
                if (finished)
                    return;
                if (wasBlankLoad) {
                    browser.Load(path);
                } else if (wasPathLoad)
                    e.Browser.MainFrame.ExecuteJavaScriptAsync(script);
            }
        }
    }
}
