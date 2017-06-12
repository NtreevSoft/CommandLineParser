using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public abstract class CommandProviderBase : ICommandProvider
    {
        private readonly string commandName;

        protected CommandProviderBase(string commandName)
        {
            this.commandName = commandName;
        }

        public string CommandName
        {
            get { return this.commandName; }
        }
    }
}
