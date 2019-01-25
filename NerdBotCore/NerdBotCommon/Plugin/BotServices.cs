using System;
using NerdBotCommon.Parsers;
using NerdBotCommon.POCOS;
using NerdBotCommon.UrlShorteners;
using Serilog;

namespace NerdBotCommon.Plugin
{
    public class BotServices : IBotServices
    {
        private readonly ICommandParser _commandParser;
        private readonly IHttpHandler _httpClient;
        private readonly IUrlShortener _urlShortener;
        private readonly BotConfiguration _botConfiguration;
        private readonly ILogger _logger;

        #region Properties
        public ICommandParser CommandParser
        {
            get { return this._commandParser; }
        }

        public IHttpHandler HttpClient
        {
            get { return this._httpClient; }
        }

        public IUrlShortener UrlShortener
        {
            get { return this._urlShortener; }
        }

        public BotConfiguration BotConfig
        {
            get { return this._botConfiguration; }
        }

        public ILogger Logger
        {
            get { return this._logger; }
        }
        #endregion

        public BotServices(
            ICommandParser commandParser,
            IHttpHandler httpClient,
            IUrlShortener urlShortener,
            BotConfiguration botConfiguration,
            ILogger logger)
        {
            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (httpClient == null)
                throw new ArgumentNullException("httpClient");

            if (urlShortener == null)
                throw new ArgumentNullException("urlShortener");

            if (botConfiguration == null)
                throw new ArgumentNullException("botConfiguration");

            if (logger == null)
                throw new ArgumentNullException("logger");

            this._commandParser = commandParser;
            this._httpClient = httpClient;
            this._urlShortener = urlShortener;
            this._botConfiguration = botConfiguration;
            this._logger = logger;
        }
    }
}
