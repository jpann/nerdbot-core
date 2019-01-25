using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using Serilog;

namespace NerdBotCommon.Plugin
{
    public abstract class MessagePluginBase : IMessagePlugin
    {
        protected string mBotName;
        protected IBotServices _services;
        protected ILogger _logger;

        public MessagePluginBase(
            IBotServices services)
        {
            if (services == null)
                throw new ArgumentNullException("services");

            this._services = services;
        }

        #region Properties
        public string BotName
        {
            set
            {
                this.mBotName = value;
            }

            get { return this.mBotName; }
        }

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
        #endregion

        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract Task<bool> OnMessage(IMessage message, IMessenger messenger);
    }
}
