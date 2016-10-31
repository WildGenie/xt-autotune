using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using VimeoDotNet;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace AutoTune.Search.Vimeo {

    class VimeoVideoClient : VimeoClient {

        internal VimeoVideoClient(string clientId, string clientSecret) :
            base(clientId, clientSecret) {
        }

        internal Paginated<Video> GetVideos(SearchQuery query, string currentPage) {
            var request = currentPage != null ? GenerateVideosRequest(currentPage) : GenerateVideosRequest(query);
            var response = request.ExecuteRequest<Paginated<Video>>();
            UpdateRateLimit(response);
            CheckStatusCodeError(response, "Error retrieving videos.");
            return response.Data;
        }

        bool IsSuccessStatusCode(HttpStatusCode statusCode) {
            int code = (int)statusCode;
            return code >= 200 && code < 300;
        }

        IApiRequest GenerateVideosRequest(string path) {
            var result = ApiRequestFactory.GetApiRequest(ClientId, ClientSecret);
            result.Method = Method.GET;
            result.Path = path;
            return result;
        }

        void UpdateRateLimit(IRestResponse response) {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            typeof(VimeoClient).GetField("_headers", flags).SetValue(this, response.Headers);
        }

        IApiRequest GenerateVideosRequest(SearchQuery query) {
            string path = query.Query != null ? "/categories/music/videos" : "/videos/" + query.RelatedId + "/videos";
            var request = GenerateVideosRequest(path);
            request.Query.Add("sort", "relevant");
            request.Query.Add("categories", Uri.EscapeDataString("[music]"));
            request.Query.Add("per_page", query.PageSize.ToString());
            if (query.Query != null)
                request.Query.Add("query", query.Query);
            else
                request.Query.Add("filter", "related");
            return request;
        }

        void CheckStatusCodeError(IRestResponse response, string message, params HttpStatusCode[] validStatusCodes) {
            if (validStatusCodes == null ||
                IsSuccessStatusCode(response.StatusCode) ||
                validStatusCodes.Contains(response.StatusCode))
                return;
            string format = "{1}{0}Code: {2}{0}Message: {3}";
            string formatted = string.Format(format, Environment.NewLine, message, response.StatusCode, response.Content);
            throw new SearchException(formatted);
        }
    }
}
