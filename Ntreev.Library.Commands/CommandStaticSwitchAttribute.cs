using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CommandStaticSwitchAttribute : Attribute
    {
        private readonly string typeName;
        private readonly Type type;

        public CommandStaticSwitchAttribute(string typeName)
            : this(Type.GetType(typeName))
        {
            
        }

        public CommandStaticSwitchAttribute(Type type)
        {
            if (type.GetConstructor(Type.EmptyTypes) == null && type.IsAbstract && type.IsSealed)
            {
                this.type = type;
                this.typeName = type.AssemblyQualifiedName;
            }
            else
            {
                throw new NotImplementedException("type is not static class.");
            }
        }

        public string TypeName
        {
            get { return this.typeName; }
        }

        internal Type StaticType
        {
            get { return this.type; }
        }
    }
}
