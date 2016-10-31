using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace AutoTune.Search.Similar {

    public static class SimilarEngine {

        const string Fields = "id,name,similarity,decade,main_genre";
        const string ArtistEndpoint = "http://api.musicgraph.com/api/v2/artist/search?fields={0}&api_key={1}&name={2}";
        const string SimilarEndpoint = "http://api.musicgraph.com/api/v2/artist/{0}/similar?fields={1}&api_key={2}&limit={3}";

        public static List<SimilarResult> Search(string key, string artist, int artistResults, int limit) {
            var result = new List<SimilarResult>();
            string artistUrl = string.Format(ArtistEndpoint, Fields, key, Uri.EscapeDataString(artist));
            var artistResponse = Execute(artistUrl);
            foreach (var response in artistResponse.data.Take(artistResults)) {
                string similarUrl = string.Format(SimilarEndpoint, response.id, Fields, key, limit);
                var similarResponse = Execute(similarUrl);
                result.AddRange(similarResponse.data.Select(r => new SimilarResult {
                    Name = r.name,
                    Similarity = r.similarity
                }));
            }
            return result.OrderByDescending(r => r.Similarity).ToList();
        }

        static Response Execute(string url) {
            Response result;
            using (WebClient client = new WebClient())
                result = JsonConvert.DeserializeObject<Response>(client.DownloadString(url));
            if (result.status.code == 0)
                return result;
            string format = "Similar search returned with status {0}.";
            throw new SearchException(string.Format(format, result.status.message));
        }
    }
}
