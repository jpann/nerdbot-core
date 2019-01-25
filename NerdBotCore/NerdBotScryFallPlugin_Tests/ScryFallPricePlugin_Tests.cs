using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NerdBotCommon.Messengers.GroupMe;
using NerdBotCommon.Parsers;
using NerdBotScryFallPlugin;
using NerdBotScryFallPlugin.POCO;
using NerdBot_TestHelper;
using NUnit.Framework;

namespace NerdBotScryFallPlugin_Tests
{
    public class ScryFallPricePlugin_Tests
    {
        private ScryFallPricePlugin scryCommandPlugin;
        private UnitTestContext unitTestContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            scryCommandPlugin = new ScryFallPricePlugin(unitTestContext.BotServicesMock.Object);

            scryCommandPlugin.Logger = unitTestContext.LoggerMock.Object;
        }

        [Test]
        public void ScryCommand_NoArguments()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByName()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                PriceUsd = "53.54",
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Strip Mine [EXP] - $53.54"))));
        }

        [Test]
        public void ScryCommand_ByNameAndSet()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                PriceUsd = "53.54",
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?exact={0}&set={1}", Uri.EscapeDataString(card.Name), Uri.EscapeDataString(card.SetCode))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Strip Mine [EXP] - $53.54"))));
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound()
        {
            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?fuzzy=" + Uri.EscapeDataString("not found")))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoCardFound()
        {
            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}&set={1}", Uri.EscapeDataString("not found"), Uri.EscapeDataString("EXP"))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void ScryCommand_ByName_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    ""
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    ""
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoSet()
        {
            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void ScryCommand_NoPrice()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                PriceUsd = "",
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg?1509690533"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound_RunAutocomplete()
        {
            string name = "spore bloud";

            // Setup mock response from autocomplete website
            unitTestContext.HttpClientMock.Setup(ac => ac.GetAsync<ScryFallAutocompleteCatalog>("https://api.scryfall.com/cards/autocomplete?q=spore"))
                .ReturnsAsync(new ScryFallAutocompleteCatalog()
                {
                    Object = "catalog",
                    TotalValues = 4,
                    Data = new List<string>()
                    {
                        "Spore Frog",
                        "Spore Burst",
                        "Spore Swarm",
                        "Sporemound"
                    }
                });

            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}", Uri.EscapeDataString(name))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void ScryCommand_ByNameAndSet_NoCardFound_RunAutocomplete()
        {
            string name = "spore bloud";

            // Setup mock response from autocomplete website
            unitTestContext.HttpClientMock.Setup(ac => ac.GetAsync<ScryFallAutocompleteCatalog>("https://api.scryfall.com/cards/autocomplete?q=spore"))
                .ReturnsAsync(new ScryFallAutocompleteCatalog()
                {
                    Object = "catalog",
                    TotalValues = 4,
                    Data = new List<string>()
                    {
                        "Spore Frog",
                        "Spore Burst",
                        "Spore Swarm",
                        "Sporemound"
                    }
                });

            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}&set={1}", Uri.EscapeDataString(name), Uri.EscapeDataString("EXP"))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "EXP",
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void ScryCommand_ByName_NoCardFound_NoAutocomplete()
        {
            string name = "spore bloud";

            // Setup mock response from autocomplete website
            unitTestContext.HttpClientMock.Setup(ac => ac.GetAsync<ScryFallAutocompleteCatalog>("https://api.scryfall.com/cards/autocomplete?q=spore"))
                .ReturnsAsync(new ScryFallAutocompleteCatalog()
                {
                    Object = "catalog",
                    TotalValues = 0,
                    Data = new List<string>()
                    {
                    }
                });

            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}", Uri.EscapeDataString(name))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }

        [Test]
        public void ScryCommand_ByNameSet_NoCardFound_NoAutocomplete()
        {
            string name = "spore bloud";

            // Setup mock response from autocomplete website
            unitTestContext.HttpClientMock.Setup(ac => ac.GetAsync<ScryFallAutocompleteCatalog>("https://api.scryfall.com/cards/autocomplete?q=spore"))
                .ReturnsAsync(new ScryFallAutocompleteCatalog()
                {
                    Object = "catalog",
                    TotalValues = 4,
                    Data = new List<string>()
                    {
                    }
                });

            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}&set={1}", Uri.EscapeDataString(name), Uri.EscapeDataString("C13"))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "scry",
                Arguments = new string[]
                {
                    "C13",
                    name
                }
            };

            var msg = new GroupMeMessage();
            scryCommandPlugin.OnLoad();

            bool handled = scryCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }
    }
}
