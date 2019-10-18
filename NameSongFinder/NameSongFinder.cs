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
    public class NameSongFinder : INameSongFinder
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
}
