using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public virtual bool IsEnabled
        {
            get { return true; }
        }

        protected virtual bool IsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            return true;
        }

        public virtual string[] GetCompletions(CommandMethodDescriptor methodDescriptor, CommandMemberDescriptor memberDescriptor)
        {
            return null;
        }

        internal bool InvokeIsMethodEnabled(CommandMethodDescriptor descriptor)
        {
            return this.IsMethodEnabled(descriptor);
        }
    }
}
