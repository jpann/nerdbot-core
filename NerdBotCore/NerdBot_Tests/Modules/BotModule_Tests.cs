using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Nancy;
using Nancy.Testing;
using NerdBotCommon;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.Factory;
using NerdBotCommon.Parsers;
using NerdBotCommon.POCOS;
using NerdBotHost.Modules;
using NerdBotHost.PluginManager;
using NerdBot_TestHelper;
using NUnit.Framework;
using Serilog;

namespace NerdBot_Tests.Modules
{
    [TestFixture]
    class BotModule_Tests
    {
        private Browser browserGoodToken;

        private UnitTestContext unitTestContext;

        private string[] secretTokenGood = new string[] { "TOKEN" };
        private string[] secretTokenBad = new string[] { "BADTOKEN" };

        private void SetUp_Data()
        {
            
        }

        [SetUp]
        public void SetUp()
        {
            SetUp_Data();

            unitTestContext = new UnitTestContext();

            // ICommandParser Mocks
            unitTestContext.CommandParserMock.Setup(c => c.Parse("img boros charm"))
                .Returns(() => new Command()
                {
                    Cmd = "img",
                    Arguments = new string[] { "boros charm" }
                });

            unitTestContext.CommandParserMock.Setup(c => c.Parse("help img"))
                .Returns(() => new Command()
                {
                    Cmd = "help",
                    Arguments = new string[] { "img" }
                });

            // Setup the Browser object
            browserGoodToken = new Browser(with =>
            {
                with.Module<BotModule>();
                with.Dependency<IMessengerFactory>(unitTestContext.MessengerFactoryMock.Object);
                with.Dependency<IPluginManager>(unitTestContext.PluginManagerMock.Object);
                with.Dependency<ICommandParser>(unitTestContext.CommandParserMock.Object);
                with.Dependency<IHttpHandler>(unitTestContext.HttpClientMock.Object);
                with.Dependency<ILogger>(unitTestContext.LoggerMock.Object);
                with.Dependency<BotConfiguration>(unitTestContext.BotConfig);
                with.Dependency<IConfiguration>(unitTestContext.ConfigurationMock.Object);
            });
        }

        [Test]
        public void ValidMessage()
        {
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""group_id"":""9999999"",
""name"":""User Name"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""text"":""img boros charm"",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenGood[0],
            with =>
            {
                with.HttpRequest();
                with.Body(groupMeMessageBody);
                with.Header("content-type", "application/json");
            });

            // Verify that the command parser was given this message to try and parse
            unitTestContext.CommandParserMock.Verify(c => c.Parse("img boros charm"));

            Assert.AreEqual(HttpStatusCode.Accepted, response.Result.StatusCode);
        }

        [Test]
        public void ValidMessage_FromBotName()
        {
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""group_id"":""9999999"",
""name"":""BotName"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""text"":""Bot Message"",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenGood[0],
                with =>
                {
                    with.HttpRequest();
                    with.Body(groupMeMessageBody);
                    with.Header("content-type", "application/json");
                });

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.Result.StatusCode);
        }

        [Test]
        public void ValidMessage_EmptyText()
        {
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""group_id"":""9999999"",
""name"":""User Name"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""text"":"""",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenGood[0],
                with =>
                {
                    with.HttpRequest();
                    with.Body(groupMeMessageBody);
                    with.Header("content-type", "application/json");
                });

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.Result.StatusCode);
        }

        [Test]
        public void ValidMessage_HelpCommand()
        {
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""group_id"":""9999999"",
""name"":""User Name"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""text"":""help img"",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenGood[0],
                with =>
                {
                    with.HttpRequest();
                    with.Body(groupMeMessageBody);
                    with.Header("content-type", "application/json");
                });

            unitTestContext.PluginManagerMock.Verify(c => c.HandledHelpCommandAsync(It.Is<Command>(cmd => cmd.Cmd == "help" && cmd.Arguments[0] == "img"), It.IsAny<IMessenger>()));

            Assert.AreEqual(HttpStatusCode.Accepted, response.Result.StatusCode);
        }

        [Test]
        public void InvalidMessage()
        {
            // This message body is missing the 'groupme_id' and 'text' properties
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""name"":""User Name"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenGood[0],
            with =>
            {
                with.HttpRequest();
                with.Body(groupMeMessageBody);
                with.Header("content-type", "application/json");
            });

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.Result.StatusCode);
        }

        [Test]
        public void InvalidToken()
        {
            string groupMeMessageBody = @"{
""id"":""141909488216484256"",
""source_guid"":""b4182bb58a18ba162b29434"",
""created_at"":1419094882,
""user_id"":""111111"",
""group_id"":""9999999"",
""name"":""User Name"",
""avatar_url"":""https://i.groupme.com/668x401.jpeg"",
""text"":""This is a test message?"",
""system"":false,
""attachments"":[
]
}";

            var response = browserGoodToken.Post("/bot/" + secretTokenBad[0],
            with =>
            {
                with.HttpRequest();
                with.Body(groupMeMessageBody);
                with.Header("content-type", "application/json");
            });

            Assert.AreEqual(HttpStatusCode.NotAcceptable, response.Result.StatusCode);
        }
    }
}
