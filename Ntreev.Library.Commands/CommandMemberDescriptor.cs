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
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public abstract class CommandMemberDescriptor
    {
        private const string keyName = "key";
        private const string valueName = "value";

        private readonly string descriptorName;
        private readonly string name;
        private readonly string shortName;
        private readonly bool isRequired;
        private readonly bool isExplicit;

        protected CommandMemberDescriptor(CommandPropertyAttribute attribute, string descriptorName)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            attribute.InvokeValidate(this);
            this.descriptorName = descriptorName ?? throw new ArgumentNullException(nameof(descriptorName));
            this.name = attribute.GetName(descriptorName);
            this.shortName = attribute.InternalShortName;
            this.isRequired = attribute.IsRequired;
            this.isExplicit = attribute.IsExplicit;
        }

        public string Name
        {
            get { return this.name; }
        }

        public string ShortName
        {
            get { return this.shortName; }
        }

        public virtual string DisplayName
        {
            get
            {
                if (this.IsRequired == true && this.isExplicit == false)
                    return this.descriptorName;
                var nameItems = this.isExplicit == true ? new string[] { this.ShortNamePattern, this.NamePattern } : new string[] { this.shortName, this.name };
                var displayName = string.Join(" | ", nameItems.Where(item => item != string.Empty).ToArray());
                if (displayName == string.Empty)
                    return CommandSettings.NameGenerator(this.descriptorName);
                return displayName;
            }
        }

        public virtual string Summary
        {
            get { return string.Empty; }
        }

        public virtual string Description
        {
            get { return string.Empty; }
        }

        public virtual object DefaultValue
        {
            get { return DBNull.Value; }
        }

        public virtual bool IsRequired
        {
            get { return this.isRequired; }
        }

        public virtual bool IsExplicit
        {
            get { return this.isExplicit; }
        }

        public abstract Type MemberType
        {
            get;
        }

        public virtual TypeConverter Converter
        {
            get { return TypeDescriptor.GetConverter(this.MemberType); }
        }

        public virtual IEnumerable<Attribute> Attributes
        {
            get { yield break; }
        }

        public string DescriptorName
        {
            get { return this.descriptorName; }
        }

        protected abstract void SetValue(object instance, object value);

        protected abstract object GetValue(object instance);

        protected virtual void OnValidateTrigger(IDictionary<CommandMemberDescriptor, ParseDescriptorItem> descriptors)
        {

        }

        internal object Parse(object instance, string arg)
        {
            return Parser.Parse(this, arg);
        }

        internal void Parse(object instance, List<string> arguments)
        {
            if (this.MemberType == typeof(bool))
            {
                this.SetValue(instance, true);
            }
            else
            {
                var arg = arguments.First();
                var value = Parser.Parse(this, arg);
                this.SetValue(instance, value);
                arguments.RemoveAt(0);
            }
        }

        internal string[] GetCompletion(object target)
        {
            if (this.MemberType.IsEnum == true)
            {
                return Enum.GetNames(this.MemberType);
            }
            else if (this.Attributes.FirstOrDefault(item => item is CommandCompletionAttribute) is CommandCompletionAttribute attr)
            {
                if (attr.Type == null)
                {
                    var methodInfo = target.GetType().GetMethod(attr.MethodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new Type[] { }, null);
                    return methodInfo.Invoke(target, null) as string[];
                }
                else
                {
                    var methodInfo = attr.Type.GetMethod(attr.MethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null);
                    return methodInfo.Invoke(null, null) as string[];
                }
            }
            return null;
        }

        public string NamePattern
        {
            get
            {
                if (this.name == string.Empty)
                    return string.Empty;
                return CommandSettings.Delimiter + this.name;
            }
        }

        public string ShortNamePattern
        {
            get
            {
                if (this.shortName == string.Empty)
                    return string.Empty;
                return CommandSettings.ShortDelimiter + this.shortName;
            }
        }

        public string DisplayPattern
        {
            get
            {
                var patternItems = new string[] { this.ShortNamePattern, this.NamePattern };
                var patternText = string.Join(" | ", patternItems.Where(item => item != string.Empty).ToArray());
                return patternText;
            }
        }

        internal void SetValueInternal(object instance, object value)
        {
            this.SetValue(instance, value);
        }

        internal object GetValueInternal(object instance)
        {
            return this.GetValue(instance);
        }

        internal void ValidateTrigger(Dictionary<CommandMemberDescriptor, ParseDescriptorItem> descriptors)
        {
            this.OnValidateTrigger(descriptors);
        }
    }
}
