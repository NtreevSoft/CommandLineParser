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
                var descriptors = new MethodDescriptorCollection();

                foreach (var item in type.GetMethods())
                {
                    var attr = item.GetCustomAttribute<CommandMethodAttribute>();
                    if (attr == null)
                        continue;
                    descriptors.Add(new MethodDescriptor(item, attr));
                }
                typeToMethodDescriptors.Add(type, descriptors);
            }
            return typeToMethodDescriptors[type];
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(Type type)
        {
            if (typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToSwitchDescriptors.Add(type, CreateSwitches(type));
            }
            return typeToSwitchDescriptors[type];
        }

        public static SwitchDescriptorCollection GetMethodSwitchDescriptors(Type type)
        {
            if (typeToMethodSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToMethodSwitchDescriptors.Add(type, CreateMethodSwitches(type));
            }

            return typeToMethodSwitchDescriptors[type];
        }

        private static SwitchDescriptorCollection CreateMethodSwitches(Type type)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = type.GetProperties();

            foreach (var item in properties)
            {
                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                if (item.CanWrite == false)
                    throw new Exception(string.Format("'{0}'은(는) 쓰기 작업이 불가능하기 때문에 스위치로 설정할 수 없습니다.", item.Name));

                if (item.GetBrowsable() == false || item.CanWrite == false)
                    continue;

                switches.Add(new SwitchPropertyInfoDescriptor(item));
            }

            switches.Sort();

            return switches;
        }

        private static SwitchDescriptorCollection CreateSwitches(Type type)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor item in properties)
            {
                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                if (item.IsReadOnly == true)
                    throw new Exception(string.Format("'{0}'은(는) 읽기 전용이므로 스위치로 설정할 수 없습니다.", item.Name));

                if (item.IsBrowsable == false || item.IsReadOnly == true)
                    continue;

                switches.Add(new SwitchPropertyDescriptor(item));
            }

            switches.Sort();

            return switches;
        }
    }
}
