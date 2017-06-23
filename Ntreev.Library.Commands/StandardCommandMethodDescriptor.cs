using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class StandardCommandMethodDescriptor : CommandMethodDescriptor
    {
        private readonly MethodInfo methodInfo;
        private readonly CommandMethodAttribute attribute;
        private readonly CommandMemberDescriptor[] members;
        private readonly string name;
        private readonly string displayName;
        private readonly string summary;
        private readonly string description;

        public StandardCommandMethodDescriptor(MethodInfo methodInfo)
            : base(methodInfo)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(methodInfo.DeclaringType);
            this.methodInfo = methodInfo;
            this.attribute = methodInfo.GetCommandMethodAttribute();
            this.name = attribute.Name != string.Empty ? attribute.Name : CommandSettings.NameGenerator(methodInfo.Name);
            this.displayName = methodInfo.GetDisplayName();

            var memberList = new List<CommandMemberDescriptor>();

            foreach (var item in methodInfo.GetParameters())
            {
                if (item.GetCustomAttribute<ParamArrayAttribute>() != null)
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

        public override string DescriptorName
        {
            get { return this.methodInfo.Name; }
        }

        public override string Name
        {
            get { return this.name; }
        }

        public override string DisplayName
        {
            get { return this.displayName; }
        }

        public override CommandMemberDescriptor[] Members
        {
            get { return this.members.ToArray(); }
        }

        public override string Summary
        {
            get { return this.summary; }
        }

        public override string Description
        {
            get { return this.description; }
        }

        public override IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach (Attribute item in this.methodInfo.GetCustomAttributes(true))
                {
                    yield return item;
                }
            }
        }

        protected override void OnInvoke(object instance, object[] parameters)
        {
            if (this.methodInfo.DeclaringType.IsAbstract && this.methodInfo.DeclaringType.IsSealed == true)
                this.methodInfo.Invoke(null, parameters);
            else
                this.methodInfo.Invoke(instance, parameters);
        }
    }
}
