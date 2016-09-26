using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class UsageDescriptionProvider : IUsageDescriptionProvider
    {
        public string GetDescription(PropertyDescriptor descriptor)
        {
            return descriptor.Description;
        }

        public string GetDescription(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        public string GetDescription(ParameterInfo parameterInfo)
        {
            return parameterInfo.GetDescription();
        }

        public string GetDescription(object instance)
        {
            return instance.GetType().GetDescription();
        }

        public string GetSummary(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetSummary();
        }

        public string GetSummary(ParameterInfo parameterInfo)
        {
            return parameterInfo.GetSummary();
        }

        public string GetSummary(PropertyDescriptor descriptor)
        {
            return descriptor.GetSummary();
        }

        public string GetSummary(object instance)
        {
            return instance.GetType().GetSummary();
        }

        public static readonly UsageDescriptionProvider Default = new UsageDescriptionProvider();
    }
}
