using System.Collections.Generic;

namespace AutoTune.Search {

    public class SearchQuery {

        public int PageSize { get; }
        public string Query { get; }
        public string RelatedId { get; }
        public IDictionary<string, SearchCredentials> Credentials { get;  }

        public SearchQuery(IDictionary<string, SearchCredentials> credentials, string query, int pageSize) {
            Query = query;
            PageSize = pageSize;
            Credentials = credentials;
        }

        public SearchQuery(string typeId, SearchCredentials credentials, string relatedId, int pageSize) {
            PageSize = pageSize;
            RelatedId = relatedId;
            Credentials = new Dictionary<string, SearchCredentials> { { typeId, credentials } };
        }
    }
}
