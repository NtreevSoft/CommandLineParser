using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CommandMethodAttribute : Attribute
    {
        private readonly string name;

        public CommandMethodAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return this.name ?? string.Empty; }
        }
    }
}
