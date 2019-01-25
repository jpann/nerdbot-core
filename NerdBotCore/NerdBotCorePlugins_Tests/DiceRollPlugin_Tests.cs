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
    public class DiceRollPlugin_Tests
    {
        private DiceRollPlugin plugin;

        private UnitTestContext unitTestContext;
            
        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            plugin = new DiceRollPlugin(unitTestContext.BotServicesMock.Object);

            plugin.Logger = unitTestContext.LoggerMock.Object;

            plugin.OnLoad();
        }

        [Test]
        public void Roll_NoArguments()
        {
            var cmd = new Command()
            {
                Cmd = "roll",
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

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Roll 1-6"))), Times.Once);
        }

        [Test]
        public void Roll_Argument()
        {
            var cmd = new Command()
            {
                Cmd = "roll",
                Arguments = new string[]
                {
                    "20"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Roll 1-20"))), Times.Once);
        }

        [Test]
        public void Roll_Argument_MultipleDice()
        {
            var cmd = new Command()
            {
                Cmd = "roll",
                Arguments = new string[]
                {
                    "20 x2"
                }
            };

            var msg = new GroupMeMessage();

            bool handled =
            plugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
                ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Roll 1-20 x2"))), Times.Once);
        }
    }
}
