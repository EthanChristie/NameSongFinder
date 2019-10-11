using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyApi.NetCore;
using SpotifyApi.NetCore.Authorization;
using SpotifyApi.NetCore.Http;

namespace NameSongFinder
{
    public class NameSongFinder
    {
        public async Task<string> FindSongForName(string name)
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

            Trace.WriteLine($"Artist.Name = {guyWhoSingsYourNameOverAndOverAgain.Name}");

            var accessToken = await accountsService.GetAccessToken();
            var albumApi = new MyAlbumsApi(httpClient, accessToken);

            var albumsByArtistId = await albumApi.GetAlbumsByArtistId(artistId);

            foreach (var album in albumsByArtistId)
            {
                Trace.WriteLine(album.Name);
            }


            // Get recommendations based on seed Artist Ids
            //var browse = new BrowseApi(http, accounts);
            //RecommendationsResult result = await browse.GetRecommendations(new[] { "1tpXaFf2F55E7kVJON4j4G", "4Z8W4fKeB5YxbusRsdQVPb" }, null, null);
            //string firstTrackName = result.Tracks[0].Name;
            //Trace.WriteLine($"First recommendation = {firstTrackName}");

            //// Page through a list of tracks in a Playlist
            //var playlists = new PlaylistsApi(http, accounts);
            //int limit = 100;
            //PlaylistPaged playlist = await playlists.GetTracks("4h4urfIy5cyCdFOc1Ff4iN", limit: limit);
            //int offset = 0;
            //int j = 0;
            //// using System.Linq
            //while (playlist.Items.Any())
            //{
            //    for (int i = 0; i < playlist.Items.Length; i++)
            //    {
            //        Trace.WriteLine($"Track #{j += 1}: {playlist.Items[i].Track.Artists[0].Name} / {playlist.Items[i].Track.Name}");
            //    }
            //    offset += limit;
            //    playlist = await playlists.GetTracks("4h4urfIy5cyCdFOc1Ff4iN", limit: limit, offset: offset);
            //}

            return guyWhoSingsYourNameOverAndOverAgain.Name;

        }
    }

    public class MyAlbumsApi : AlbumsApi
    {
        public async Task<Album[]> GetAlbumsByArtistId(string artistId, string accessToken = null)
        {
            return await GetModelFromProperty<Album[]>($"{BaseUrl}/artists/{artistId}/albums", "items", accessToken);
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
