using AutoTune.Settings;
using AutoTune.Shared;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace AutoTune.Drivers.DailyMotion {

    class DailyMotionSearch : Search {

        const string Fields = "description,embed_url,id,thumbnail_120_url,title,url";
        const string RelatedEndpointFormat = "https://api.dailymotion.com/video/{0}/related?fields={1}&page={2}&limit={3}";
        const string SearchEndpointFormat = "https://api.dailymotion.com/videos/?fields={0}&search={1}&page={2}&limit={3}";

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            string url;
            string responseString = null;
            var app = AppSettings.Instance;
            int page = pageToken == null ? 1 : int.Parse(pageToken);
            if (query != null)
                url = string.Format(SearchEndpointFormat, Fields, Uri.EscapeDataString(query), page, app.SearchPageSize);
            else
                url = string.Format(RelatedEndpointFormat, similarTo.VideoId, Fields, page, app.SearchPageSize);
            using (WebClient client = new WebClient())
                responseString = client.DownloadString(url);
            var response = JsonConvert.DeserializeObject<DailyMotionResponse>(responseString);
            return new SearchResult {
                State = new SearchState {
                    TotalResults = response.total,
                    NextPageToken = (response.page + 1).ToString()
                },
                Results = response.list.Select(v => new Result {
                    VideoId = v.id,
                    Title = v.title,
                    Description = v.description,
                    ThumbnailUrl = v.thumbnail_120_url
                })
            };
        }
    }
}
