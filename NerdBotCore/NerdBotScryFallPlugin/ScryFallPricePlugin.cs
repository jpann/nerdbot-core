using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NerdBotCommon.Extensions;
using NerdBotCommon.Messengers;
using NerdBotCommon.Parsers;
using NerdBotCommon.Plugin;
using NerdBotScryFallPlugin.POCO;

namespace NerdBotScryFallPlugin
{
    public class ScryFallPricePlugin : PluginBase
    {
        private const string cSearchUrl = "https://scryfall.com/search?q=name:/{0}/";

        private ScryFallFetcher fetcher;

        public override string Name
        {
            get { return "scry command"; }
        }

        public override string Description
        {
            get { return "Returns the card's USD price from scryfall.com."; }
        }

        public override string ShortDescription
        {
            get { return "Returns the card's USD price from scryfall.com."; }
        }

        public override string Command
        {
            get { return "scry"; }
        }

        public override string HelpCommand
        {
            get { return "help scry"; }
        }

        public override string HelpDescription
        {
            get { return $"{this.Command} example usage: 'scry spore clou%' or 'scry fem;spore cloud' or 'scry fallen empires;spore %loud'"; }
        }

        public ScryFallPricePlugin(IBotServices services) : base(services)
        {
        }

        public override void OnLoad()
        {
            fetcher = new ScryFallFetcher(this.Services.HttpClient, this.Logger);
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

            var autocompleter = new ScryFallAutocompleter(this.Services.HttpClient, this.Logger);

            if (command.Arguments.Any())
            {
                ScryFallCard scryCard = null;

                string searchTerm = null;

                if (command.Arguments.Length == 1)
                {
                    string name = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    this.Logger.Debug($"Using Name: {name}");

                    searchTerm = name;

                    // Get card using only name
                    scryCard = await fetcher.GetCard(name);
                }
                else if (command.Arguments.Length == 2)
                {
                    string name = command.Arguments[1];
                    string set = command.Arguments[0];

                    if (string.IsNullOrEmpty(name))
                        return false;

                    if (string.IsNullOrEmpty(set))
                        return false;

                    this.Logger.Debug($"Using Name: {name}; Set: {set}");

                    searchTerm = string.Join(" ", command.Arguments);

                    // Get card using name and set name or code
                    scryCard = await fetcher.GetCard(name, set);
                }

                if (scryCard != null)
                {
                    this.Logger.Debug($"Found card '{scryCard.Name}' in set '{scryCard.SetName}'.");

                    if (scryCard.Prices != null)
                    {
                        string price = scryCard.Prices.USD;

                        string url = scryCard.ScryFallUri;

                        url = this.Services.UrlShortener.ShortenUrl(url);

                        if (!string.IsNullOrEmpty(price))
                        {
                            string msg = string.Format($"{scryCard.Name} [{scryCard.SetCode.ToUpper()}] - ${price}. {url}");

                            messenger.SendMessage(msg);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(scryCard.Prices.USDFoil))
                            {
                                string msg = string.Format($"{scryCard.Name} [{scryCard.SetCode.ToUpper()}] - ${scryCard.Prices.USDFoil} (FOIL). {url}");

                                messenger.SendMessage(msg);
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    this.Logger.Warning("Couldn't find card using arguments.");

                    // Use autocomplete to try returning a list of suggested names
                    string name = "";
                    if (command.Arguments.Length == 1)
                        name = command.Arguments[0];
                    else
                        name = command.Arguments[1];

                    // Get first 5 characters of name to use with autocomplete
                    string autocompleteName = new string(name.Take(5).ToArray());

                    var autocompleteResults = await autocompleter.GetAutocompleteAsync(autocompleteName);
                    if (autocompleteResults != null && autocompleteResults.Any())
                    {
                        this.Logger.Debug($"Autocomplete returned '{autocompleteResults.Count()}' results for '{name}'...");

                        string suggestions = autocompleteResults.Take(5).OxbridgeOr();

                        messenger.SendMessage($"Did you mean {suggestions}?");
                    }
                    else
                    {
                        name = Uri.EscapeDataString(name);

                        string url = string.Format(cSearchUrl, name);

                        messenger.SendMessage($"Try seeing if your card is here: {url}");
                    }
                }
            }
            else
            {
                this.Logger.Warning("No arguments provided.");
            }

            return false;
        }
    }
}
