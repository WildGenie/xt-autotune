using AutoTune.Settings;
using AutoTune.Shared;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace AutoTune.Drivers {

    class DailyMotionSearch : Search {

        public const string TypeId = "DailyMotion";
        const string Fields = "description,embed_url,id,thumbnail_120_url,title,url";
        const string EndpointFormat = "https://api.dailymotion.com/videos/?fields={0}&search={1}";

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            string responseString = null;
            var app = AppSettings.Instance;
            using (WebClient client = new WebClient()) {
                string url = string.Format(EndpointFormat, Fields, Uri.EscapeDataString(query));
                responseString = client.DownloadString(url);
            }
            var response = JsonConvert.DeserializeObject<DailyMotionResponse>(responseString);
            return new SearchResult {
                State = new SearchState {
                    TotalResults = response.total,
                    NextPageToken = (response.page + 1).ToString()
                },
                Results = response.list.Select(v => new Result {
                    VideoId = v.id,
                    Type = TypeId,
                    Title = v.title,
                    Description = v.description,
                    ThumbnailUrl = v.thumbnail_120_url,
                    Url = string.Format(app.DailyMotion.UrlPattern, v.embed_url),
                    PlayUrl = string.Format(app.DailyMotion.PlayUrlPattern, v.embed_url),
                    DownloadUrl = string.Format(app.DailyMotion.DownloadUrlPattern, v.url)
                })
            };
        }
    }
}
