using AutoTune.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace AutoTune.Search.DailyMotion {

    class DailyMotionEngine : SearchEngine {

        const string Fields = "description,embed_url,id,thumbnail_120_url,title,url";
        const string RelatedFormat = "https://api.dailymotion.com/video/{0}/related?fields={1}&page={2}&limit={3}";
        const string SearchFormat = "https://api.dailymotion.com/videos/?fields={0}&search={1}&page={2}&limit={3}";

        static List<SearchResult> TransformResponse(DailyMotionResponse response) {
            return response.list.Select(v => new SearchResult {
                VideoId = v.id,
                Title = v.title,
                TypeId = DailyMotionTypeId,
                Description = v.description,
                ThumbnailBase64 = Convert.ToBase64String(Utility.Download(v.thumbnail_120_url))
            }).ToList();
        }

        static DailyMotionResponse ExecuteRequest(SearchQuery q, string currentPage) {
            string url = FormatEndpointUrl(q, currentPage);
            using (WebClient client = new WebClient())
                return JsonConvert.DeserializeObject<DailyMotionResponse>(client.DownloadString(url));
        }

        static string FormatEndpointUrl(SearchQuery query, string page) {
            page = page ?? "1";
            if (query.Query == null)
                return string.Format(RelatedFormat, query.RelatedId, Fields, page, query.PageSize);
            else
                return string.Format(SearchFormat, Fields, Uri.EscapeDataString(query.Query), page, query.PageSize);
        }

        internal override SearchResults Execute(SearchQuery query, string currentPage) {
            var response = ExecuteRequest(query, currentPage);
            return new SearchResults((response.page + 1).ToString(), TransformResponse(response));
        }
    }
}
