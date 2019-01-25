using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotUrbanDictPlugin.POCO;

namespace NerdBotUrbanDictPlugin
{
    public class UrbanDictionaryPlugin : PluginBase
    {
        public override string Name
        {
            get { return "UrbanDictionary command"; }
        }

        public override string Description
        {
            get { return "Returns the top rated definition from UrbanDictionary";  }
        }

        public override string ShortDescription
        {
            get { return "Returns the top rated definition from UrbanDictionary"; }
        }

        public override string Command
        {
            get { return "wtf"; }
        }

        public override string HelpCommand
        {
            get { return "help wtf"; }
        }

        public override string HelpDescription
        {
            get { return $"{this.Command} example usage: 'wtf is a box?'"; }
        }

        public UrbanDictionaryPlugin(IBotServices services) : base(services)
        {
        }

        public override void OnLoad()
        {
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

            string url = "http://api.urbandictionary.com/v0/define?term={0}";

            var urbanDict = new UrbanDictionaryFetcher(url, this.Services.HttpClient, this.Logger);

            if (command.Arguments.Any())
            {
                UrbanDictionaryData defData = null;

                // wtf is a <text>?
                if (command.Arguments.Length == 1)
                {
                    string word = null;

                    if (command.Arguments[0].StartsWith("is an"))
                    {
                        word = command.Arguments[0].Replace("is an", "").Trim();
                    }
                    else if (command.Arguments[0].StartsWith("is a"))
                    {
                        word = command.Arguments[0].Replace("is a", "").Trim();
                    }
                    else if (command.Arguments[0].StartsWith("is"))
                    {
                        word = command.Arguments[0].Replace("is", "").Trim();
                    }

                    if (!string.IsNullOrEmpty(word))
                        defData = await urbanDict.GetDefinition(word);
                }

                if (defData != null)
                {
                    if (defData.Definitions.Any())
                    {
                        // Get random definition
                        var definition = defData.Definitions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                        if (definition != null)
                        {
                            messenger.SendMessage(definition.Definition);

                            if (defData.Tags != null && defData.Tags.Any())
                            {
                                string tags = string.Join(", ", defData.Tags.Take(5).ToArray());

                                messenger.SendMessage($"Perhaps you meant {tags}.");
                            }
                        }
                    }
                    else
                    {
                        messenger.SendMessage("There is no definition for that");

                        return false;
                    }
                }
            }

            return false;
        }
    }
}
