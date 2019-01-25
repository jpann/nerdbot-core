using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;

namespace NerdBotGiphyPlugin
{
    public class GiphyPlugin : PluginBase
    {
        private GiphyFetcher _giphyFetcher;

        public override string Name
        {
            get { return "giphy command"; }
        }

        public override string Description
        {
            get { return "Returns a random gif from Giphy using the keyword.";  }
        }

        public override string ShortDescription
        {
            get { return "Returns a random gif from Giphy using the keyword."; }
        }

        public override string Command
        {
            get { return "giphy"; }
        }

        public override string HelpCommand
        {
            get { return "help giphy"; }
        }

        public override string HelpDescription
        {
            get { return $"{this.Command} example usage: 'giphy awesome'"; }
        }

        public GiphyPlugin(IBotServices services) : base(services)
        {
        }

        public override void OnLoad()
        {
            this._giphyFetcher = new GiphyFetcher(this.Services.HttpClient, this.Logger);
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

            try
            {
                if (command.Arguments.Any())
                {
                    string giphyUrl = null;

                    if (command.Arguments.Length == 1)
                    {
                        string keyword = command.Arguments[0];

                        if (!string.IsNullOrEmpty(keyword))
                            giphyUrl = await this._giphyFetcher.GetGifAsync(keyword);
                    }

                    if (!string.IsNullOrEmpty(giphyUrl))
                    {
                        await messenger.SendMessage(giphyUrl);
                    }
                }

                return false;
            }
            catch (Exception er)
            {
                this._logger.Error(er, "Error handling giphy command.");

                return false;
            }

        }
    }
}
