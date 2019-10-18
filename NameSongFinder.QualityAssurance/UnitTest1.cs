using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;

namespace NameSongFinder.QualityAssurance
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task Test1()
        {
            var nameSongFinder = new NameSongFinder();

            var track = await nameSongFinder.FindSongForName("Chris");

            Assert.AreEqual("The Chris Song", track.Name);
        }
    }
}