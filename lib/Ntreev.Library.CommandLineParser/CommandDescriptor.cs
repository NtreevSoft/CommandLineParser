using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Ntreev.Library
{
    public static class CommandDescriptor
    {
        private static Dictionary<Type, SwitchDescriptorCollection> typeToSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();
        private static Dictionary<Type, SwitchDescriptorCollection> typeToMethodSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();

        public static MethodDescriptor GetMethodDescriptor(Type type, string methodName)
        {
            return GetMethodDescriptors(type)[methodName];
        }

        public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return GetMethodDescriptors(instance)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            if(instance is Type)
                return GetMethodDescriptors(instance as Type);
            return GetMethodDescriptors(instance.GetType());
        }

        public static MethodDescriptorCollection GetMethodDescriptors(Type type)
        {
            if (typeToMethodDescriptors.ContainsKey(type) == false)
            {
                var descriptors = new MethodDescriptorCollection();

                foreach (var item in type.GetMethods())
                {
                    if (item.IsCommandMethod() == false)
                        continue;
                    descriptors.Add(new MethodDescriptor(item));
                }
                typeToMethodDescriptors.Add(type, descriptors);
            }
            return typeToMethodDescriptors[type];
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(object instance)
        {
            if (instance is Type)
                return GetSwitchDescriptors(instance as Type);
            return GetSwitchDescriptors(instance.GetType());
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(Type type)
        {
            if (typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                typeToSwitchDescriptors.Add(type, CreateSwitches(type));
            }
            return typeToSwitchDescriptors[type];
        }

        public static SwitchDescriptorCollection GetMethodSwitchDescriptors(object instance)
        {
            if (instance is Type)
                return GetMethodSwitchDescriptors(instance as Type);
            return GetMethodSwitchDescriptors(instance.GetType());
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
