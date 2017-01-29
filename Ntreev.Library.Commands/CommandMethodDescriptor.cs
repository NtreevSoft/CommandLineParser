using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;

namespace Ntreev.Library.Commands
{
    public class CommandMethodDescriptor
    {
        private readonly MethodInfo methodInfo;
        private readonly CommandMethodAttribute attribute;
        private readonly CommandMemberDescriptor[] members;
        private readonly string name;
        private readonly string displayName;
        private readonly string summary;
        private readonly string description;

        internal CommandMethodDescriptor(MethodInfo methodInfo)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(methodInfo.DeclaringType);
            this.methodInfo = methodInfo;
            this.attribute = methodInfo.GetCommandMethodAttribute();
            this.name = attribute.Name != string.Empty ? attribute.Name : CommandSettings.NameGenerator(methodInfo.Name);
            this.displayName = methodInfo.GetDisplayName();

            var memberList = new List<CommandMemberDescriptor>();

            foreach (var item in methodInfo.GetParameters())
            {
                if(item.GetCustomAttribute<ParamArrayAttribute>() != null)
                {
                    memberList.Add(new CommandParameterArrayDescriptor(item));
                }
                    else
                {
                    memberList.Add(new CommandParameterDescriptor(item));
                }
            }

            var methodAttr = this.methodInfo.GetCustomAttribute<CommandMethodPropertyAttribute>();
            if (methodAttr != null)
            {
                foreach (var item in methodAttr.PropertyNames)
                {
                    var memberDescriptor = CommandDescriptor.GetMemberDescriptors(methodInfo.DeclaringType)[item];
                    if (memberDescriptor == null)
                        throw new ArgumentException(string.Format("'{0}' attribute does not existed .", item));
                    memberList.Add(memberDescriptor);
                }
            }

            var staticAttrs = this.methodInfo.GetCustomAttributes<CommandStaticPropertyAttribute>();
            foreach (var item in staticAttrs)
            {
                var memberDescriptors = CommandDescriptor.GetMemberDescriptors(item.StaticType);
                memberList.AddRange(memberDescriptors);
            }

            this.members = memberList.OrderBy(item => item is CommandMemberArrayDescriptor).ToArray();
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

        public CommandMemberDescriptor[] Members
        {
            get { return this.members.ToArray(); }
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

        internal static void Invoke(object instance, string arguments, MethodInfo methodInfo, IEnumerable<CommandMemberDescriptor> memberDescriptors)
        {
            var helper = new ParseDescriptor(memberDescriptors);
            helper.Parse(instance, arguments);

            var values = new ArrayList();
            var descriptors = memberDescriptors.ToDictionary(item => item.DescriptorName);

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
