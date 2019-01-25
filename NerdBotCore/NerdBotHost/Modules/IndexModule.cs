using Microsoft.Extensions.Configuration;
using Nancy;
using NerdBotCommon;
using NerdBotCommon.Messengers;
using Serilog;

namespace NerdBotHost.Modules
{
    public class IndexModule : NancyModule
    {
        private readonly ILogger _logger;
        private readonly IHttpHandler _httpClient;
        private readonly IConfiguration _config;

        public IndexModule(
            IConfiguration config, 
            IHttpHandler httpHandler,
            ILogger logger)
        {
            _logger = logger;
            _httpClient = httpHandler;
            _config = config;

            Get("/", args =>
            {
                var id = _config["NERD_BOTID"];

                _logger.Information("ROOT");

                return "Hello World";
            });

            Get("/test/{text}", args =>
            {
                _logger.Information($"test = {args.text}");

                return Response.AsJson(new
                {
                    Text = args.text
                });
            });
        }
    }
}
