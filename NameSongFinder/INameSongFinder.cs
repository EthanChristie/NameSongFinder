using System.Threading.Tasks;
using SpotifyApi.NetCore;

namespace NameSongFinder
{
    public interface INameSongFinder
    {
        Task<Track> FindSongForName(string name);
    }
}