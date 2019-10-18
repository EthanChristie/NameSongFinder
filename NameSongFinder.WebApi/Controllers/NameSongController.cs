using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SpotifyApi.NetCore;

namespace NameSongFinder.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NameSongController : ControllerBase
    {
        private readonly INameSongFinder _nameSongFinder;

        public NameSongController(INameSongFinder nameSongFinder)
        {
            _nameSongFinder = nameSongFinder;
        }

        [HttpGet]
        public IActionResult Get(string name)
        {
            var track = _nameSongFinder.FindSongForName(name);
            return Ok(track);
        }
    }
}