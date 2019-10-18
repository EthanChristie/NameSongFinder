using System.Net.Http;
using System.Threading.Tasks;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;
using SpotifyApi.NetCore.Helpers;

namespace NameSongFinder
{
    public class MyAlbumsApi : AlbumsApi
    {
        public async Task<Album[]> GetAlbumsByArtistId(string artistId, string accessToken = null)
        {
            return await GetModelFromProperty<Album[]>($"{BaseUrl}/artists/{artistId}/albums", "items", accessToken);
        }

        public new async Task<Track[]> GetAlbumTracks(
            string albumId,
            int? limit = null,
            int offset = 0,
            string market = null,
            string accessToken = null)
            => await NewGetAlbumTracks<Track[]>(albumId, limit: limit, offset: offset, market: market, accessToken: accessToken);

        public async Task<T> NewGetAlbumTracks<T>(
            string albumId,
            int? limit = null,
            int offset = 0,
            string market = null,
            string accessToken = null)
        {
            var url = "https://api.spotify.com/v1/albums/" + SpotifyUriHelper.AlbumId(albumId) + "/tracks";
            if (limit.HasValue || !string.IsNullOrEmpty(market))
                url += "?";
            if (limit.HasValue)
                url += $"limit={limit.Value}&offset={offset}&";
            if (!string.IsNullOrEmpty(market))
                url = url + "market=" + market;
            return await GetModelFromProperty<T>(url, "items", accessToken);
        }

        public MyAlbumsApi(HttpClient httpClient) : base(httpClient)
        {
        }

        public MyAlbumsApi(HttpClient httpClient, string accessToken) : base(httpClient, accessToken)
        {
        }

        public MyAlbumsApi(HttpClient httpClient, IAccessTokenProvider accessTokenProvider) : base(httpClient, accessTokenProvider)
        {
            
        }
    }
}