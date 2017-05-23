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
        private readonly Type commandType;

        protected CommandProviderBase(Type commandType)
        {
            this.commandType = commandType;
        }

        public Type CommandType
        {
            get { return this.commandType; }
        }
    }
}
