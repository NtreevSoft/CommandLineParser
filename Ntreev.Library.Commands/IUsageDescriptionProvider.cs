using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public interface IUsageDescriptionProvider
    {
        string GetDescription(object instance);

        string GetSummary(object instance);

        string GetDescription(PropertyDescriptor descriptor);

        string GetSummary(PropertyDescriptor descriptor);

        string GetDescription(PropertyInfo propertyInfo);

        string GetSummary(PropertyInfo propertyInfo);

        string GetDescription(ParameterInfo parameterInfo);

        string GetSummary(ParameterInfo parameterInfo);

        string GetDescription(MethodInfo methodInfo);

        string GetSummary(MethodInfo methodInfo);
    }
}
