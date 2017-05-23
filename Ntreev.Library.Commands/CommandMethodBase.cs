using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public abstract class CommandMethodBase : ICommand
    {
        private readonly string name;

        protected CommandMethodBase(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name; }
        }
    }
}
