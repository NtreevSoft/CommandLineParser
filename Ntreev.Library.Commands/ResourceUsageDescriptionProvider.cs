using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public class ResourceUsageDescriptionProvider : IUsageDescriptionProvider
    {
        private const string extension = ".resources";
        private readonly static Dictionary<string, ResourceManager> resourceSets = new Dictionary<string, ResourceManager>();
        private readonly string resourceName;

        static ResourceUsageDescriptionProvider()
        {

        }

        public ResourceUsageDescriptionProvider()
            : this(string.Empty)
        {

        }

        public ResourceUsageDescriptionProvider(string resourceName)
        {
            this.resourceName = resourceName ?? string.Empty;
        }

        public static string GetString(Assembly assembly, string resourceName, string name)
        {
            var resourceSet = GetResourceSet(resourceName, assembly);
            if (resourceSet == null)
                return null;
            return resourceSet.GetString(name);
        }

        public string GetDescription(PropertyInfo propertyInfo)
        {
            var description = GetResourceDescription(this.resourceName, propertyInfo.DeclaringType, propertyInfo.Name, this.IsShared);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(propertyInfo);
        }

        public string GetDescription(ParameterInfo parameterInfo)
        {
            var description = GetResourceDescription(this.resourceName, parameterInfo.Member.DeclaringType, string.Join(".", parameterInfo.Member.Name, parameterInfo.Name), this.IsShared);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(parameterInfo);
        }

        public string GetDescription(object instance)
        {
            var description = GetResourceDescription(this.resourceName, instance.GetType(), instance.GetType().Name, this.IsShared);
            if (description != null)
                return description;
            description = GetResourceDescription(this.resourceName, instance.GetType(), "ctor", this.IsShared);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(instance);
        }

        public string GetDescription(MethodInfo methodInfo)
        {
            var description = GetResourceDescription(this.resourceName, methodInfo.DeclaringType, methodInfo.Name, this.IsShared);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(methodInfo);
        }

        public string GetSummary(PropertyInfo propertyInfo)
        {
            var summary = GetResourceSummary(this.resourceName, propertyInfo.DeclaringType, propertyInfo.Name, this.IsShared);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(propertyInfo);
        }

        public string GetSummary(ParameterInfo parameterInfo)
        {
            var summary = GetResourceSummary(this.resourceName, parameterInfo.Member.DeclaringType, string.Join(".", parameterInfo.Member.Name, parameterInfo.Name), this.IsShared);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(parameterInfo);
        }

        public string GetSummary(object instance)
        {
            var summary = GetResourceSummary(this.resourceName, instance.GetType(), instance.GetType().Name, this.IsShared);
            if (summary != null)
                return summary;
            summary = GetResourceSummary(this.resourceName, instance.GetType(), "ctor", this.IsShared);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(instance);
        }

        public string GetSummary(MethodInfo methodInfo)
        {
            var summary = GetResourceSummary(this.resourceName, methodInfo.DeclaringType, methodInfo.Name, this.IsShared);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(methodInfo);
        }

        public bool IsShared
        {
            get; set;
        }

        private static string GetResourceDescription(string resourceName, Type type, string name, bool isShared)
        {
            var resourceSet = GetResourceSet(resourceName, type);
            if (resourceSet == null)
                return null;
            var resName = name;
            if (isShared == true && type.Name != name)
            {
                resName = $"{type.Name}.{name}";
            }
            return resourceSet.GetString(resName);
        }

        private static string GetResourceSummary(string resourceName, Type type, string name, bool isShared)
        {
            var resourceSet = GetResourceSet(resourceName, type);
            if (resourceSet == null)
                return null;
            var resName = name;
            if (isShared == true && type.Name != name)
            {
                resName = $"{type.Name}.{name}";
            }
            return resourceSet.GetString("@" + resName);
        }

        private static ResourceManager GetResourceSet(string resourceName, Type type)
        {
            var resourceNames = type.Assembly.GetManifestResourceNames();
            var baseName = resourceName == string.Empty ? type.FullName : resourceName;

            if (resourceNames.Contains(baseName + extension) == false)
                return null;

            if (resourceSets.ContainsKey(baseName) == false)
                resourceSets.Add(baseName, new ResourceManager(baseName, type.Assembly));

            return resourceSets[baseName];
        }

        private static ResourceManager GetResourceSet(string resourceName, Assembly assembly)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            var baseName = resourceName;

            if (resourceNames.Contains(baseName + extension) == false)
                return null;

            if (resourceSets.ContainsKey(baseName) == false)
                resourceSets.Add(baseName, new ResourceManager(baseName, assembly));

            return resourceSets[baseName];
        }
    }
}
