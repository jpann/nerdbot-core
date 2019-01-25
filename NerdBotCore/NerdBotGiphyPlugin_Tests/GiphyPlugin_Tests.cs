using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotGiphyPlugin;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotGiphyPlugin_Tests
{
    [TestFixture]
    class GiphyPlugin_Tests
    {
        private GiphyPlugin plugin;
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

            plugin = new GiphyPlugin(unitTestContext.BotServicesMock.Object);

            plugin.Logger = unitTestContext.LoggerMock.Object;
            plugin.OnLoad();
        }

        [Test]
        public void Giphy_Call()
        {
            string keyword = "geek";

            var cmd = new Command()
            {
                Cmd = "giphy",
                Arguments = new string[]
                {
                    keyword
                }
            };

            var httpJsonTask = new TaskCompletionSource<string>();
            httpJsonTask.SetResult(giphydata);

            unitTestContext.HttpClientMock.Setup(h => h.GetStringAsync($"http://api.giphy.com/v1/gifs/translate?s={keyword}&api_key=dc6zaTOxFJmzC"))
                .Returns(httpJsonTask.Task);

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.EndsWith("giphy.gif"))), Times.AtLeastOnce);
        }
    }
}
