using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using NerdBotCommon;
using NerdBotCommon.Http;
using NerdBotCommon.Messengers;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.POCOS;
using NerdBotCommon.UrlShorteners;
using NerdBotHost.PluginManager;
using Serilog;

namespace NerdBotHost
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public Bootstrapper(IConfiguration configuration, ILogger logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            container.Register(this._logger);
            container.Register(this._configuration);
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            container.Register<IHttpHandler, HttpClientHandler>();
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);

            container.Register(this._logger);

            // Configurations
            string[] msgrBotId;
            string msgrBotName;
            string msgrEndPointUrl;
            string[] msgrIgnoreNames;
            string[] botRouteToken;
            string urlUser;
            string urlKey;

            // Load configurations
            LoadConfiguration(
                out msgrBotId,
                out msgrBotName,
                out msgrEndPointUrl,
                out msgrIgnoreNames,
                out botRouteToken,
                out urlUser,
                out urlKey
            );

            // Register the instance of IUrlShortener
            var bitlyUrl = new BitlyShorten(
                urlUser,
                urlKey,
                container.Resolve<IHttpHandler>(),
                this._logger);
            container.Register<IUrlShortener>(bitlyUrl);

            // Register the instance of ICommandParser
            container.Register<ICommandParser, CommandParser>();

            // Register IMessenger with names for each BotID.
            var botConfiguration = new BotConfiguration();
            botConfiguration.BotName = msgrBotName;

            for (int i = 0; i < msgrBotId.Length; i++)
            {
                var token = botRouteToken[i];

                // Register the new bot instance of IMessenger using the botID as the registration name
                var groupMeMessenger = new GroupMeMessenger(
                    msgrBotId[i],
                    msgrBotName,
                    msgrIgnoreNames,
                    msgrEndPointUrl,
                    container.Resolve<IHttpHandler>(),
                    _logger);

                container.Register<IMessenger, GroupMeMessenger>(groupMeMessenger, token);

                botConfiguration.Routes.Add(new BotRoute()
                {
                    SecretToken = token,
                    BotId = msgrBotId[i]
                });
            }

            container.Register(botConfiguration);

            // Register the instance of IBotServices
            container.Register<IBotServices, BotServices>();

            string pluginDirectory = _configuration["PLUGIN_DIR"] ??
                                     Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                                         "plugins");

            var pluginManager = new PluginManager.PluginManager(
                msgrBotName,
                pluginDirectory,
                this._logger,
                container.Resolve<ICommandParser>(),
                container);

            container.Register<IPluginManager>(pluginManager);
        }

        private void LoadConfiguration(
            out string[] msgrBotId,
            out string msgrBotName,
            out string msgrEndPointUrl,
            out string[] msgrIgnoreNames,
            out string[] botRouteToken,
            out string urlUser,
            out string urlKey)
        {

            if (string.IsNullOrEmpty(_configuration["BOTID"]))
            {
                throw new Exception("Configuration 'BOTID' was not specified.");
            }
      
            msgrBotId = _configuration["BOTID"].Split('|');

            if (string.IsNullOrEmpty(_configuration["BOTNAME"]))
            {
                throw new Exception("Configuration 'BOTNAME' was not specified.");
            }

            msgrBotName = _configuration["BOTNAME"] ?? "NerdBot";
            msgrEndPointUrl = "https://api.groupme.com/v3/bots/post";

            string ignoreNames = _configuration["IGNORENAMES"];
            msgrIgnoreNames = new string[] { };
            if (!string.IsNullOrEmpty(ignoreNames))
            {
                if (ignoreNames.Contains("|"))
                    msgrIgnoreNames = ignoreNames.Split('|').ToArray();
            }

            if (string.IsNullOrEmpty(_configuration["TOKEN"]))
            {
                throw new Exception("Configuration 'TOKEN' was not specified.");
            }

            botRouteToken = _configuration["TOKEN"].Split('|');

            if (string.IsNullOrEmpty(_configuration["BITLY_USER"]))
            {
                throw new Exception("Configuration 'BITLY_USER' was not specified.");
            }

            urlUser = _configuration["BITLY_USER"];

            if (string.IsNullOrEmpty(_configuration["BITLY_KEY"]))
            {
                throw new Exception("Configuration 'BITLY_KEY' was not specified.");
            }

            urlKey = _configuration["BITLY_KEY"];
        }
    }
}
