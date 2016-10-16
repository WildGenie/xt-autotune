﻿namespace AutoTune.Shared {

    public class Settings : SettingsBase<Settings> {

        private const string FileName = "settings.xml";

        public class DriverSettings {
            public bool Enabled { get; set; } = true;
            public bool PostProcess { get; set; } = true;
            public bool KeepOriginal { get; set; } = false;
            public string UrlPattern { get; set; }
            public string PlayUrlPattern { get; set; }
            public string DownloadUrlPattern { get; set; }
        }

        public class YouTubeSettings : DriverSettings {
            public YouTubeSettings() {
                UrlPattern = "https://www.youtube.com/embed/{0}?autoplay=0&fs=0&modestbranding=1";
                PlayUrlPattern = "https://www.youtube.com/embed/{0}?autoplay=1&fs=0&modestbranding=1";
                DownloadUrlPattern = "https://www.youtube.com/watch?v={0}";
            }
        }

        public class VimeoSettings : DriverSettings {
            public VimeoSettings() {
                UrlPattern = "https://player.vimeo.com/video/{0}?autoplay=0";
                PlayUrlPattern = "https://player.vimeo.com/video/{0}?autoplay=1";
                DownloadUrlPattern = "https://player.vimeo.com/video/{0}";
            }
        }

        public class GeneralSettings {
            public int ExtractorTimeout = 10000;
            public int PageSize { get; set; } = 10;
            public Result CurrentTrack { get; set; }
            public int DownloadThreads { get; set; } = 0;
            public bool AutoLoadMore { get; set; } = true;
            public string AppName { get; set; } = "AutoTune";
            public bool PersistSessions { get; set; } = true;
            public string ExtractorDelimiter = "{4863DB53-583E-4FB2-AC13-B76EB566A8AF}";
        }

        public class ThemeSettings {
            public string BackColor1 { get; set; } = "#000000";
            public string BackColor2 { get; set; } = "#222222";
            public string ForeColor1 { get; set; } = "#cccccc";
            public string ForeColor2 { get; set; } = "#f12b24";
        }

        public class UISettings {
            public string LastSearch { get; set; } = "";
            public LogLevel TraceLevel { get; set; } = LogLevel.Info;
            public bool LogCollapsed { get; set; } = true;
            public bool SearchCollapsed { get; set; } = false;
            public bool NotificationsCollapsed { get; set; } = false;
        }

        public class PostProcessingSettings {
            public string Command = "ffmpeg";
            public int Threads { get; set; } = 0;
            public string Extension { get; set; } = "mp3";
            public string Arguments { get; set; } = "-i \"{0}\" -y \"{1}.{2}\" -quality good -cpu-used 0";
        }

        public GeneralSettings General { get; set; } = new GeneralSettings();
        public PostProcessingSettings PostProcessing { get; set; } = new PostProcessingSettings();
        public VimeoSettings Vimeo { get; set; } = new VimeoSettings();
        public YouTubeSettings YouTube { get; set; } = new YouTubeSettings();
        public ThemeSettings Theme { get; set; } = new ThemeSettings();
        public UISettings UI { get; set; } = new UISettings();

        public static void Save() {
            Save(FileName);
        }

        public static void Initialize() {
            Initialize(FileName);
        }
    }
}
