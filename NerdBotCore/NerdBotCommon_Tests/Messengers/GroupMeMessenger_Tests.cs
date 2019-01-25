using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NerdBotCommon;
using NerdBotCommon.Messengers.GroupMe;
using NUnit.Framework;
using Serilog;

namespace NerdBotCommon_Tests.Messengers
{
    [TestFixture]
    class GroupMeMessenger_Tests
    {
        [Test]
        public async Task SendMessage()
        {
            // Arrange
            string url = "https://api.groupme.com/v3/bots/post";

            var httpClientMock = new Mock<IHttpHandler>();
            var loggerMock = new Mock<ILogger>();


            httpClientMock.Setup(c => c.PostAsync(url, It.IsAny<StringContent>()))
                .Returns(Task.FromResult(new HttpResponseMessage()))
                .Verifiable();

            var messenger = new GroupMeMessenger("BOT_ID", "BOT_NAME", new string[] {}, url, httpClientMock.Object, loggerMock.Object);
         
            // Act
            bool actualResult = await messenger.SendMessage("Message here");

            // Assert
            httpClientMock.Verify();
            Assert.True(actualResult);
        }

        [Test]
        public async Task SendMessage_Failure()
        {
            // Arrange
            string url = "https://api.groupme.com/v3/bots/post";

            var httpClientMock = new Mock<IHttpHandler>();
            var loggerMock = new Mock<ILogger>();

            // Mock failure
            httpClientMock.Setup(c => c.PostAsync(url, It.IsAny<StringContent>()))
                .Throws(new HttpRequestException("ERROR"));

            var messenger = new GroupMeMessenger("BOT_ID", "BOT_NAME", new string[] { }, url, httpClientMock.Object, loggerMock.Object);

            // Act
            bool actualResult = await messenger.SendMessage("Message here");

            // Assert
            Assert.False(actualResult);
        }
    }
}
