using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public sealed class CommandParameterDescriptor : CommandMemberDescriptor
    {
        private readonly ParameterInfo parameterInfo;
        private readonly string summary;
        private readonly string description;
        private object value;

        public CommandParameterDescriptor(ParameterInfo parameterInfo)
            : base(new CommandPropertyAttribute() { IsRequired = true, }, parameterInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(parameterInfo.Member.DeclaringType);
            var paramAttr = parameterInfo.GetCustomAttribute<ParamArrayAttribute>();
            this.parameterInfo = parameterInfo;
            this.summary = provider.GetSummary(parameterInfo);
            this.description = provider.GetDescription(parameterInfo);
            this.value = this.parameterInfo.DefaultValue;
        }

        public override string DisplayName
        {
            get
            {
                var displayName = this.parameterInfo.GetDisplayName();
                if (displayName != string.Empty)
                    return displayName;
                return this.Name;
            }
        }

        public override string Summary
        {
            get { return this.summary; }
        }

        public override string Description
        {
            get { return this.description; }
        }

        public override object DefaultValue
        {
            get { return this.parameterInfo.DefaultValue; }
        }

        public override Type MemberType
        {
            get { return this.parameterInfo.ParameterType; }
        }

        public override IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach (Attribute item in this.parameterInfo.GetCustomAttributes(true))
                {
                    yield return item;
                }
            }
        }

        protected override void SetValue(object instance, object value)
        {
            this.value = value;
        }

        protected override object GetValue(object instance)
        {
            return this.value;
        }
    }
}
