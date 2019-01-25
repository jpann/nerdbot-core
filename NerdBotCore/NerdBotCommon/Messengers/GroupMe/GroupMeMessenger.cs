using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Nancy.Json;
using Serilog;

namespace NerdBotCommon.Messengers.GroupMe
{
    public class GroupMeMessenger : IMessenger
    {
        private readonly IHttpHandler _httpClient;
        private readonly ILogger _logger;
        private readonly string _botId;
        private readonly string _botName;
        private readonly string _endpointUrl;
        private readonly string[] _ignoreNames;

        #region Properties
        public string BotId
        {
            get { return this._botId; }
        }

        public string BotName
        {
            get { return this._botName; }
        }

        public string[] IgnoreNames
        {
            get { return this._ignoreNames; }
        }
        #endregion

        public GroupMeMessenger(
            string botId,
            string botName,
            string[] ignoreNames,
            string endPointUrl,
            IHttpHandler httpClient,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(botId))
                throw new ArgumentException("botId");

            if (string.IsNullOrEmpty(botName))
                throw new ArgumentException("botName");

            if (string.IsNullOrEmpty(endPointUrl))
                throw new ArgumentException("endPointUrl");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this._botId = botId;
            this._botName = botName;
            this._ignoreNames = ignoreNames;
            this._endpointUrl = endPointUrl;
            this._httpClient = httpClient ?? throw new ArgumentNullException("httpClient");
            this._logger = logger;
        }

        public async Task<bool> SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            string json = new JavaScriptSerializer().Serialize(new
            {
                text = message,
                bot_id = this._botId
            });

            try
            {
                this._logger.Verbose($"Sending message '{message}' using botId '{this._botId}'...");

                await this._httpClient.PostAsync(this._endpointUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                return true;
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"Error sending groupme message: {message}");

                return false;
            }
        }

        public async Task<bool> SendMessageWithMention(string message, string mentionId, int start, int end)
        {
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("message");

            string json = new JavaScriptSerializer().Serialize(new
            {
                text = message,
                bot_id = this._botId,
                attachments = new [] {
                    new
                    {
                        loci = new[]
                        {
                            new []
                            {
                                start, end
                            }
                        },
                        type = "mentions",
                        user_ids = new []
                        {
                            mentionId
                        }
                    }
                }
            });

            try
            {
                this._logger.Verbose($"Sending message '{message}' using botId '{this._botId}'...");

                await this._httpClient.PostAsync(this._endpointUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                return true;
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"Error sending groupme message: {message}");

                return false;
            }
        }
    }
}
