using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Ntreev.Library.Commands
{
    public class MethodDescriptor
    {
        private readonly MethodInfo methodInfo;
        private readonly string originalName;
        private readonly string name;
        private readonly string displayName;
        private readonly SwitchDescriptor[] switches;

        internal MethodDescriptor(MethodInfo methodInfo)
            : this(methodInfo, methodInfo.Name.ToSpinalCase())
        {

        }

        internal MethodDescriptor(MethodInfo methodInfo, string name)
        { 
            this.methodInfo = methodInfo;
            this.originalName = methodInfo.Name;
            this.name = name;
            this.displayName = methodInfo.GetDisplayName();

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
                        throw new ArgumentException(string.Format("{0} 은(는) 존재하지 않는 속성입니다.", item));
                    switchList.Add(switchDescriptor);
                }
            }

            this.switches = switchList.ToArray();
        }

        public string OriginalName
        {
            get { return this.originalName; }
        }

        public string Name
        {
            get { return this.name; }
        }

        public string DisplayName
        {
            get { return this.displayName; }
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
            var descriptors = this.switches.ToDictionary(item => item.OriginalName);

            foreach (var item in methodInfo.GetParameters())
            {
                var descriptor = descriptors[item.Name];

                var value = descriptor.GetVaue(instance);
                values.Add(value);
            }

            this.methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
