using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotCommon.POCOS;

namespace NerdBotCorePlugin
{
    public class CoinFlipPlugin : PluginBase
    {
        private Random _random;

        public override string Name
        {
            get { return "coinflip command"; }
        }

        public override string Description
        {
            get { return "Clips a coin and returns the result.";  }
        }

        public override string ShortDescription
        {
            get { return "Clips a coin and returns the result."; }
        }

        public override string Command
        {
            get { return "coinflip"; }
        }

        public override string HelpCommand
        {
            get { return "help coinflip"; }
        }

        public override string HelpDescription
        {
            get { return $"{this.Command} example usage: 'coinflip'"; }
        }

        public CoinFlipPlugin(IBotServices services) : base(services)
        {
        }

        public override void OnLoad()
        {
            this._random = new Random();
        }

        public override void OnUnload()
        {
        }

        public override async Task<bool> OnCommand(Command command, IMessage message, IMessenger messenger)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (message == null)
                throw new ArgumentNullException("message");

            if (messenger == null)
                throw new ArgumentNullException("messenger");

            string flip = "Heads";

            if ((this._random.Next(0, 100) % 2) == 0)
                flip = "Heads";
            else
                flip = "Tails";

            await messenger.SendMessage($"Coin flip: {flip}");

            return true;
        }
    }
}
