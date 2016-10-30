using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandMethodSwitchAttribute : Attribute
    {
        private readonly string[] propertyNames;

        public CommandMethodSwitchAttribute(string propertyName)
        {
            this.propertyNames = new string[] { propertyName, };
        }

        public CommandMethodSwitchAttribute(params string[] propertyNames)
        {
            this.propertyNames = propertyNames;
        }

        public string[] PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
