﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public abstract class SubCommandBase : ICommand
    {
        private readonly string name;
        private readonly CommandTypes types;

        protected SubCommandBase(string name)
        {
            this.name = name;
            this.types = CommandTypes.HasSubCommand;
        }

        public string Name
        {
            get { return this.name; }
        }

        protected virtual void OnExecute()
        {

        }

        #region ICommand

        CommandTypes ICommand.Types
        {
            get { return this.types; }
        }

        void ICommand.Execute()
        {
            this.OnExecute();
        }

        #endregion
    }
}
