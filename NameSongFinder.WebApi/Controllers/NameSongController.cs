using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SpotifyApi.NetCore;

namespace NameSongFinder.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NameSongController : ControllerBase
    {
        [HttpGet]
        [Route("{name?}")]
        public async Task<Track> Get(string name = "Chris")
        {
            var nameSongFinder = new NameSongFinder();
            return await nameSongFinder.FindSongForName(name);
        }
    }
}
