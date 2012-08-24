using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Ntreev.Library
{
    static class CommandDescriptor
    {
        private static Dictionary<Type, SwitchDescriptorCollection> typeToSwitchDescriptors = new Dictionary<Type, SwitchDescriptorCollection>();
        private static Dictionary<Type, MethodDescriptorCollection> typeToMethodDescriptors = new Dictionary<Type, MethodDescriptorCollection>();
        
        public static MethodDescriptor GetMethodDescriptor(Type type, string methodName)
        {
            return CommandDescriptor.GetMethodDescriptors(type)[methodName];
        }

        public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return CommandDescriptor.GetMethodDescriptors(instance)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            return CommandDescriptor.GetMethodDescriptors(instance.GetType());
        }

        public static MethodDescriptorCollection GetMethodDescriptors(Type type)
        {
            if (CommandDescriptor.typeToMethodDescriptors.ContainsKey(type) == false)
            {
                MethodDescriptorCollection descriptors = new MethodDescriptorCollection();

                foreach (MethodInfo item in type.GetMethods())
                {
                    if (item.IsCommandMethod() == false)
                        continue;
                    descriptors.Add(new MethodDescriptor(item));
                }
                CommandDescriptor.typeToMethodDescriptors.Add(type, descriptors);
            }
            return CommandDescriptor.typeToMethodDescriptors[type];
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(object instance)
        {
            return CommandDescriptor.GetSwitchDescriptors(instance.GetType());
        }

        public static SwitchDescriptorCollection GetSwitchDescriptors(Type type)
        {
            if (CommandDescriptor.typeToSwitchDescriptors.ContainsKey(type) == false)
            {
                CommandDescriptor.CreateSwitches(type);
            }

            return CommandDescriptor.typeToSwitchDescriptors[type];
        }

        private static void CreateSwitches(Type type)
        {
            SwitchDescriptorCollection switches = new SwitchDescriptorCollection();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);

            foreach (PropertyDescriptor item in properties)
            {
                if (item.IsBrowsable == false || item.IsReadOnly == true)
                    continue;

                if (item.IsReadOnly == true)
                {
                    if (item.PropertyType.IsValueType == true)
                        continue;
                }

                Parser parser = Parser.GetParser(item);

                if (parser == null)
                    continue;

                switches.Add(new SwitchDescriptor(item));
            }

            CommandDescriptor.typeToSwitchDescriptors.Add(type, switches);
        }
    }
}
