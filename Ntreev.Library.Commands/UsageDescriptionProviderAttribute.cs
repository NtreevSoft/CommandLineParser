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
        private readonly Type providerType;

        public UsageDescriptionProviderAttribute(string typeName)
            : this(Type.GetType(typeName))
        {

        }

        public UsageDescriptionProviderAttribute(Type type)
        {
            if (typeof(IUsageDescriptionProvider).IsAssignableFrom(type) == false)
                throw new ArgumentException();
            this.providerType = type;
        }

        public Type ProviderType
        {
            get { return this.providerType; }
        }

        protected virtual IUsageDescriptionProvider CreateInstance(Type type)
        {
            return TypeDescriptor.CreateInstance(null, this.ProviderType, null, null) as IUsageDescriptionProvider;
        }

        internal IUsageDescriptionProvider CreateInstanceInternal(Type type)
        {
            return this.CreateInstance(type);
        }
    }
}
