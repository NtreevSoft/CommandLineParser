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
        private static Dictionary<object, CommandMethodDescriptorCollection> instanceToMethodDescriptors = new Dictionary<object, CommandMethodDescriptorCollection>();
        private static Dictionary<Type, CommandMethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, CommandMethodDescriptorCollection>();
        private static Dictionary<Type, CommandMemberDescriptorCollection> typeToMemberDescriptors = new Dictionary<Type, CommandMemberDescriptorCollection>();
        private static Dictionary<ICustomAttributeProvider, CommandMemberDescriptorCollection> providerToMemberDescriptors = new Dictionary<ICustomAttributeProvider, CommandMemberDescriptorCollection>();
        private static Dictionary<ICustomAttributeProvider, CommandMethodDescriptorCollection> providerToMethodDescriptors = new Dictionary<ICustomAttributeProvider, CommandMethodDescriptorCollection>();
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

        public static CommandMethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return GetMethodDescriptors(instance)[methodName];
        }

        public static CommandMethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            var type = instance is Type ? (Type)instance : instance.GetType();
            if (typeToMethodDescriptors.ContainsKey(type) == false)
            {
                typeToMethodDescriptors.Add(type, CreateMethodDescriptors(type));
            }
            return typeToMethodDescriptors[type];
        }

        public static CommandMethodDescriptorCollection GetStaticMethodDescriptors(ICustomAttributeProvider provider)
        {
            if (providerToMethodDescriptors.ContainsKey(provider) == false)
            {
                providerToMethodDescriptors.Add(provider, CreateStaticMethodDescriptors(provider));
            }

            return providerToMethodDescriptors[provider];
        }

        public static CommandMemberDescriptorCollection GetStaticMemberDescriptors(ICustomAttributeProvider provider)
        {
            if (providerToMemberDescriptors.ContainsKey(provider) == false)
            {
                providerToMemberDescriptors.Add(provider, CreateStaticMemberDescriptors(provider));
            }

            return providerToMemberDescriptors[provider];
        }

        public static CommandMemberDescriptorCollection GetMemberDescriptors(object instance)
        {
            var type = instance is Type ? (Type)instance : instance.GetType();
            if (typeToMemberDescriptors.ContainsKey(type) == false)
            {
                typeToMemberDescriptors.Add(type, CreateMemberDescriptors(type));
            }

            return typeToMemberDescriptors[type];
        }

        public static CommandMemberDescriptorCollection CreateStaticMemberDescriptors(ICustomAttributeProvider provider)
        {
            var descriptors = new CommandMemberDescriptorCollection();
            var attrs = provider.GetCustomAttributes(typeof(CommandStaticPropertyAttribute), true);

            foreach (var item in attrs)
            {
                if (item is CommandStaticPropertyAttribute == false)
                    continue;
                var attr = item as CommandStaticPropertyAttribute;

                var staticDescriptors = CommandDescriptor.GetMemberDescriptors(attr.StaticType);
                descriptors.AddRange(Filter(staticDescriptors, attr.PropertyNames));
            }

            return descriptors;
        }

        public static CommandMethodDescriptorCollection CreateStaticMethodDescriptors(ICustomAttributeProvider provider)
        {
            var descriptors = new CommandMethodDescriptorCollection();
            var attrs = provider.GetCustomAttributes(typeof(CommandStaticMethodAttribute), true);

            foreach (var item in attrs)
            {
                if (item is CommandStaticMethodAttribute == false)
                    continue;
                var attr = item as CommandStaticMethodAttribute;

                var staticDescriptors = CommandDescriptor.GetMethodDescriptors(attr.StaticType);
                descriptors.AddRange(Filter(staticDescriptors, attr.MethodNames));
            }

            return descriptors;
        }

        public static CommandMethodDescriptorCollection CreateMethodDescriptors(Type type)
        {
            var descriptors = new CommandMethodDescriptorCollection();

            foreach (var item in type.GetMethods())
            {
                var attr = item.GetCustomAttribute<CommandMethodAttribute>();
                if (attr == null)
                    continue;
                descriptors.Add(new StandardCommandMethodDescriptor(item));
            }

            foreach (var item in GetStaticMethodDescriptors(type))
            {
                descriptors.Add(item);
            }

            return descriptors;
        }

        private static CommandMemberDescriptorCollection CreateMemberDescriptors(Type type)
        {
            var descriptors = new CommandMemberDescriptorCollection();
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                var attr = item.GetCommandPropertyAttribute();
                if (attr == null)
                    continue;

                if (item.CanWrite == false)
                    throw new Exception(string.Format("'{0}' is not available because it cannot write.", item.Name));

                if (attr is CommandPropertyArrayAttribute == true)
                    descriptors.Add(new CommandPropertyArrayDescriptor(item));
                else
                    descriptors.Add(new CommandPropertyDescriptor(item));
            }

            foreach (var item in GetStaticMemberDescriptors(type))
            {
                descriptors.Add(item);
            }

            if (descriptors.Where(item => item is CommandMemberArrayDescriptor).Count() > 1)
                throw new InvalidOperationException("CommandPropertyArrayDescriptor is can be used only once.");

            descriptors.Sort();

            return descriptors;
        }

        private static IEnumerable<CommandMemberDescriptor> Filter(CommandMemberDescriptorCollection descriptors, params string[] propertyNames)
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

        private static IEnumerable<CommandMethodDescriptor> Filter(CommandMethodDescriptorCollection descriptors, params string[] methodNames)
        {
            if (methodNames.Any() == false)
            {
                foreach (var item in descriptors)
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in methodNames)
                {
                    yield return descriptors[item];
                }
            }
        }
    }
}
