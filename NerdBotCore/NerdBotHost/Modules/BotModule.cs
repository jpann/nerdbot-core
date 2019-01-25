using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.ModelBinding;
using NerdBotCommon;
using NerdBotCommon.Extensions;
using NerdBotCommon.Messengers.Factory;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotCommon.POCOS;
using NerdBotHost.PluginManager;
using Serilog;

namespace NerdBotHost.Modules
{
    public class BotModule : NancyModule
    {
        private readonly ILogger _logger;
        private readonly IHttpHandler _httpClient;
        private readonly IConfiguration _config;
        private readonly IMessengerFactory _messengerFactory;
        private readonly BotConfiguration _botConfiguration;

        public BotModule(
            IConfiguration config,
            IHttpHandler httpHandler,
            ILogger logger,
            ICommandParser commandParser,
            IMessengerFactory messengerFactory,
            IPluginManager pluginManager,
            BotConfiguration botConfiguration) : base("/bot")
        {
            _logger = logger;
            _httpClient = httpHandler;
            _config = config;
            _messengerFactory = messengerFactory;
            _botConfiguration = botConfiguration;

            Get("/", _ => { return HttpStatusCode.Accepted; });

            Post("/{token}", async parameters =>
            {
                try
                {
                    string sentToken = parameters.token;

                    if (_botConfiguration.Routes.FirstOrDefault(route => route.SecretToken == sentToken) == null)
                    {
                        _logger.Information($"Invalid token sent from {this.Request.UserHostAddress}. Token = {sentToken}");

                        return HttpStatusCode.NotAcceptable;
                    }

                    var message = new GroupMeMessage();

                    // Bind and validate the request to GroupMeMessage
                    var msg = this.BindToAndValidate(message);

                    if (!ModelValidationResult.IsValid)
                    {
                        _logger.Information($"Invalid message sent from {this.Request.UserHostAddress}.");

                        return HttpStatusCode.NotAcceptable;
                    }

                    // Don't handle messages sent from ourself
                    if (message.name.ToLower() == _botConfiguration.BotName.ToLower())
                        return HttpStatusCode.NotAcceptable;

                    if (string.IsNullOrEmpty(message.text))
                    {
                        return HttpStatusCode.NotAcceptable;
                    }

                    // Get messenger for this token
                    var messenger = _messengerFactory.Create(sentToken);
                    if (messenger == null)
                        return HttpStatusCode.InternalServerError;

                    // Parse the command
                    var command = commandParser.Parse(message.text);
                    if (command != null)
                    {
                        if (!string.IsNullOrEmpty(command.Cmd))
                        {
                            _logger.Debug($"Received command '{command.Cmd}'");

                            if (command.Cmd.ToLower() == "help")
                            {
                                bool helpHandled = await pluginManager.HandledHelpCommandAsync(command, messenger);
                            }
                            else
                            {
                                bool handled = await pluginManager.HandleCommandAsync(command, message, messenger);
                            }
                        }
                    }

                    return HttpStatusCode.Accepted;
                }
                catch (Exception e)
                {
                    _logger.Here().Error(e, "Bot route error");

                    return HttpStatusCode.InternalServerError;
                }
            });
        }
    }
}
