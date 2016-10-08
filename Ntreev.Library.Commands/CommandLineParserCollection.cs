using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandLineParserCollection : IEnumerable<CommandLineParser>
    {
        private readonly Dictionary<ICommand, CommandLineParser> parsers = new Dictionary<ICommand, CommandLineParser>();

        public CommandLineParserCollection()
        {

        }

        public bool Contains(ICommand command)
        {
            if (this.parsers.ContainsKey(command) == false)
                return false;
            return false;
        }

        public CommandLineParser this[ICommand command]
        {
            get
            {
                if (this.parsers.ContainsKey(command) == false)
                    return null;
                return this.parsers[command];
            }
        }

        internal void Add(ICommand command, CommandLineParser parser)
        {
            this.parsers.Add(command, parser);
        }

        #region IEnumerable

        IEnumerator<CommandLineParser> IEnumerable<CommandLineParser>.GetEnumerator()
        {
            foreach(var item in this.parsers)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.parsers)
            {
                yield return item.Value;
            }
        }

        #endregion
    }
}
