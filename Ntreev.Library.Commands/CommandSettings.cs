using Ntreev.Library.Commands.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class CommandSettings
    {
        public const string SwitchPattern = "[a-zA-Z][_a-zA-Z0-9]*";
        public const string ShortSwitchPattern = "[a-zA-Z]";
        private static string delimiter = "--";
        private static string shortDelimiter = "-";
        private static char itemSeparator = ';';
        private static Func<string, string> nameGenerator;

        static CommandSettings()
        {
            IsConsoleMode = true;
        }

        public static string Delimiter
        {
            get { return delimiter; }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.DelimiterMustBePunctuation);
                delimiter = value;
            }
        }

        public static string ShortDelimiter
        {
            get { return shortDelimiter; }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.DelimiterMustBePunctuation);
                shortDelimiter = value;
            }
        }

        public static char ItemSperator
        {
            get { return itemSeparator; }
            set
            {
                if (char.IsPunctuation(value) == false)
                    throw new Exception(Resources.DelimiterMustBePunctuation);
                itemSeparator = value;
            }
        }

        public static Func<string, string> NameGenerator
        {
            get { return nameGenerator ?? ToSpinalCase; }
            set { nameGenerator = value; }
        }

        public static bool IsConsoleMode
        {
            get; set;
        }

        private static string ToSpinalCase(string text)
        {
            ValidateIdentifier(text);
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        internal static void ValidateIdentifier(string name)
        {
            if (Regex.IsMatch(name, $"^{SwitchPattern}") == false)
                throw new ArgumentException(string.Format("{0} is a invalid member name", name));
        }

        internal static bool VerifyName(string argument)
        {
            return Regex.IsMatch(argument, $"{CommandSettings.Delimiter}\\S+|{CommandSettings.ShortDelimiter}\\S+");
        }
    }
}
