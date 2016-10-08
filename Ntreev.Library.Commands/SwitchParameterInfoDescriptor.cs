using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class SwitchParameterInfoDescriptor : SwitchDescriptor
    {
        private readonly ParameterInfo parameterInfo;
        private readonly string summary;
        private readonly string description;
        private object value;

        public SwitchParameterInfoDescriptor(ParameterInfo parameterInfo)
            : base(new CommandSwitchAttribute() { Required = true, }, parameterInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(parameterInfo.Member.DeclaringType);
            this.parameterInfo = parameterInfo;
            this.summary = provider.GetSummary(parameterInfo);
            this.description = provider.GetDescription(parameterInfo);
            this.value = this.parameterInfo.DefaultValue;
        }

        public override string DisplayName
        {
            get { return this.parameterInfo.GetDisplayName(); }
        }

        public override object DefaultValue
        {
            get { return this.parameterInfo.DefaultValue; }
        }

        public override Type SwitchType
        {
            get { return this.parameterInfo.ParameterType; }
        }

        public override void SetValue(object instance, object value)
        {
            this.value = value;
        }

        public override object GetValue(object instance)
        {
            return this.value;
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
    }
}
