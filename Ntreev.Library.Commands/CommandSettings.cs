using Ntreev.Library.Commands.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public static class CommandSettings
    {
        private static string switchDelimiter = "--";
        private static string shortSwitchDelimiter = "-";
        private static char itemSeperator = ';';

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
            get { return itemSeperator; }
            set
            {
                if(char.IsPunctuation(value) == false)
                    throw new Exception(Resources.SwitchDelimiterMustBePunctuation);
                itemSeperator = value;
            }
        }
    }
}
