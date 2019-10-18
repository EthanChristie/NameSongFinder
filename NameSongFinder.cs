using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;
using SpotifyApi.NetCore.Helpers;
using SpotifyApi.NetCore.Http;

namespace NameSongFinder
{
    public class NameSongFinder
    {
        public async Task<Track> FindSongForName(string name)
        {
            // HttpClient and AccountsService can be reused. 
            // Tokens are automatically cached and refreshed
            var httpClient = new HttpClient();

            var accountsService = new AccountsService(httpClient);

            // Get an artist by Spotify Artist Id
            var artistsApi = new ArtistsApi(httpClient, accountsService);

            var searchResult = await artistsApi.SearchArtists("The Guy Who Sings Your Name");

            var artists = searchResult.Artists.Items;

            if (!artists.Any())
                throw new Exception("Could not find any artists");

            var guyWhoSingsYourNameOverAndOverAgain = artists.First();

            var artistId = guyWhoSingsYourNameOverAndOverAgain.Id;

            var accessToken = await accountsService.GetAccessToken();
            var albumApi = new MyAlbumsApi(httpClient, accessToken);

            var albumsByArtistId = await albumApi.GetAlbumsByArtistId(artistId);

            foreach (var album in albumsByArtistId)
            {
                var albumTracks = await GetAllAlbumTracks(albumApi, album);

                foreach (var albumTrack in albumTracks)
                {
                    if (albumTrack.Name.Contains(name))
                    {
                        return albumTrack;
                    }
                }
            }

            return new Track();
        }

        private static async Task<Track[]> GetAllAlbumTracks(MyAlbumsApi albumApi, Album album)
        {
            var offset = 0;
            var tracks = new List<Track>();

            while (offset < album.TotalTracks)
            {
                var tracksResponse = await albumApi.GetAlbumTracks(album.Id, 20, offset);
                tracks.AddRange(tracksResponse);
                offset += tracksResponse.Length;
            }

            return tracks.ToArray();
        }
    }

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
            string url = "https://api.spotify.com/v1/albums/" + SpotifyUriHelper.AlbumId(albumId) + "/tracks";
            if (limit.HasValue || !string.IsNullOrEmpty(market))
                url += "?";
            if (limit.HasValue)
                url += string.Format("limit={0}&offset={1}&", (object)limit.Value, (object)offset);
            if (!string.IsNullOrEmpty(market))
                url = url + "market=" + market;
            return await GetModelFromProperty<T>(url, "items", accessToken);
        }

        protected internal virtual async Task<T> NewGetModel<T>(string url, string accessToken = null)
        {
            HttpClient http = this._http;
            string requestUrl = url;
            string parameter = accessToken;
            if (parameter == null)
                parameter = await this.GetAccessToken((string)null);
            var result = await http.Get(requestUrl, new AuthenticationHeaderValue("Bearer", parameter));
            result = "[" + result + "]";
            return JsonConvert.DeserializeObject<T>(result);
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
