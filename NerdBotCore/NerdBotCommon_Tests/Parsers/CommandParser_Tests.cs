using System;
using System.Collections.Generic;
using System.Text;
using NerdBotCommon.Parsers;
using NUnit.Framework;

namespace NerdBotCommon_Tests.Parsers
{
    [TestFixture]
    class CommandParser_Tests
    {
        private ICommandParser commandParser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            commandParser = new CommandParser();
        }

        #region Image Command Tests
        [Test]
        public void ImageCommand()
        {
            string message = "img spore burst";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(1, cmd.Arguments.Length);
            Assert.AreEqual("spore burst", cmd.Arguments[0]);
        }

        [Test]
        public void ImageCommand_WithWildcard()
        {
            string message = "img spore %";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(1, cmd.Arguments.Length);
            Assert.AreEqual("spore %", cmd.Arguments[0]);
        }

        [Test]
        public void ImageCommand_WithComma()
        {
            string message = "img shu yun, the silent tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(1, cmd.Arguments.Length);
            Assert.AreEqual("shu yun, the silent tempest", cmd.Arguments[0]);
        }

        [Test]
        public void ImageCommand_WithApostrophe()
        {
            string message = "img krark’s thumb";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(1, cmd.Arguments.Length);
            Assert.AreEqual("krark’s thumb", cmd.Arguments[0]);
        }

        [Test]
        public void ImageCommand_WithCommaAndWildcard()
        {
            string message = "img shu yun, the % tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(1, cmd.Arguments.Length);
            Assert.AreEqual("shu yun, the % tempest", cmd.Arguments[0]);
        }

        [Test]
        public void ImageCommand_WithSetName()
        {
            string message = "img conspiracy;spore burst";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("conspiracy", cmd.Arguments[0]);
            Assert.AreEqual("spore burst", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetNameAndWildcard()
        {
            string message = "img conspiracy;spore %";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("conspiracy", cmd.Arguments[0]);
            Assert.AreEqual("spore %", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetName_WithComma()
        {
            string message = "img fate reforged;shu yun, the silent tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("fate reforged", cmd.Arguments[0]);
            Assert.AreEqual("shu yun, the silent tempest", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetName_WithCommaAndWildcard()
        {
            string message = "img fate reforged;shu yun, the % tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("fate reforged", cmd.Arguments[0]);
            Assert.AreEqual("shu yun, the % tempest", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetCode()
        {
            string message = "img con;spore burst";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("con", cmd.Arguments[0]);
            Assert.AreEqual("spore burst", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetCodeAndWildcard()
        {
            string message = "img con;spore %";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("con", cmd.Arguments[0]);
            Assert.AreEqual("spore %", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetCode_WithComma()
        {
            string message = "img frf;shu yun, the silent tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("frf", cmd.Arguments[0]);
            Assert.AreEqual("shu yun, the silent tempest", cmd.Arguments[1]);
        }

        [Test]
        public void ImageCommand_WithSetCode_WithCommaAndWildcard()
        {
            string message = "img frf;shu yun, the % tempest";

            var cmd = commandParser.Parse(message);

            Assert.AreEqual("img", cmd.Cmd);
            Assert.AreEqual(2, cmd.Arguments.Length);
            Assert.AreEqual("frf", cmd.Arguments[0]);
            Assert.AreEqual("shu yun, the % tempest", cmd.Arguments[1]);
        }

		[Test]
		public void ImageCommand_WithSetCodeContainsUnderscore()
		{
			string message = "img mps_akh;Diabolic Intent";

			var cmd = commandParser.Parse(message);

			Assert.AreEqual("img", cmd.Cmd);
			Assert.AreEqual(2, cmd.Arguments.Length);
			Assert.AreEqual("mps_akh", cmd.Arguments[0]);
			Assert.AreEqual("Diabolic Intent", cmd.Arguments[1]);
		}
        #endregion
    }
}
