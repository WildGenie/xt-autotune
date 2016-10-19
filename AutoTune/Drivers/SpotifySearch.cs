using AutoTune.Settings;
using AutoTune.Shared;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using System.Linq;

namespace AutoTune.Drivers {

    class SpotifySearch : Search {

        public const string TypeId = "Spotify";

        protected override SearchResult Execute(int totalResults, string pageToken, string query, Result similarTo) {
            var app = AppSettings.Instance;
            var user = UserSettings.Instance;
            var auth = new ClientCredentialsAuth();
            auth.ClientId = user.SpotifyId;
            auth.ClientSecret = user.SpotifySecret;
            auth.Scope = Scope.UserReadPrivate;
            var token = auth.DoAuth();
            var spotify = new SpotifyWebAPI();
            spotify.TokenType = token.TokenType;
            spotify.AccessToken = token.AccessToken;
            spotify.UseAuth = false;
            var response = spotify.SearchItems(query, SearchType.Track, app.SearchPageSize, 0);
            return new SearchResult {
                State = new SearchState {
                    TotalResults = 0,
                    NextPageToken = null
                },
                Results = response.Tracks.Items.Select(i => new Result {
                    Type = TypeId,
                    VideoId = i.Id,
                    Title = i.Name,
                    Description = i.Album.Name,
                    ThumbnailUrl = i.Album.Images[0].Url,
                    Url = string.Format(app.Spotify.UrlPattern, i.ExternUrls["spotify"]),
                    PlayUrl = string.Format(app.Spotify.PlayUrlPattern, i.ExternUrls["spotify"]),
                    DownloadUrl = string.Format(app.Spotify.DownloadUrlPattern, i.ExternUrls["spotify"])
                })
            };
        }
    }
}
