using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class CommandPropertyDescriptor : CommandMemberDescriptor
    {
        private readonly PropertyInfo propertyInfo;
        private readonly string summary;
        private readonly string description;

        public CommandPropertyDescriptor(PropertyInfo propertyInfo)
            : base(propertyInfo.GetCommandSwitchAttribute(), propertyInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyInfo.DeclaringType);
            this.propertyInfo = propertyInfo;
            this.summary = provider.GetSummary(propertyInfo);
            this.description = provider.GetDescription(propertyInfo);
        }

        public override string DisplayName
        {
            get { return this.propertyInfo.GetDisplayName(); }
        }

        public override Type SwitchType
        {
            get { return this.propertyInfo.PropertyType; }
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
            get { return this.propertyInfo.GetDefaultValue(); }
        }

        public override void SetValue(object instance, object value)
        {
            this.propertyInfo.SetValue(instance, value, null);
        }

        public override object GetValue(object instance)
        {
            return this.propertyInfo.GetValue(instance, null);
        }

        public override IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach (Attribute item in this.propertyInfo.GetCustomAttributes(true))
                {
                    yield return item;
                }
            }
        }
    }
}
