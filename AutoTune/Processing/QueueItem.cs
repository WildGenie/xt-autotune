using AutoTune.Search;
using System;
using YAXLib;

namespace AutoTune.Processing {

    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
    internal class QueueItem {

        internal SearchResult Search { get; set; }
        internal string DownloadPath { get; set; }
        internal string BaseFileName { get; set; }
        internal string Id { get; set; } = Guid.NewGuid().ToString();

        public QueueItem() {
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
