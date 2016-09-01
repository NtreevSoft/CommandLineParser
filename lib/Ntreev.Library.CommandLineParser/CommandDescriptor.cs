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

        public static MethodDescriptor GetMethodDescriptor(object instance, string methodName)
        {
            return GetMethodDescriptors(instance)[methodName];
        }

        public static MethodDescriptorCollection GetMethodDescriptors(object instance)
        {
            var type = instance.GetType();
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
            if (typeToSwitchDescriptors.ContainsKey(instance.GetType()) == false)
            {
                CreateSwitches(instance);
            }
            return typeToSwitchDescriptors[instance.GetType()];
        }

        private static void CreateSwitches(object instance)
        {
            var switches = new SwitchDescriptorCollection();
            var properties = TypeDescriptor.GetProperties(instance);

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

            typeToSwitchDescriptors.Add(instance.GetType(), switches);
        }
    }
}
