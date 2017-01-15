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
        private static string switchDelimiter = "--";
        private static string shortSwitchDelimiter = "-";
        private static char itemSeparator = ';';
        private static Func<string, string> nameGenerator;

        public static string SwitchDelimiter
        {
            get { return switchDelimiter; }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
                switchDelimiter = value;
            }
        }

        public static string ShortSwitchDelimiter
        {
            get { return shortSwitchDelimiter; }
            set
            {
                if (value.Any(item => char.IsPunctuation(item)) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
                shortSwitchDelimiter = value;
            }
        }

        public static char ItemSperator
        {
            get { return itemSeparator; }
            set
            {
                if (char.IsPunctuation(value) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
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
    }
}
