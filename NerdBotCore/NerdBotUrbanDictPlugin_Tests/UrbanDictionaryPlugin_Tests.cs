using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotUrbanDictPlugin;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotUrbanDictPlugin_Tests
{
    [TestFixture]
    public class UrbanDictionaryPlugin_Tests
    {
        private UrbanDictionaryPlugin plugin;
        private IHttpHandler httpClient;
        private UnitTestContext unitTestContext;

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();


            httpClient = new HttpClientHandler();

            unitTestContext.BotServicesMock.SetupGet(b => b.HttpClient)
                .Returns(httpClient);

            plugin = new UrbanDictionaryPlugin(unitTestContext.BotServicesMock.Object);

            plugin.Logger = unitTestContext.LoggerMock.Object;
        }

        [Test]
        public void GetDefinition_IsA()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is a butt"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_IsAn()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is an butt"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_Is()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is butt"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetDefinition_NoDefinition()
        {
            var cmd = new Command()
            {
                Cmd = "wtf",
                Arguments = new string[]
                {
                    "is wertyui"   
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("There is no definition for that"), Times.AtLeastOnce);
        }
    }
}
