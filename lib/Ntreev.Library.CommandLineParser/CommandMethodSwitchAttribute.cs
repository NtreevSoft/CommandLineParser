using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandMethodSwitchAttribute : Attribute
    {
        private readonly string propertyName;

        public CommandMethodSwitchAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public string PropertyName
        {
            get { return this.propertyName; }
        }
    }
}
