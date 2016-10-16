﻿using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using VimeoDotNet;
using VimeoDotNet.Models;
using VimeoDotNet.Net;

namespace AutoTune.Drivers {

    class VimeoVideoClient : VimeoClient {

        public VimeoVideoClient(string clientId, string clientSecret) :
            base(clientId, clientSecret) {
        }

        public Paginated<Video> GetVideos(string query, string videoId, int? page, int? perPage) {
            try {
                var result = GetVideosAsync(query, videoId, page, perPage);
                result.Wait();
                return result.Result;
            } catch (AggregateException ex) {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;
            }
        }

        public async Task<Paginated<Video>> GetVideosAsync(string query, string videoId, int? page, int? perPage) {
            try {
                IApiRequest request = GenerateVideosRequest(query, videoId, page, perPage);
                IRestResponse<Paginated<Video>> response = await request.ExecuteRequestAsync<Paginated<Video>>();
                UpdateRateLimit(response);
                CheckStatusCodeError(response, "Error retrieving videos.");
                return response.Data;
            } catch (Exception ex) {
                throw new DriverException("Error retrieving videos.", ex);
            }
        }

        bool IsSuccessStatusCode(HttpStatusCode statusCode) {
            var code = (int)statusCode;
            return code >= 200 && code < 300;
        }

        void UpdateRateLimit(IRestResponse response) {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            typeof(VimeoClient).GetField("_headers", flags).SetValue(this, response.Headers);
        }

        IApiRequest GenerateVideosRequest(string query, string videoId, int? page, int? perPage) {
            IApiRequest request = ApiRequestFactory.GetApiRequest(ClientId, ClientSecret);
            request.Method = Method.GET;
            request.Path = query != null ? "/videos" : "/videos/" + videoId + "/videos";
            if (page.HasValue)
                request.Query.Add("page", page.ToString());
            if (perPage.HasValue)
                request.Query.Add("per_page", perPage.ToString());
            if (query != null)
                request.Query.Add("query", query);
            if (videoId != null)
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
            throw new DriverException(formatted);
        }
    }
}