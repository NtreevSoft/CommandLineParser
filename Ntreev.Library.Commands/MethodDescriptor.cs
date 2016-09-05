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
        private readonly SwitchDescriptor[] switches;

        public MethodDescriptor(MethodInfo methodInfo)
        {
            this.methodInfo = methodInfo;
            this.attribute = this.methodInfo.GetCommandMethodAttribute();

            var switchList = new List<SwitchDescriptor>();

            foreach (var item in methodInfo.GetParameters())
            {
                var switchDescriptor = new SwitchDescriptor(item);
                switchList.Add(switchDescriptor);
            }

            var attr = this.methodInfo.GetCustomAttribute<CommandMethodSwitchAttribute>();
            if (attr != null)
            {
                foreach (var item in attr.PropertyNames)
                {
                    var switchDescriptor = CommandDescriptor.GetMethodSwitchDescriptors(methodInfo.DeclaringType)[item];
                    if (switchDescriptor == null)
                        throw new SwitchException(string.Format("{0} 은(는) 존재하지 않는 속성입니다.", item));
                    switchList.Add(switchDescriptor);
                }
            }

            this.switches = switchList.ToArray();
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

        public MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        public SwitchDescriptor[] Switches
        {
            get { return this.switches.ToArray(); }
        }

        public string Description
        {
            get { return this.methodInfo.GetDescription(); }
        }

        public string Summary
        {
            get { return this.methodInfo.GetSummary(); }
        }

        internal void Invoke(object instance, string arguments)
        {
            var helper = new SwitchHelper(this.switches);
            helper.Parse(instance, arguments);

            var values = new ArrayList();
            var s = this.switches.ToDictionary(item => item.Name);

            foreach (var item in methodInfo.GetParameters())
            {
                var switchDescriptor = s[item.Name];

                var value = switchDescriptor.GetVaue(instance);
                values.Add(value);
            }

            this.methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
