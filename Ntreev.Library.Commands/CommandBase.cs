using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public abstract class CommandBase : ICommand, IExecutable
    {
        private readonly string name;

        protected CommandBase(string name)
        {
            this.name = name;
        }

        public virtual string[] GetCompletions(CommandCompletionContext completionContext)
        {
            return null;
        }

        public string Name
        {
            get { return this.name; }
        }

        public virtual bool IsEnabled
        {
            get { return true; }
        }

        protected abstract void OnExecute();

        protected CommandMemberDescriptor GetDescriptor(string propertyName)
        {
            return CommandDescriptor.GetMemberDescriptors(this)[propertyName];
        }

        protected CommandMemberDescriptor GetStaticDescriptor(Type type, string propertyName)
        {
            return CommandDescriptor.GetStaticMemberDescriptors(type)[propertyName];
        }

        #region ICommand

        void IExecutable.Execute()
        {
            this.OnExecute();
        }

        #endregion
    }
}
