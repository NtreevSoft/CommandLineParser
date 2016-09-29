using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class ResourceUsageDescriptionProvider : IUsageDescriptionProvider
    {
        private readonly static Dictionary<Type, ResourceSet> resourceSets = new Dictionary<Type, ResourceSet>();

        static ResourceUsageDescriptionProvider()
        {

        }

        public ResourceUsageDescriptionProvider()
        {

        }

        public string GetDescription(PropertyDescriptor descriptor)
        {
            var description = GetResourceDescription(descriptor.ComponentType, descriptor.Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(descriptor);
        }

        public string GetDescription(PropertyInfo propertyInfo)
        {
            var description = GetResourceDescription(propertyInfo.DeclaringType, propertyInfo.Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(propertyInfo);
        }

        public string GetDescription(ParameterInfo parameterInfo)
        {
            var description = GetResourceDescription(parameterInfo.Member.DeclaringType, string.Join(".", parameterInfo.Member.Name, parameterInfo.Name));
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(parameterInfo);
        }

        public string GetDescription(object instance)
        {
            var description = GetResourceDescription(instance.GetType(), instance.GetType().Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(instance);
        }

        public string GetDescription(MethodInfo methodInfo)
        {
            var description = GetResourceDescription(methodInfo.DeclaringType, methodInfo.Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(methodInfo);
        }

        public string GetSummary(PropertyInfo propertyInfo)
        {
            var summary = GetResourceSummary(propertyInfo.DeclaringType, propertyInfo.Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(propertyInfo);
        }

        public string GetSummary(ParameterInfo parameterInfo)
        {
            var summary = GetResourceSummary(parameterInfo.Member.DeclaringType, string.Join(".", parameterInfo.Member.Name, parameterInfo.Name));
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(parameterInfo);
        }

        public string GetSummary(PropertyDescriptor descriptor)
        {
            var summary = GetResourceSummary(descriptor.ComponentType, descriptor.Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(descriptor);
        }

        public string GetSummary(object instance)
        {
            var summary = GetResourceSummary(instance.GetType(), instance.GetType().Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(instance);
        }

        public string GetSummary(MethodInfo methodInfo)
        {
            var summary = GetResourceSummary(methodInfo.DeclaringType, methodInfo.Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(methodInfo);
        }

        private static string GetResourceDescription(Type type, string name)
        {
            var resourceSet = GetResourceSet(type);
            if (resourceSet == null)
                return null;
            return resourceSet.GetString(name);
        }

        private static string GetResourceSummary(Type type, string name)
        {
            var resourceSet = GetResourceSet(type);
            if (resourceSet == null)
                return null;
            return resourceSet.GetString("@" + name);
        }

        private static ResourceSet GetResourceSet(Type type)
        {
            var resourceNames = type.Assembly.GetManifestResourceNames();
            if (resourceSets.ContainsKey(type) == false)
            {
                var stream = type.Assembly.GetManifestResourceStream(type.FullName + ".resources");

                if (stream == null)
                    return null;
                resourceSets.Add(type, new ResourceSet(stream));
                stream.Dispose();
            }

            return resourceSets[type];
        }
    }
}
