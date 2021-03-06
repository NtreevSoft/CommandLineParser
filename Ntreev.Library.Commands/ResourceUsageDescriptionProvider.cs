﻿//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace Ntreev.Library.Commands
{
    public class ResourceUsageDescriptionProvider : IUsageDescriptionProvider
    {
        private const string extension = ".resources";
        private readonly static Dictionary<string, ResourceManager> resourceManagers = new Dictionary<string, ResourceManager>();
        private readonly string resourceName;
        private string prefix;

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
            var description = this.GetResourceDescription(propertyInfo.DeclaringType, propertyInfo.Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(propertyInfo);
        }

        public string GetDescription(ParameterInfo parameterInfo)
        {
            var items = new string[] { parameterInfo.Member.Name, parameterInfo.Name };
            var description = this.GetResourceDescription(parameterInfo.Member.DeclaringType, string.Join(".", items));
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(parameterInfo);
        }

        public string GetDescription(object instance)
        {
            var description = this.GetResourceDescription(instance.GetType(), instance.GetType().Name);
            if (description != null)
                return description;
            description = this.GetResourceDescription(instance.GetType(), "ctor");
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(instance);
        }

        public string GetDescription(MethodInfo methodInfo)
        {
            var description = this.GetResourceDescription(methodInfo.DeclaringType, methodInfo.Name);
            if (description != null)
                return description;
            return UsageDescriptionProvider.Default.GetDescription(methodInfo);
        }

        public string GetSummary(PropertyInfo propertyInfo)
        {
            var summary = this.GetResourceSummary(propertyInfo.DeclaringType, propertyInfo.Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(propertyInfo);
        }

        public string GetSummary(ParameterInfo parameterInfo)
        {
            var items = new string[] { parameterInfo.Member.Name, parameterInfo.Name };
            var summary = this.GetResourceSummary(parameterInfo.Member.DeclaringType, string.Join(".", items));
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(parameterInfo);
        }

        public string GetSummary(object instance)
        {
            var summary = this.GetResourceSummary(instance.GetType(), instance.GetType().Name);
            if (summary != null)
                return summary;
            summary = this.GetResourceSummary(instance.GetType(), "ctor");
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(instance);
        }

        public string GetSummary(MethodInfo methodInfo)
        {
            var summary = this.GetResourceSummary(methodInfo.DeclaringType, methodInfo.Name);
            if (summary != null)
                return summary;
            return UsageDescriptionProvider.Default.GetSummary(methodInfo);
        }

        public bool IsShared
        {
            get; set;
        }

        public string Prefix
        {
            get { return this.prefix ?? string.Empty; }
            set { this.prefix = value; }
        }

        private string GetResourceDescription(Type type, string name)
        {
            var resourceManager = GetResourceSet(this.resourceName, type);
            if (resourceManager == null)
                return null;
            var resName = name;
            if (this.IsShared == true && type.Name != name)
                resName = $"{type.Name}.{name}";
            if (this.Prefix != string.Empty)
                resName = $"{this.Prefix}.{resName}";

            return GetString(resourceManager, resName);
        }

        private string GetResourceSummary(Type type, string name)
        {
            var resourceManager = GetResourceSet(this.resourceName, type);
            if (resourceManager == null)
                return null;
            var resName = name;
            if (this.IsShared == true && type.Name != name)
                resName = $"{type.Name}.{name}";
            if (this.Prefix != string.Empty)
                resName = $"{this.Prefix}.{resName}";

            return GetString(resourceManager, "@" + resName);
        }

        private static ResourceManager GetResourceSet(string resourceName, Type type)
        {
            var resourceNames = type.Assembly.GetManifestResourceNames();
            var baseName = resourceName == string.Empty ? type.FullName : resourceName;

            if (resourceNames.Contains(baseName + extension) == false)
                return null;

            if (resourceManagers.ContainsKey(baseName) == false)
                resourceManagers.Add(baseName, new ResourceManager(baseName, type.Assembly));

            return resourceManagers[baseName];
        }

        private static ResourceManager GetResourceSet(string resourceName, Assembly assembly)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            var baseName = resourceName;

            if (resourceNames.Contains(baseName + extension) == false)
                return null;

            if (resourceManagers.ContainsKey(baseName) == false)
                resourceManagers.Add(baseName, new ResourceManager(baseName, assembly));

            return resourceManagers[baseName];
        }

        private static string GetString(ResourceManager resourceManager, string resName)
        {
            var text = resourceManager.GetString(resName);
            if (text != null && text.StartsWith("#"))
            {
                var text2 = resourceManager.GetString(text);
                if (text2 != null)
                    return text2;
            }
            return text;
        }
    }
}
