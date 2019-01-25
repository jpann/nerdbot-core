using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Nancy.TinyIoc;
using NerdBotCommon.Extensions;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using Serilog;

namespace NerdBotHost.PluginManager
{
    public class PluginManager : IPluginManager
    {
        private readonly ILogger _logger;
        private readonly ICommandParser _commandParser;
        private readonly TinyIoCContainer _container;
        private string _pluginDirectory;
        private List<IPlugin> _plugins = new List<IPlugin>();
        private List<IMessagePlugin> _messagePlugins = new List<IMessagePlugin>();
        private string _botName;

        #region Properties
        public string BotName
        {
            set { this._botName = value; }
            get { return this._botName; }
        }

        public string PluginDirectory
        {
            get { return this._pluginDirectory; }
            set
            {
                if (!Directory.Exists(value))
                    throw new DirectoryNotFoundException(value);

                this._pluginDirectory = value;
            }
        }

        public List<IPlugin> Plugins
        {
            get { return this._plugins; }
        }

        public List<IMessagePlugin> MessagePlugins
        {
            get { return this._messagePlugins; }
        }
        #endregion

        public PluginManager(ILogger logger, ICommandParser commandParser, TinyIoCContainer container)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");
            if (container == null)
                throw new ArgumentNullException("container");

            this._logger = logger;
            this._commandParser = commandParser;
            this._container = container;
        }

        public PluginManager(
            string botName,
            string pluginDirectory,
            ILogger logger,
            ICommandParser commandParser,
            TinyIoCContainer container)
        {
            if (string.IsNullOrEmpty(botName))
                throw new ArgumentException("botName");

            if (string.IsNullOrEmpty(pluginDirectory))
                throw new ArgumentException("pluginDirectory");

            if (!Directory.Exists(pluginDirectory))
            {
                Directory.CreateDirectory(pluginDirectory);
            }
                

            if (commandParser == null)
                throw new ArgumentNullException("commandParser");

            if (container == null)
                throw new ArgumentNullException("container");

            this._botName = botName;
            this._pluginDirectory = pluginDirectory;
            this._logger = logger;
            this._commandParser = commandParser;
            this._container = container;

            this.LoadPlugins();
        }

        public void LoadPlugins()
        {
            // Load plugins from main plugin directory
            this.LoadPluginsFromDirectory(this._pluginDirectory);

            // Load plugins from any sub directory of the main plugin directory
            DirectoryInfo info = new DirectoryInfo(this._pluginDirectory);
            foreach (var subDirectory in info.GetDirectories())
            {
                this.LoadPluginsFromDirectory(subDirectory.FullName);
            }
        }

        private void LoadPluginsFromDirectory(string directory)
        {
            this._logger.Debug($"Loading plugins from directory '{directory}...");

            DirectoryInfo info = new DirectoryInfo(directory);

            foreach (FileInfo fileInfo in info.GetFiles("*.dll"))
            {
                this._logger.Information($"Trying to load file '{fileInfo.FullName}");

                Assembly currentAssembly = Assembly.LoadFile(fileInfo.FullName);

                if (!currentAssembly.GetTypes().Any())
                {
                    this._logger.Warning($"File {fileInfo.FullName} does not contain any assemblies");
                }

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.ImplementsInterface(typeof(IPlugin)))
                    {
                        this._logger.Warning($"Type {type.ToString()} doesn't implement IPlugin! Skipping...");
                        continue;
                    }
                        

                    this._logger.Debug($"Loading plugin '{type.ToString()}' from '{currentAssembly.GetName()}'...");

                    IPlugin plugin = (IPlugin)this._container.Resolve(type);

                    this._container.BuildUp(plugin);

                    plugin.OnLoad();

                    this._logger.Debug($"Loaded plugin '{type.ToString()}'!");

                    this._plugins.Add(plugin);
                }
            }

            this._logger.Debug($"Loaded {this.Plugins.Count} plugins.");

            // Load IMessagePlugin plugins
            this._logger.Debug($"Loading message plugins from {directory}...");

            DirectoryInfo mpInfo = new DirectoryInfo(directory);

            foreach (FileInfo fileInfo in mpInfo.GetFiles("*.dll"))
            {
                Assembly currentAssembly = Assembly.LoadFile(fileInfo.FullName);

                foreach (Type type in currentAssembly.GetTypes())
                {
                    if (!type.ImplementsInterface(typeof(IMessagePlugin)))
                        continue;

                    this._logger.Debug($"Loading message plugin '{type.ToString()}' from '{currentAssembly.GetName()}'...");

                    IMessagePlugin plugin = (IMessagePlugin)this._container.Resolve(type);

                    this._container.BuildUp(plugin);

                    plugin.BotName = this._botName;
                    plugin.OnLoad();

                    this._logger.Debug($"Loaded message plugin '{type.ToString()}'!");

                    this._messagePlugins.Add(plugin);
                }
            }

            this._logger.Debug($"Loaded { this.MessagePlugins.Count} message plugins.");
        }

        public void UnloadPlugins()
        {
            foreach (IPlugin plugin in this._plugins)
            {
                plugin.OnUnload();

                this._plugins.Remove(plugin);
            }

            foreach (IMessagePlugin plugin in this._messagePlugins)
            {
                plugin.OnUnload();

                this._messagePlugins.Remove(plugin);
            }
        }

        public async Task<bool> HandleCommandAsync(
            Command command, 
            IMessage message, 
            IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            try
            {
                foreach (IPlugin plugin in this._plugins)
                {
                    if (plugin.Command == command.Cmd)
                    {
                        this._logger.Debug($"Calling OnCommand for {command.Cmd}' in plugin '{plugin.Name}'...");

                        bool handled = await plugin.OnCommand(command, message, messenger);

                        return handled;
                    }
                }
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"Error sending command to plugins: {er.Message}");
            }

            return false;
        }

        public async Task<bool> HandledHelpCommandAsync(Command command, IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            // If no arguments were used, return false because the command wasn't handled
            if (!command.Arguments.Any())
            {
                this._logger.Debug($"Command '{command.Cmd}' had no arguments provided.");

                return false;
            }

            try
            {
                string argument = command.Arguments[0].ToLower().Trim();

                this._logger.Debug($"Help argument: {argument}");

                foreach (IPlugin plugin in this._plugins)
                {
                    string helpCmd = command.Cmd + " " + argument;

                    if (plugin.HelpCommand.ToLower() == helpCmd)
                    {
                        string helpText = plugin.HelpDescription;

                        messenger.SendMessage(helpText);

                        return true;
                    }
                }

                // Check for core help commands
                // Get list of available commands
                if (argument == "commands")
                {
                    string msg;

                    if (!this.Plugins.Any())
                    {
                        msg = "No commands available.";
                    }
                    else
                    {
                        string availableCommands = string.Join(", ", this.Plugins.Select(p => p.Command).ToArray());

                        msg = $"Available commands: {availableCommands}";
                    }

                    this._logger.Debug($"Available commands: {msg}");

                    messenger.SendMessage(msg);

                    return true;
                }
            }
            catch (Exception er)
            {
                this._logger.Error(er, $"Error handling help command: {er.Message}");
            }

            return false;
        }
    }
}
