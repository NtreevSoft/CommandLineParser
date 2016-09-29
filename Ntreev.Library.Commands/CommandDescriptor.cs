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
        //private static Dictionary<Type, MethodDescriptor> typeToCommandDescriptors = new Dictionary<Type, MethodDescriptor>();
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();
        private static Dictionary<Type, SwitchDescriptorCollection> typeToMethodSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<Type, IUsageDescriptionProvider> typeToUsageDescriptionProvider = new Dictionary<Type, IUsageDescriptionProvider>();

        //public static MethodDescriptor GetCommandDescriptor(Type type)
        //{
        //    if (typeToCommandDescriptors.ContainsKey(type) == false)
        //    {
        //        var query = from item in type.GetMethods()
        //                    where item.IsCommand()
        //                    select item;

        //        var method = query.SingleOrDefault();

        //        if (method == null)
        //            throw new SwitchException("커맨드로 설정될수 있는 메소드는 오직 하나여야만 합니다.");

        //        var attr = method.GetCustomAttribute<DefaultCommandAttribute>();

        //        typeToCommandDescriptors.Add(type, new MethodDescriptor(method));
        //    }
        //    return typeToCommandDescriptors[type];
        //}

        //public static bool HasCommandDescriptor(Type type)
        //{
        //    var query = from item in type.GetMethods()
        //                where item.IsCommand()
        //                select item;

        //    return query.SingleOrDefault() != null;
        //}

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

        //public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        //{
        //    return GetMethodDescriptors(instance)[methodName];
        //}

        //public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        //{
        //    if(instance is Type)
        //        return GetMethodDescriptors(instance as Type);
        //    return GetMethodDescriptors(instance.GetType());
        //}

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
                    var name = attr.Name != string.Empty ? attr.Name : item.Name.ToSpinalCase();
                    descriptors.Add(new MethodDescriptor(item, name));
                }
                typeToMethodDescriptors.Add(type, descriptors);
            }
            return typeToMethodDescriptors[type];
        }

        //public static SwitchDescriptorCollection GetSwitchDescriptors(object instance)
        //{
        //    if (instance is Type)
        //        return GetSwitchDescriptors(instance as Type);
        //    return GetSwitchDescriptors(instance.GetType());
        //}

        public static SwitchDescriptorCollection GetSwitchDescriptors(Type type)
        {
            if (typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToSwitchDescriptors.Add(type, CreateSwitches(type));
            }
            return typeToSwitchDescriptors[type];
        }

        //public static SwitchDescriptorCollection GetMethodSwitchDescriptors(object instance)
        //{
        //    if (instance is Type)
        //        return GetMethodSwitchDescriptors(instance as Type);
        //    return GetMethodSwitchDescriptors(instance.GetType());
        //}

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
                if (item.GetBrowsable() == false || item.CanWrite == false)
                    continue;

                var parser = Parser.GetParser(item);

                if (parser == null)
                    continue;

                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                switches.Add(new SwitchDescriptor(item));
            }

            return switches;
        }

        private static SwitchDescriptorCollection CreateSwitches(Type type)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor item in properties)
            {
                if (item.IsBrowsable == false || item.IsReadOnly == true)
                    continue;

                var parser = Parser.GetParser(item);

                if (parser == null)
                    continue;

                if (item.GetCommandSwitchAttribute() == null)
                    continue;

                switches.Add(new SwitchDescriptor(item));
            }

            return switches;
        }
    }
}
