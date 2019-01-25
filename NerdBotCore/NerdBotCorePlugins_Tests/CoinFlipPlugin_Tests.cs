using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotCorePlugin;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotCorePlugin_Tests
{
    [TestFixture]
    public class CoinFlipPlugin_Tests
    {
        private CoinFlipPlugin plugin;

        private UnitTestContext unitTestContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }
            
        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();
            
            plugin = new CoinFlipPlugin(
                unitTestContext.BotServicesMock.Object);

            plugin.Logger = unitTestContext.LoggerMock.Object;

            plugin.OnLoad();
        }

        [Test]
        public void PerformFlip()
        {
            var cmd = new Command()
            {
                Cmd = "coinflip",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
                plugin.OnCommand(
                    cmd,
                    msg,
                    unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Coin flip"))), Times.Once);
        }
    }
}
