using Newtonsoft.Json;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace AutoTune.Search.Suggest {

    public static class SuggestionEngine {

        const string Fields = "id,name,similarity,decade,main_genre";
        const string ArtistEndpoint = "http://api.musicgraph.com/api/v2/artist/search?fields={0}&api_key={1}&name={2}";
        const string SimilarEndpoint = "http://api.musicgraph.com/api/v2/artist/{0}/similar?fields={1}&api_key={2}";

        public static List<SuggestionResult> Search(string key, string artist, int artistResults) {
            var result = new List<SuggestionResult>();
            string artistUrl = string.Format(ArtistEndpoint, Fields, key, Uri.EscapeDataString(artist));
            var artistResponse = Execute(artistUrl);
            foreach (var response in artistResponse.data.Take(artistResults)) {
                string similarUrl = string.Format(SimilarEndpoint, response.id, Fields, key);
                var similarResponse = Execute(similarUrl);
                result.AddRange(similarResponse.data.Select(r => new SuggestionResult {
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
            string format = "Suggestion search returned with status {0}.";
            throw new SearchException(string.Format(format, result.status.message));
        }
    }
}
