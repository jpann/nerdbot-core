using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using NerdBotCommon;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.Factory;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.POCOS;
using NerdBotCommon.UrlShorteners;
using NerdBotHost.PluginManager;
using Serilog;

namespace NerdBot_TestHelper
{
    public class UnitTestContext
    {
        public string BotName { get; set; }
        public string BotId { get; set; }
        public string[] SecretToken { get; set; }
        public string[] SetcretTokenBad { get; set; }
        public BotConfiguration BotConfig { get; set; }
        public Mock<ILogger> LoggerMock { get; set; }
        public Mock<IHttpHandler> HttpClientMock { get; set; }
        public Mock<IPluginManager> PluginManagerMock { get; set; }
        public Mock<IMessenger> MessengerMock { get; set; }
        public Mock<IMessengerFactory> MessengerFactoryMock { get; set; }
        public Mock<ICommandParser> CommandParserMock { get; set; }
        public Mock<IBotServices> BotServicesMock { get; set; }
        public Mock<IUrlShortener> UrlShortenerMock { get; set; }

        public Mock<IConfiguration> ConfigurationMock { get; set; }

        public UnitTestContext()
        {
            Setup();
        }

        public void Setup()
        {
            BotName = "BotName";
            BotId = "BOTID";

            SecretToken = new string[] { "TOKEN" };
            SetcretTokenBad = new string[] { "BADTOKEN" };

            // Setup BotConfig
            BotConfig = new BotConfiguration()
            {
                BotName = BotName,
                Routes = new List<BotRoute>()
                {
                    new BotRoute()
                    {
                        SecretToken = SecretToken[0],
                        BotId = BotId
                    }
                }
            };

            // Setup IConfiguration Mock
            ConfigurationMock = new Mock<IConfiguration>();

            // Setup ILoggingService Mock
            LoggerMock = new Mock<ILogger>();

            // Setup IPluginManager
            PluginManagerMock = new Mock<IPluginManager>();

            // Setup IMessenger Mock
            MessengerMock = new Mock<IMessenger>();

            // Setup IHttpClientMock
            HttpClientMock = new Mock<IHttpHandler>();

            MessengerMock.Setup(p => p.BotName)
                .Returns(BotName);

            MessengerMock.Setup(p => p.BotId)
                .Returns(BotId);

            // Setup IMessengerFactory Mock
            MessengerFactoryMock = new Mock<IMessengerFactory>();

            MessengerFactoryMock.Setup(c => c.Create(SecretToken[0]))
                .Returns(() => MessengerMock.Object);

            // Setup ICommandParser Mock
            CommandParserMock = new Mock<ICommandParser>();

            // Setup IUrlShortener Mock
            UrlShortenerMock = new Mock<IUrlShortener>();

            // Setup IBotServices Mock
            BotServicesMock = new Mock<IBotServices>();

            BotServicesMock.SetupGet(s => s.CommandParser)
                .Returns(CommandParserMock.Object);

            BotServicesMock.SetupGet(s => s.HttpClient)
                .Returns(HttpClientMock.Object);

            BotServicesMock.SetupGet(s => s.UrlShortener)
                .Returns(UrlShortenerMock.Object);
        }
    }
}
