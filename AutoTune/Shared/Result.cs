using AutoTune.Settings;
using System;
using System.Xml.Serialization;

namespace AutoTune.Shared {

    public class Result {

        public string Type { get; set; }
        public string Title { get; set; }
        public string VideoId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string DownloadPath { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [XmlIgnore]
        public string Url => string.Format(AppSettings.Searches[Type].UrlPattern, VideoId);
        [XmlIgnore]
        public string PlayUrl => string.Format(AppSettings.Searches[Type].PlayUrlPattern, VideoId);
        [XmlIgnore]
        public string DownloadUrl => string.Format(AppSettings.Searches[Type].DownloadUrlPattern, VideoId);

        public Result NewId() {
            Result result = (Result)MemberwiseClone();
            result.Id = Guid.NewGuid().ToString();
            return result;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj) {
            return ((Result)obj).Id.Equals(Id);
        }
    }
}
