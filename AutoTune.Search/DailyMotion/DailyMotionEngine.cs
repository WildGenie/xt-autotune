using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace AutoTune.Search.DailyMotion {

    class DailyMotionEngine : SearchEngine {

        const string Fields = "description,embed_url,id,thumbnail_120_url,title,url";
        const string RelatedEndpointFormat = "https://api.dailymotion.com/video/{0}/related?fields={1}&page={2}&limit={3}";
        const string SearchEndpointFormat = "https://api.dailymotion.com/videos/?fields={0}&search={1}&page={2}&limit={3}";

        internal override SearchResults Execute(SearchQuery query, SearchState state) {
            string url;
            string responseString = null;
            int page = state.NextPageToken == null ? 1 : int.Parse(state.NextPageToken);
            if (query.Query != null)
                url = string.Format(SearchEndpointFormat, Fields, Uri.EscapeDataString(query.Query), page, query.PageSize);
            else
                url = string.Format(RelatedEndpointFormat, query.RelatedId, Fields, page, query.PageSize);
            using (WebClient client = new WebClient())
                responseString = client.DownloadString(url);
            var response = JsonConvert.DeserializeObject<DailyMotionResponse>(responseString);
            var newState = new SearchState(response.total, (response.page + 1).ToString());
            var results = response.list.Select(v => new SearchResult {
                VideoId = v.id,
                Title = v.title,
                TypeId = DailyMotionTypeId,
                Description = v.description,
                ThumbnailUrl = v.thumbnail_120_url
            });
            return new SearchResults(newState, results.ToList());
        }
    }
}
