using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    class SwitchPropertyDescriptor : SwitchDescriptor
    {
        private readonly PropertyDescriptor propertyDescriptor;
        private readonly string summary;
        private readonly string description;

        public SwitchPropertyDescriptor(PropertyDescriptor propertyDescriptor)
            : base(propertyDescriptor.GetCommandSwitchAttribute(), propertyDescriptor.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyDescriptor.ComponentType);
            this.propertyDescriptor = propertyDescriptor;
            this.summary = provider.GetSummary(propertyDescriptor);
            this.description = provider.GetDescription(propertyDescriptor);
        }

        public override string DisplayName
        {
            get { return this.propertyDescriptor.DisplayName; }
        }

        public override string Summary
        {
            get { return this.summary; }
        }

        public override string Description
        {
            get { return this.description; }
        }

        public override Type SwitchType
        {
            get { return this.propertyDescriptor.PropertyType; }
        }

        public override TypeConverter Converter
        {
            get { return this.propertyDescriptor.Converter; }
        }

        public override object DefaultValue
        {
            get { return this.propertyDescriptor.GetDefaultValue(); }
        }

        public override void SetValue(object instance, object value)
        {
            this.propertyDescriptor.SetValue(instance, value);
        }

        public override object GetValue(object instance)
        {
            return this.propertyDescriptor.GetValue(instance);
        }

        public override IEnumerable<Attribute> Attributes
        {
            get
            {
                foreach(Attribute item in this.propertyDescriptor.Attributes)
                {
                    yield return item;
                }
            }
        }
    }
}
