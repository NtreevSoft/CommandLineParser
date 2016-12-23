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
        private static Dictionary<Type, SwitchDescriptorCollection> typeToSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();
        private static Dictionary<Type, SwitchDescriptorCollection> typeToMethodSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<ICustomAttributeProvider, SwitchDescriptorCollection> providerToSwitchDescriptors = new Dictionary<ICustomAttributeProvider, SwitchDescriptorCollection>();
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

        public static MethodDescriptor GetMethodDescriptor(Type type, string methodName)
        {
            return GetMethodDescriptors(type)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(Type type)
        {
            if (typeToMethodDescriptors.ContainsKey(type) == false)
            {
                typeToMethodDescriptors.Add(type, CreateMethodDescriptors(type));
            }
            return typeToMethodDescriptors[type];
        }

        public static SwitchDescriptorCollection GetPropertyDescriptors(Type type)
        {
            if (typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToSwitchDescriptors.Add(type, CreatePropertyDescriptors(type));
            }
            return typeToSwitchDescriptors[type];
        }

        public static SwitchDescriptorCollection GetStaticPropertyInfoDescriptors(ICustomAttributeProvider provider)
        {
            if (providerToSwitchDescriptors.ContainsKey(provider) == false)
            {
                providerToSwitchDescriptors.Add(provider, CreateStaticPropertyInfoDescriptors(provider));
            }

            return providerToSwitchDescriptors[provider];
        }
        

        public static SwitchDescriptorCollection GetPropertyInfoDescriptors(Type type)
        {
            if (typeToMethodSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToMethodSwitchDescriptors.Add(type, CreatePropertyInfoDescriptors(type));
            }

            return typeToMethodSwitchDescriptors[type];
        }

        public static SwitchDescriptorCollection CreateStaticPropertyInfoDescriptors(ICustomAttributeProvider provider)
        {
            var switches = new SwitchDescriptorCollection();
            var attrs = provider.GetCustomAttributes(typeof(CommandStaticSwitchAttribute), true);

            foreach (var item in attrs)
            {
                if (item is CommandStaticSwitchAttribute == false)
                    continue;
                var attr = item as CommandStaticSwitchAttribute;

                switches.AddRange(CommandDescriptor.GetPropertyInfoDescriptors(attr.StaticType));
            }

            return switches;
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

        private static SwitchDescriptorCollection CreatePropertyInfoDescriptors(Type type)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                if (item.CanWrite == false)
                    throw new Exception(string.Format("'{0}' is not available because it cannot write.", item.Name));

                switches.Add(new SwitchPropertyInfoDescriptor(item));
            }

            switches.Sort();

            return switches;
        }

        private static SwitchDescriptorCollection CreatePropertyDescriptors(Type type)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor item in properties)
            {
                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                if (item.IsReadOnly == true)
                    throw new Exception(string.Format("'{0}' is not available because it is read-only.", item.Name));

                switches.Add(new SwitchPropertyDescriptor(item));
            }

            switches.Sort();

            return switches;
        }
    }
}
