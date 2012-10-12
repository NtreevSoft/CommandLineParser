using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Ntreev.Library
{
    public class MethodDescriptor
    {
        private readonly MethodInfo methodInfo;
        private readonly CommandMethodAttribute attribute;
        private MethodUsageProvider usageProvider;
        private readonly Dictionary<string, SwitchDescriptor> nameToStwich = new Dictionary<string, SwitchDescriptor>();

        public MethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.attribute = this.methodInfo.GetCommandMethodAttribute();

            if (this.attribute.UsageProvider == null)
            {
                this.usageProvider = new InternalMethodUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                   null,
                   this.attribute.UsageProvider,
                   new Type[] { typeof(MethodDescriptor), },
                   new object[] { this, }) as MethodUsageProvider;
            }

            foreach (ParameterInfo item in methodInfo.GetParameters())
            {
                SwitchDescriptor switchDescriptor = new SwitchDescriptor(item);
                nameToStwich.Add(switchDescriptor.Name, switchDescriptor);
            }

            CommandMethodSwitchAttribute attr = this.methodInfo.GetCustomAttribute<CommandMethodSwitchAttribute>();
            if(attr != null)
            {
                foreach(string item in attr.PropertyNames)
                {
                    SwitchDescriptor switchDescriptor = CommandDescriptor.GetSwitchDescriptors(methodInfo.DeclaringType)[item];
                    nameToStwich.Add(switchDescriptor.Name, switchDescriptor);
                }
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this.attribute.Name) == true)
                    return this.methodInfo.Name;
                return this.attribute.Name;
            }
        }

        public MethodUsageProvider UsageProvider
        {
            get { return this.usageProvider; }
        }

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        public SwitchDescriptor[] Switches
        {
            get { return this.nameToStwich.Values.ToArray(); }
        }

        public string Description
        {
            get { return this.methodInfo.GetDescription(); }
        }

        internal void Invoke(string[] parameters, bool caseSensitive)
        {
            this.Invoke(null, parameters, caseSensitive);
        }

        internal void Invoke(object instance, string[] parameters, bool caseSensitive)
        {
            SwitchHelper helper = new SwitchHelper(this.nameToStwich.Values);
            helper.Parse(instance, parameters, caseSensitive);

            ArrayList values = new ArrayList();

            foreach (ParameterInfo item in methodInfo.GetParameters())
            {
                SwitchDescriptor switchDescriptor = this.nameToStwich[item.Name];

                object value = switchDescriptor.GetVaue(instance);;
                values.Add(value);
            }

            this.methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
