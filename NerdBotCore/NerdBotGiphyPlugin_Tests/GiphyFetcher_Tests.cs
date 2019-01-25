using System;
using System.Threading.Tasks;
using NerdBotGiphyPlugin;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotGiphyPlugin_Tests
{
    [TestFixture]
    public class GiphyFetcher_Tests
    {
        private GiphyFetcher fetcher;
        private UnitTestContext unitTestContext;

        private string giphyUrl = @"https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.gif";

        private string giphydata = @"{
  ""data"": {
    ""images"": {
      ""original"": {
        ""url"": ""https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.gif"",
        ""width"": ""400"",
        ""height"": ""250"",
        ""size"": ""2535859"",
        ""frames"": ""111"",
        ""mp4"": ""https://media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.mp4"",
        ""mp4_size"": ""472711"",
        ""webp"": ""https///media1.giphy.com/media/xT77XZrTKOxycjaYvK/giphy.webp"",
        ""webp_size"": ""3193862""
      }
    }
  }
}";

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            fetcher = new GiphyFetcher(unitTestContext.HttpClientMock.Object, unitTestContext.LoggerMock.Object);
        }

        [Test]
        public void GetGiphyGif()
        {
            string keyword = "cat";

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(giphydata);

            unitTestContext.HttpClientMock.Setup(h => h.GetStringAsync($"http://api.giphy.com/v1/gifs/translate?s={keyword}&api_key=dc6zaTOxFJmzC"))
                .Returns(httpJsonTask.Task);

            string actual = fetcher.GetGifAsync(keyword).Result;

            Assert.AreEqual(giphyUrl, actual);
        }
    }
}
