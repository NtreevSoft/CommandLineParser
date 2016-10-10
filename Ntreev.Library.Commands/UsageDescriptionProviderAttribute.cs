using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageDescriptionProviderAttribute : Attribute
    {
        private readonly Type type;

        public UsageDescriptionProviderAttribute(string typeName)
            : this(Type.GetType(typeName))
        {
            
        }

        public UsageDescriptionProviderAttribute(Type type)
        {
            if (typeof(IUsageDescriptionProvider).IsAssignableFrom(type) == false)
                throw new ArgumentException();
            this.type = type;
        }

        internal IUsageDescriptionProvider CreateInstance()
        {
            return TypeDescriptor.CreateInstance(null, this.type, null, null) as IUsageDescriptionProvider;
        }
    }
}
