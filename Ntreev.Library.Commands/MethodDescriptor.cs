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
        private readonly CommandMethodAttribute attribute;
        private readonly string name;
        private readonly string displayName;
        private readonly SwitchDescriptor[] switches;
        private readonly string summary;
        private readonly string description;

        internal MethodDescriptor(MethodInfo methodInfo)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(methodInfo.DeclaringType);
            this.methodInfo = methodInfo;
            this.attribute = methodInfo.GetCommandMethodAttribute();
            this.name = attribute.Name != string.Empty ? attribute.Name : CommandSettings.NameGenerator(methodInfo.Name);
            this.displayName = methodInfo.GetDisplayName();

            var switchList = new List<SwitchDescriptor>();

            foreach (var item in methodInfo.GetParameters())
            {
                var switchDescriptor = new SwitchParameterInfoDescriptor(item);
                switchList.Add(switchDescriptor);
            }

            var switchAttr = this.methodInfo.GetCustomAttribute<CommandMethodSwitchAttribute>();
            if (switchAttr != null)
            {
                foreach (var item in switchAttr.PropertyNames)
                {
                    var switchDescriptor = CommandDescriptor.GetPropertyInfoDescriptors(methodInfo.DeclaringType)[item];
                    if (switchDescriptor == null)
                        throw new ArgumentException(string.Format("'{0}' attribute does not existed .", item));
                    switchList.Add(switchDescriptor);
                }
            }

            var staticAttrs = this.methodInfo.GetCustomAttributes<CommandStaticSwitchAttribute>();
            foreach (var item in staticAttrs)
            {
                var switches = CommandDescriptor.GetPropertyInfoDescriptors(item.StaticType);
                switchList.AddRange(switches);
            }

            this.switches = switchList.Distinct().ToArray();
            this.summary = provider.GetSummary(methodInfo);
            this.description = provider.GetDescription(methodInfo);
        }

        public string DescriptorName
        {
            get { return this.methodInfo.Name; }
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

        public string Summary
        {
            get { return this.summary; }
        }

        public string Description
        {
            get { return this.description; }
        }

        public virtual IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach (Attribute item in this.methodInfo.GetCustomAttributes(true))
                {
                    yield return item;
                }
            }
        }

        internal MethodInfo MethodInfo
        {
            get { return this.methodInfo; }
        }

        internal static void Invoke(object instance, string arguments, MethodInfo methodInfo, IEnumerable<SwitchDescriptor> switches)
        {
            var helper = new SwitchHelper(switches);
            helper.Parse(instance, arguments);

            var values = new ArrayList();
            var descriptors = switches.ToDictionary(item => item.DescriptorName);

            foreach (var item in methodInfo.GetParameters())
            {
                var descriptor = descriptors[item.Name];

                var value = descriptor.GetValue(instance);
                values.Add(value);
            }

            methodInfo.Invoke(instance, values.ToArray());
        }
    }
}
