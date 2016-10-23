using AutoTune.Search;
using System;

namespace AutoTune.Processing {

    public class QueueItem {

        public SearchResult Search { get; set; }
        public string DownloadPath { get; set; }
        public string BaseFileName { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();

        internal QueueItem() {
        }

        internal QueueItem(SearchResult search) {
            Search = search;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        internal QueueItem NewId() {
            QueueItem result = new QueueItem();
            result.Search = Search;
            result.DownloadPath = DownloadPath;
            result.BaseFileName = BaseFileName;
            return result;
        }

        public override bool Equals(object obj) {
            return Id.Equals(((QueueItem)obj).Id);
        }
    }
}
