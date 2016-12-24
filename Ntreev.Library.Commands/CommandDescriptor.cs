using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Ntreev.Library.Commands
{
    public static class CommandDescriptor
    {
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();
        private static Dictionary<Type, SwitchDescriptorCollection> typeToswitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<ICustomAttributeProvider, SwitchDescriptorCollection> providerToSwitchDescriptors = new Dictionary<ICustomAttributeProvider, SwitchDescriptorCollection>();
        private static Dictionary<ICustomAttributeProvider, MethodDescriptorCollection> providerToMethodDescriptors = new Dictionary<ICustomAttributeProvider, MethodDescriptorCollection>();
        private static Dictionary<Type, IUsageDescriptionProvider> typeToUsageDescriptionProvider = new Dictionary<Type, IUsageDescriptionProvider>();

        public static IUsageDescriptionProvider GetUsageDescriptionProvider(Type type)
        {
            var attribute = type.GetCustomAttribute<UsageDescriptionProviderAttribute>();
            if (attribute == null)
                return UsageDescriptionProvider.Default;
            if (typeToUsageDescriptionProvider.ContainsKey(type) == false)
            {
                typeToUsageDescriptionProvider.Add(type, attribute.CreateInstance());
            }
            return typeToUsageDescriptionProvider[type];
        }

        public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return GetMethodDescriptors(instance)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            var type = instance is Type ? (Type)instance : instance.GetType();
            if (typeToMethodDescriptors.ContainsKey(type) == false)
            {
                typeToMethodDescriptors.Add(type, CreateMethodDescriptors(type));
            }
            return typeToMethodDescriptors[type];
        }

        public static MethodDescriptorCollection GetStaticMethodDescriptors(ICustomAttributeProvider provider)
        {
            if (providerToMethodDescriptors.ContainsKey(provider) == false)
            {
                providerToMethodDescriptors.Add(provider, CreateStaticMethodDescriptors(provider));
            }

            return providerToMethodDescriptors[provider];
        }

        public static SwitchDescriptorCollection GetStaticSwitchDescriptors(ICustomAttributeProvider provider)
        {
            if (providerToSwitchDescriptors.ContainsKey(provider) == false)
            {
                providerToSwitchDescriptors.Add(provider, CreateStaticSwitchDescriptors(provider));
            }

            return providerToSwitchDescriptors[provider];
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(object instance)
        {
            var type = instance is Type ? (Type)instance : instance.GetType();
            if (typeToswitchDescriptors.ContainsKey(type) == false)
            {
                typeToswitchDescriptors.Add(type, CreateSwitchDescriptors(type));
            }

            return typeToswitchDescriptors[type];
        }

        public static SwitchDescriptorCollection CreateStaticSwitchDescriptors(ICustomAttributeProvider provider)
        {
            var descriptors = new SwitchDescriptorCollection();
            var attrs = provider.GetCustomAttributes(typeof(CommandStaticSwitchAttribute), true);

            foreach (var item in attrs)
            {
                if (item is CommandStaticSwitchAttribute == false)
                    continue;
                var attr = item as CommandStaticSwitchAttribute;

                var staticDescriptors = CommandDescriptor.GetSwitchDescriptors(attr.StaticType);
                descriptors.AddRange(Filter(staticDescriptors, attr.PropertyNames));
            }

            return descriptors;
        }

        public static MethodDescriptorCollection CreateStaticMethodDescriptors(ICustomAttributeProvider provider)
        {
            var descriptors = new MethodDescriptorCollection();
            var attrs = provider.GetCustomAttributes(typeof(CommandStaticSwitchAttribute), true);

            foreach (var item in attrs)
            {
                if (item is CommandStaticMethodAttribute == false)
                    continue;
                var attr = item as CommandStaticMethodAttribute;

                descriptors.AddRange(CommandDescriptor.GetMethodDescriptors(attr.StaticType));
            }

            return descriptors;
        }

        public static MethodDescriptorCollection CreateMethodDescriptors(Type type)
        {
            var descriptors = new MethodDescriptorCollection();

            foreach (var item in type.GetMethods())
            {
                var attr = item.GetCustomAttribute<CommandMethodAttribute>();
                if (attr == null)
                    continue;
                descriptors.Add(new MethodDescriptor(item));
            }

            return descriptors;
        }

        private static SwitchDescriptorCollection CreateSwitchDescriptors(Type type)
        {
            var descriptors = new SwitchDescriptorCollection();
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                if (item.CanWrite == false)
                    throw new Exception(string.Format("'{0}' is not available because it cannot write.", item.Name));

                descriptors.Add(new SwitchPropertyInfoDescriptor(item));
            }

            foreach(var item in GetStaticSwitchDescriptors(type))
            {
                descriptors.Add(item);
            }

            descriptors.Sort();

            return descriptors;
        }

        private static IEnumerable<SwitchDescriptor> Filter(SwitchDescriptorCollection descriptors, params string[] propertyNames)
        {
            if (propertyNames.Any() == false)
            {
                foreach (var item in descriptors)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in propertyNames)
                {
                    yield return descriptors[item];
                }
            }
        }
    }
}
