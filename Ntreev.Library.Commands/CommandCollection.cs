using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class CommandCollection : IEnumerable<ICommand>
    {
        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

        internal CommandCollection()
        {

        }

        public bool Contains(string commandName)
        {
            if (this.commands.ContainsKey(commandName) == false)
                return false;
            return true;
        }

        public ICommand this[string commandName]
        {
            get
            {
                if (this.commands.ContainsKey(commandName) == false)
                    return null;
                return this.commands[commandName];
            }
        }

        public int Count
        {
            get { return this.commands.Count; }
        }

        internal void Add(ICommand command)
        {
            if (this.commands.ContainsKey(command.Name) == true && command.Types.HasFlag(CommandTypes.HasSubCommand) == false)
                throw new ArgumentException($"command '{command.Name}' is already registered.");
            this.commands.Add(command.Name, command);
        }

        #region IEnumerable

        IEnumerator<ICommand> IEnumerable<ICommand>.GetEnumerator()
        {
            foreach (var item in this.commands)
            {
                yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in this.commands)
            {
                yield return item.Value;
            }
        }

        #endregion
    }
}
