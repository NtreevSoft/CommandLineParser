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
        private static string delimiter = "--";
        private static string shortDelimiter = "-";
        private static char itemSeparator = ';';
        private static Func<string, string> nameGenerator;

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

        private static string ToSpinalCase(string text)
        {
            return Regex.Replace(text, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        internal static void ValidateIdentifier(string name)
        {
            if (Regex.IsMatch(name, "^[_a-zA-Z][_a-zA-Z0-9]*") == false)
                throw new ArgumentException(string.Format("{0} is a invalid member name"));
        }
    }
}
