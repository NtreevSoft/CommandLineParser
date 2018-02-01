using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    public sealed class CommandPropertyDescriptor : CommandMemberDescriptor
    {
        private readonly PropertyInfo propertyInfo;
        private readonly string summary;
        private readonly string description;
        private readonly List<CommandPropertyTriggerAttribute> triggerList = new List<CommandPropertyTriggerAttribute>();

        //private readonly Dictionary<CommandPropertyDescriptor, object> dependencies = new Dictionary<CommandPropertyDescriptor, object>();

        public CommandPropertyDescriptor(PropertyInfo propertyInfo)
            : base(propertyInfo.GetCommandPropertyAttribute(), propertyInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyInfo.DeclaringType);
            this.propertyInfo = propertyInfo;
            this.summary = provider.GetSummary(propertyInfo);
            this.description = provider.GetDescription(propertyInfo);
            this.triggerList.AddRange(propertyInfo.GetCustomAttributes<CommandPropertyTriggerAttribute>());
        }

        public override string DisplayName
        {
            get { return this.propertyInfo.GetDisplayName(); }
        }

        public override Type MemberType
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
            get
            {
                if (this.IsRequired == false && this.MemberType == typeof(bool))
                    return true;
                return this.propertyInfo.GetDefaultValue();
            }
        }

        public override bool IsExplicit
        {
            get
            {
                if (this.IsRequired == false && this.MemberType == typeof(bool))
                    return true;
                return base.IsExplicit;
            }
        }

        //public override bool IsToggle
        //{
        //    get
        //    {
        //        if (this.IsRequired == false && this.MemberType == typeof(bool))
        //            return true;
        //        return base.IsToggle;
        //    }
        //}

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

        public override TypeConverter Converter
        {
            get { return this.propertyInfo.GetConverter(); }
        }

        protected override void SetValue(object instance, object value)
        {
            this.propertyInfo.SetValue(instance, value, null);
        }

        protected override object GetValue(object instance)
        {
            return this.propertyInfo.GetValue(instance, null);
        }

        protected override void OnValidateTrigger(IReadOnlyDictionary<CommandMemberDescriptor, object> descriptors)
        {
            if (this.triggerList.Any() == false)
                return;
            if (descriptors[this] == DBNull.Value)
                return;

            var query = from item in this.triggerList
                        group item by item.Group into groups
                        select groups;

            var nameToDescriptor = descriptors.Keys.ToDictionary(item => item.DescriptorName);

            foreach (var items in query)
            {
                foreach (var item in items)
                {
                    if (nameToDescriptor.ContainsKey(item.PropertyName) == false)
                        throw new Exception(string.Format("'{0}' property does not exists.", item.PropertyName));
                    var triggerDescriptor = nameToDescriptor[item.PropertyName];
                    if (triggerDescriptor is CommandPropertyDescriptor == false)
                        throw new Exception(string.Format("'{0}' is not property", item.PropertyName));

                    var value1 = descriptors[triggerDescriptor];
                    if (value1 == DBNull.Value)
                        continue;

                    var value2 = ClassExtension.GetDefaultValue(triggerDescriptor.MemberType, item.Value);

                    if (object.Equals(value1, value2) == false)
                        throw new Exception(string.Format("'{0}' can not use. '{1}' property value must be '{2}'", this.DisplayPattern, triggerDescriptor.DisplayPattern, value2));
                }
            }
        }
    }
}
