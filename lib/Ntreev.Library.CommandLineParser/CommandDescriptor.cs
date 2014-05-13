using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Ntreev.Library
{
    using static_this = CommandDescriptor;

    public static class CommandDescriptor
    {
        private static Dictionary<Type, SwitchDescriptorCollection> typeToSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();

        public static MethodDescriptor GetMethodDescriptor(Type type, string methodName)
        {
            return static_this.GetMethodDescriptors(type)[methodName];
        }

        public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return static_this.GetMethodDescriptors(instance)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            if(instance is Type)
                return static_this.GetMethodDescriptors(instance as Type);
            return static_this.GetMethodDescriptors(instance.GetType());
        }

        public static MethodDescriptorCollection GetMethodDescriptors(Type type)
        {
            if (static_this.typeToMethodDescriptors.ContainsKey(type) == false)
            {
                MethodDescriptorCollection descriptors = new MethodDescriptorCollection();

                foreach (MethodInfo item in type.GetMethods())
                {
                    if (item.IsCommandMethod() == false)
                        continue;
                    descriptors.List.Add(new MethodDescriptor(item));
                }
                static_this.typeToMethodDescriptors.Add(type, descriptors);
            }
            return static_this.typeToMethodDescriptors[type];
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(object instance)
        {
            if (static_this.typeToSwitchDescriptors.ContainsKey(instance.GetType()) == false)
            {
                static_this.CreateSwitches(instance);
            }
            return static_this.GetSwitchDescriptors(instance.GetType());
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(Type type)
        {
            if (static_this.typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                static_this.CreateSwitches(type);
            }

            return static_this.typeToSwitchDescriptors[type];
        }

        private static void CreateSwitches(Type type)
        {
            SwitchDescriptorCollection switches = new SwitchDescriptorCollection();

            foreach (PropertyInfo item in type.GetProperties())
            {
                if(item.GetBrowsable() == false || item.CanRead == false || item.CanWrite == false)
                    continue;

                Parser parser = Parser.GetParser(item);

                if (parser == null)
                    continue;

                switches.List.Add(new SwitchDescriptor(item));
            }

            static_this.typeToSwitchDescriptors.Add(type, switches);
        }

        private static void CreateSwitches(object instance)
        {
            SwitchDescriptorCollection switches = new SwitchDescriptorCollection();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

            foreach (PropertyDescriptor item in properties)
            {
                if (item.IsBrowsable == false || item.IsReadOnly == true)
                    continue;

                Parser parser = Parser.GetParser(item);

                if (parser == null)
                    continue;

                switches.List.Add(new SwitchDescriptor(item));
            }

            static_this.typeToSwitchDescriptors.Add(instance.GetType(), switches);
        }
    }
}
