using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotRoastMePlugin_Tests
{
    [TestFixture]
    class NerdBotRoastMePlugins_Tests
    {
        private IHttpHandler httpClient;
        private UnitTestContext unitTestContext;
        private NerdBotRoastMePlugin.NerdBotRoastMePlugin plugin;

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            httpClient = new HttpClientHandler();

            unitTestContext.BotServicesMock.SetupGet(b => b.HttpClient)
                .Returns(httpClient);

            plugin = new NerdBotRoastMePlugin.NerdBotRoastMePlugin(unitTestContext.BotServicesMock.Object);

            plugin.BotName = unitTestContext.BotName;

            plugin.Logger = unitTestContext.LoggerMock.Object;
            plugin.OnLoad();
        }
        [Test]
        public void RoastMe()
        {
            var msg = new GroupMeMessage();
            msg.text = "hey, roast me!";
            msg.name = "JohnDoe";

            bool handled =
                plugin.OnMessage(
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessageWithMention(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.AtLeastOnce);
        }

        [Test]
        public void RoastMe_FromBot()
        {
            var msg = new GroupMeMessage();
            msg.text = "hey, roast me!";
            msg.name = unitTestContext.BotName;

            bool handled =
                plugin.OnMessage(
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => 
                    m.SendMessageWithMention(
                        It.IsAny<string>(), 
                        It.IsAny<string>(), 
                        It.IsAny<int>(), 
                        It.IsAny<int>()), 
                Times.Never);
        }
    }
}
