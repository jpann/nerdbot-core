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
    public class ScryFallImgPlugin : PluginBase
    {
        private const string cSearchUrl = "https://scryfall.com/search?q=name:/{0}/";

        private ScryFallFetcher fetcher;

        public override string Name
        {
            get { return "simg command"; }
        }

        public override string Description
        {
            get { return "Returns a link to the card's image from Scryfall.com"; }
        }

        public override string ShortDescription
        {
            get { return "Returns a link to the card's image from Scryfall.com"; }
        }

        public override string Command
        {
            get { return "simg"; }
        }

        public override string HelpCommand
        {
            get { return "help simg"; }
        }

        public override string HelpDescription
        {
            get { return $"{this.Command} example usage: 'simg spore clou%' or 'simg fem;spore cloud' or 'simg fallen empires;spore %loud'"; }
        }

        public ScryFallImgPlugin(IBotServices services) : base(services)
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

                    // If Image_Uris is not null, get image
                    if (scryCard.Image_Uris != null)
                    {
                        string img_url = string.Empty;

                        // Get image, starting with large
                        if (!string.IsNullOrEmpty(scryCard.Image_Uris.Large))
                            img_url = scryCard.Image_Uris.Large;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Png))
                            img_url = scryCard.Image_Uris.Png;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Normal))
                            img_url = scryCard.Image_Uris.Normal;
                        else if (!string.IsNullOrEmpty(scryCard.Image_Uris.Small))
                            img_url = scryCard.Image_Uris.Small;

                        if (!string.IsNullOrEmpty(img_url))
                        {
                            // Remove trailing ?xxxxxxxx portion from uri, if it exists
                            if (!string.IsNullOrEmpty(img_url) && img_url.LastIndexOf('?') > 0)
                            {
                                img_url = img_url.Substring(0, img_url.LastIndexOf('?'));
                            }

                            messenger.SendMessage(img_url);
                        }
                        else
                        {
                            messenger.SendMessage("Unable to find image.");
                        }
                        
                    }
                    else
                    {
                        // This card likely has two sides, get both sides and send them
                        if (scryCard.Card_Faces != null)
                        {
                            foreach (ScryFall_CardFace face in scryCard.Card_Faces)
                            {
                                string img_url = string.Empty;

                                // Get image, starting with large
                                if (!string.IsNullOrEmpty(face.Image_Uris.Large))
                                    img_url = face.Image_Uris.Large;
                                else if (!string.IsNullOrEmpty(face.Image_Uris.Png))
                                    img_url = face.Image_Uris.Png;
                                else if (!string.IsNullOrEmpty(face.Image_Uris.Normal))
                                    img_url = face.Image_Uris.Normal;
                                else if (!string.IsNullOrEmpty(face.Image_Uris.Small))
                                    img_url = face.Image_Uris.Small;

                                if (!string.IsNullOrEmpty(img_url))
                                {
                                    // Remove trailing ?xxxxxxxx portion from uri, if it exists
                                    if (!string.IsNullOrEmpty(img_url) && img_url.LastIndexOf('?') > 0)
                                    {
                                        img_url = img_url.Substring(0, img_url.LastIndexOf('?'));
                                    }

                                    messenger.SendMessage(img_url);
                                }
                                else
                                {
                                    messenger.SendMessage("Unable to find image.");
                                }
                            }
                        }
                        else
                        {
                            messenger.SendMessage("No image found on ScryFal");
                        }
                    }

                    return true;
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

                    if (name.Contains('%') || name.Contains('*'))
                    {
                        name = name.Replace("%", "").Replace("*", "");
                    }

                    // Get first 5 characters of name to use with autocomplete
                    string autocompleteName = new string(name.Take(5).ToArray());

                    var autocompleteResults = await autocompleter.GetAutocompleteAsync(autocompleteName);
                    if (autocompleteResults != null && autocompleteResults.Any())
                    {
                        this.Logger.Debug($"Autocomplete returned '{autocompleteResults.Count()}' results for '{name}'...");

                        string suggestions = autocompleteResults.Take(5).OxbridgeOr();

                        string msg = string.Format($"Did you mean {suggestions}?");

                        messenger.SendMessage(msg);
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
