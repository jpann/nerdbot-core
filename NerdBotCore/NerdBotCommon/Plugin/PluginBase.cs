using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using Serilog;

namespace NerdBotCommon.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        protected IBotServices _services;
        protected ILogger _logger;

        public PluginBase(IBotServices services)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            this._services = services;
        }

        #region Properties
        public IBotServices Services
        {
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this._services = value;
            }
            get { return this._services; }
        }

        public ILogger Logger
        {
            get { return this._logger; }
            set { this._logger = value; }
        }


        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ShortDescription { get; }
        public abstract string Command { get; }
        public abstract string HelpCommand { get; }
        public abstract string HelpDescription { get; }
        #endregion

        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger);
    }
}
