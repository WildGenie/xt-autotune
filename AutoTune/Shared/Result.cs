using System;

namespace AutoTune.Shared {

    public class Result {

        public string Type { get; set; }
        public string Title { get; set; }
        public string PlayUrl { get; set; }
        public string FileName { get; set; }
        public bool KeepOriginal { get; set; }
        public string DownloadUrl { get; set; }
        public string Description { get; set; }
        public string DownloadPath { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool ShouldPostProcess { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();

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
