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
    public class ScryFallImgPlugin_Tests
    {
        private ScryFallImgPlugin simgCommandPlugin;
        private UnitTestContext unitTestContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [SetUp]
        public void SetUp()
        {
            unitTestContext = new UnitTestContext();

            simgCommandPlugin = new ScryFallImgPlugin(unitTestContext.BotServicesMock.Object);

            simgCommandPlugin.Logger = unitTestContext.LoggerMock.Object;
        }

        [Test]
        public void SImgCommand_NoArguments()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByName()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "53.54",
                    USDFoil = "53.54"
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);
            
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(card.Image_Uris.Small));
        }
        
        [Test]
        public void SImgCommand_ByNameAndSet()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "53.54",
                    USDFoil = "53.54"
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?exact={0}&set={1}", Uri.EscapeDataString(card.Name), Uri.EscapeDataString(card.SetCode))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(card.Image_Uris.Small));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound()
        {
            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?fuzzy=" + Uri.EscapeDataString("not found")))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoCardFound()
        {
            ScryFallCard card = null;
            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>(string.Format("https://api.scryfall.com/cards/named?fuzzy={0}&set={1}", Uri.EscapeDataString("not found"), Uri.EscapeDataString("EXP"))))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    "not found"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Try seeing if your card is here: https://scryfall.com/search?q=name:/not%20found/"));
        }

        [Test]
        public void SImgCommand_ByName_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    ""
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoName()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "EXP",
                    ""
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoSet()
        {
            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "",
                    "Spore Cloud"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            Assert.IsFalse(handled);
        }

        [Test]
        public void SImgCommand_LargeImg()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "53.54",
                    USDFoil = "53.54"
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg",
                    Large = "https://img.scryfall.com/cards/large/en/exp/43.jpg"
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(card.Image_Uris.Large));
        }

        [Test]
        public void SImgCommand_NormalImg()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "",
                    USDFoil = ""
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg",
                    Normal = "https://img.scryfall.com/cards/normal/en/exp/43.jpg",
                    Large = ""
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(card.Image_Uris.Normal));
        }

        [Test]
        public void SImgCommand_SmallImg()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "",
                    USDFoil = ""
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "https://img.scryfall.com/cards/small/en/exp/43.jpg",
                    Normal = "",
                    Large = ""
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(card.Image_Uris.Small));
        }

        [Test]
        public void SImgCommand_ExistsOnScryFallButNoImage()
        {
            var card = new ScryFallCard()
            {
                Name = "Strip Mine",
                Prices = new ScryFall_Prices()
                {
                    USD = "",
                    USDFoil = ""
                },
                ScryFallUri = "https://scryfall.com/card/exp/43?utm_source=api",
                SetName = "Zendikar Expeditions",
                SetCode = "EXP",
                Image_Uris = new ScryFall_ImgUri()
                {
                    Small = "",
                    Normal = "",
                    Large = ""
                }
            };

            unitTestContext.HttpClientMock.Setup(h =>
                    h.GetAsync<ScryFallCard>("https://api.scryfall.com/cards/named?exact=" + Uri.EscapeDataString(card.Name)))
                .ReturnsAsync(card);

            var cmd = new Command()
            {
                Cmd = "simg",
                Arguments = new string[]
                {
                    "Strip Mine"
                }
            };

            var msg = new GroupMeMessage();
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage("Unable to find image."));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound_RunAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void SImgCommand_ByNameAndSet_NoCardFound_RunAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Did you mean Spore Frog"))));
        }

        [Test]
        public void SImgCommand_ByName_NoCardFound_NoAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }

        [Test]
        public void SImgCommand_ByNameSet_NoCardFound_NoAutocomplete()
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
            simgCommandPlugin.OnLoad();

            bool handled = simgCommandPlugin.OnCommand(
                cmd,
                msg,
                unitTestContext.MessengerMock.Object
            ).Result;

            unitTestContext.MessengerMock.Verify(m => m.SendMessage(It.Is<string>(s => s.StartsWith("Try seeing if your card is here"))));
        }
    }
}
