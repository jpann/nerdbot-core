using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NerdBotCommon.Parsers
{
    public class CommandParser : ICommandParser
    {
        public Command Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException("text");

            Match cmdMatch = Regex.Match(text, @"^(?<cmd>[A-Za-z0-9_]+)", RegexOptions.IgnoreCase);

            if (cmdMatch.Success)
            {
                Command cmd = new Command();
                string command = cmdMatch.Groups["cmd"].Value.ToLower();

                // Change the argument delimiter to ; so arguments can contain a comma
                // This allows using arguments like the card name 'shu yun, the silent tempest'
                Match argMatch = Regex.Match(text, @"^(?<cmd>[A-Za-z0-9]+) (?:(?<args>[A-Za-z0-9!%&\-, ’'""_*]+);?)+", RegexOptions.IgnoreCase);

                if (argMatch.Success)
                {
                    List<string> arguments = new List<string>();

                    foreach (Capture capture in argMatch.Groups["args"].Captures)
                    {
                        arguments.Add(capture.Value);
                    }

                    cmd.Arguments = arguments.ToArray();
                }

                cmd.Cmd = command;

                return cmd;
            }

            return null;
        }
    }
}
