using System.Collections.Generic;
using System.Linq;

namespace AutoTune.Search.Vimeo {

    internal class VimeoEngine : SearchEngine {

        internal override SearchResults Execute(SearchQuery query, SearchState state) {
            SearchState newState;
            var credentials = query.Credentials[VimeoTypeId];
            var client = new VimeoVideoClient(credentials.Key, credentials.Secret);
            int? page = state.NextPageToken == null ? (int?)null : int.Parse(state.NextPageToken);
            if (state.TotalResults >= 0 && page != null && page == -1 || (page - 1) * query.PageSize >= state.TotalResults) {
                newState = new SearchState(0, "-1");
                return new SearchResults(newState, new List<SearchResult>());
            }
            var videos = client.GetVideos(query.Query, query.RelatedId, page, query.PageSize);
            newState = new SearchState(videos.total, (videos.page + 1).ToString());
            var results = videos.data.Select(v => new SearchResult {
                Title = v.name,
                TypeId = VimeoTypeId,
                VideoId = v.id?.ToString(),
                Description = v.description,
                ThumbnailUrl = v.pictures.Any() && v.pictures[0].sizes.Any() ? v.pictures[0].sizes[0].link : null
            });
            return new SearchResults(newState, results.ToList());
        }
    }
}
