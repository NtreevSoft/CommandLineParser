//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ntreev.Library.Commands
{
    public sealed class CommandPropertyDescriptor : CommandMemberDescriptor
    {
        private readonly PropertyInfo propertyInfo;
        private readonly string summary;
        private readonly string description;
        private readonly List<CommandPropertyTriggerAttribute> triggerList = new List<CommandPropertyTriggerAttribute>();

        public CommandPropertyDescriptor(PropertyInfo propertyInfo)
            : base(propertyInfo.GetCommandPropertyAttribute(), propertyInfo.Name)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyInfo.DeclaringType);
            this.propertyInfo = propertyInfo;
            this.summary = provider.GetSummary(propertyInfo);
            this.description = provider.GetDescription(propertyInfo);
            var attrs = propertyInfo.GetCustomAttributes(typeof(CommandPropertyTriggerAttribute), true);
            foreach (var item in attrs)
            {
                if (item is CommandPropertyTriggerAttribute attr)
                {
                    this.triggerList.Add(attr);
                }
            }
        }

        public override string DisplayName
        {
            get
            {
                var displayName = this.propertyInfo.GetDisplayName();
                if (displayName != string.Empty)
                    return displayName;
                return base.DisplayName;
            }
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
                if (this.MemberType == typeof(bool))
                    return true;
                return base.IsExplicit;
            }
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

        protected override void OnValidateTrigger(IDictionary<CommandMemberDescriptor, ParseDescriptorItem> descriptors)
        {
            if (this.triggerList.Any() == false || descriptors[this].IsParsed == false)
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

                    var parseInfo = descriptors[triggerDescriptor];
                    if (parseInfo.IsParsed == false)
                        continue;
                    var value1 = parseInfo.Desiredvalue;
                    var value2 = ClassExtension.GetDefaultValue(triggerDescriptor.MemberType, item.Value);

                    if (item.IsInequality == false)
                    {
                        if (object.Equals(value1, value2) == false)
                            throw new Exception(string.Format("'{0}' can not use. '{1}' property value must be '{2}'", this.DisplayPattern, triggerDescriptor.DisplayPattern, value2));
                    }
                    else
                    {
                        if (object.Equals(value1, value2) == true)
                            throw new Exception(string.Format("'{0}' can not use. '{1}' property value must be not '{2}'", this.DisplayPattern, triggerDescriptor.DisplayPattern, value2));
                    }
                }
            }
        }
    }
}
