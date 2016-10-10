using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public abstract class Command : ICommand
    {
        private readonly string name;
        private readonly CommandTypes types;

        protected Command(string name)
            : this(name, CommandTypes.None)
        {
            
        }

        protected Command(string name, CommandTypes types)
        {
            this.name = name;
            this.types = types;
        }

        public string Name
        {
            get { return this.name; }
        }

        public CommandTypes Types
        {
            get { return this.types; }
        }

        public virtual void Execute()
        {

        }
    }
}
