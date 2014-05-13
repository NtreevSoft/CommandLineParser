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
        private readonly List<SwitchDescriptor> switches = new List<SwitchDescriptor>();
        private readonly List<SwitchDescriptor> options = new List<SwitchDescriptor>();

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
                this.switches.Add(switchDescriptor);
            }


            CommandMethodSwitchAttribute attr = this.methodInfo.GetCustomAttribute<CommandMethodSwitchAttribute>();
            if(attr != null)
            {
                foreach(string item in attr.PropertyNames)
                {
                    SwitchDescriptor switchDescriptor = CommandDescriptor.GetSwitchDescriptors(methodInfo.DeclaringType)[item];
                    if (switchDescriptor.Required == true)
                        this.switches.Add(switchDescriptor);
                    else
                        this.options.Add(switchDescriptor);
                }
            }

            if (this.attribute.Name == CommandLineInvoker.defaultMethod)
            {
                if (this.switches.Count > 0 || this.options.Count > 0)
                    throw new SwitchException("default 메소드는 필수 또는 선택 인자를 가질 수 없습니다.");
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
            get { return this.switches.ToArray(); }
        }

        public SwitchDescriptor[] OptionSwitches
        {
            get { return this.options.ToArray(); }
        }

        public string Description
        {
            get { return this.methodInfo.GetDescription(); }
        }

        internal void Invoke(object target, string arguments)
        {
            if (target is Type)
                target = null;
            SwitchHelper helper = new SwitchHelper(this.switches, this.options);
            helper.Parse(target, arguments);

            ArrayList values = new ArrayList();

            IDictionary<string, SwitchDescriptor> s = this.switches.ToDictionary(item => item.Name);

            foreach (ParameterInfo item in methodInfo.GetParameters())
            {
                SwitchDescriptor switchDescriptor = s[item.Name];

                object value = switchDescriptor.GetVaue(target); ;
                values.Add(value);
            }

            this.methodInfo.Invoke(target, values.ToArray());
        }

        internal void Invoke(object instance, string[] parameters)
        {
            //SwitchHelper helper = new SwitchHelper(this.nameToStwich.Values);
            //helper.Parse(instance, parameters, caseSensitive);

            //ArrayList values = new ArrayList();

            //foreach (ParameterInfo item in methodInfo.GetParameters())
            //{
            //    SwitchDescriptor switchDescriptor = this.nameToStwich[item.Name];

            //    object value = switchDescriptor.GetVaue(instance);;
            //    values.Add(value);
            //}

            //this.methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
