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

        public string Name
        {
            get { return this.name; }
        }

        public virtual bool IsEnabled
        {
            get { return true; }
        }

        protected abstract void OnExecute();

        public virtual string[] GetCompletions(CommandCompletionContext completionContext)
        {
            return null;
        }

        #region ICommand

        void IExecutable.Execute()
        {
            this.OnExecute();
        }

        #endregion
    }
}
