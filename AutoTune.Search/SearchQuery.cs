using AutoTune.Shared;
using System.Collections.Generic;

namespace AutoTune.Search {

    public class SearchQuery {

        public bool? Local { get; }
        public int PageSize { get; }
        public string Query { get; }
        public bool Favourite { get; }
        public string RelatedId { get; }
        public IDictionary<string, SearchCredentials> Credentials { get;  }

        public SearchQuery(string typeId, SearchCredentials credentials, string relatedId, int pageSize) {
            PageSize = pageSize;
            RelatedId = relatedId;
            Credentials = new Dictionary<string, SearchCredentials> { { typeId, credentials } };
        }

        public SearchQuery(IDictionary<string, SearchCredentials> credentials, string query, bool favourite, bool? local, int pageSize) {
            Query = query;
            Local = local;
            PageSize = pageSize;
            Favourite = favourite;
            Credentials = credentials;
        }
    }
}
