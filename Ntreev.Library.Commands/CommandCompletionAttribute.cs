using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class CommandCompletionAttribute : Attribute
    {
        private readonly string methodName;
        private Type type;

        public CommandCompletionAttribute(string methodName)
        {
            this.methodName = methodName;
        }

        public string MethodName
        {
            get { return this.methodName; }
        }

        public string TypeName
        {
            get { return this.type.AssemblyQualifiedName; }
            set { this.type = Type.GetType(value); }
        }

        public Type Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    }
}
