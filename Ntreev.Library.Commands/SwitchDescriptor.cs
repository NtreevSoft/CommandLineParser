#region License
//Ntreev CommandLineParser for .Net 1.0.4548.25168
//https://github.com/NtreevSoft/CommandLineParser

//Released under the MIT License.

//Copyright (c) 2010 Ntreev Soft co., Ltd.

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 스위치의 정보를 담고 있는 클래스입니다.
    /// </summary>
    public sealed class SwitchDescriptor
    {
        public const string SwitchGroupName = "switch";
        public const string ArgGroupName = "arg";

        private readonly CommandSwitchAttribute switchAttribute;
        private readonly SwitchTypes switchType;
        private readonly string originalName;
        private readonly string name;
        private readonly string shortName;
        private readonly string displayName;
        private readonly Type type;
        private readonly TypeConverter converter;
        private readonly string summary;
        private readonly string description;
        private readonly object defaultValue = DBNull.Value;
        private readonly ValueSetter valueSetter;

        private string pattern;

        internal SwitchDescriptor(PropertyInfo propertyInfo)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyInfo.DeclaringType);
            this.switchAttribute = propertyInfo.GetCommandSwitchAttribute();

            this.switchType = SwitchTypes.Property;
            this.originalName = propertyInfo.Name;
            this.name = this.switchAttribute.Name != string.Empty ? this.switchAttribute.Name : propertyInfo.Name;
            this.shortName = this.switchAttribute.ShortNameInternal;
            this.displayName = propertyInfo.GetDisplayName();
            this.VerifyName(ref this.name, ref this.shortName, ref this.displayName, this.switchAttribute.NameType);
            this.type = propertyInfo.PropertyType;
            this.converter = propertyInfo.GetConverter();
            this.summary = provider.GetSummary(propertyInfo);
            this.description = provider.GetDescription(propertyInfo);
            this.defaultValue = propertyInfo.GetDefaultValue();
            this.valueSetter = new PropertyInfoValueSetter(this, propertyInfo);
        }

        internal SwitchDescriptor(PropertyDescriptor propertyDescriptor)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(propertyDescriptor.ComponentType);
            this.switchAttribute = propertyDescriptor.GetCommandSwitchAttribute();

            this.switchType = SwitchTypes.Property;
            this.originalName = propertyDescriptor.Name;
            this.name = this.switchAttribute.Name != string.Empty ? this.switchAttribute.Name : propertyDescriptor.Name;
            this.shortName = this.switchAttribute.ShortNameInternal;
            this.displayName = propertyDescriptor.GetDisplayName() != string.Empty ? propertyDescriptor.GetDisplayName() : this.name;
            this.VerifyName(ref this.name, ref this.shortName, ref this.displayName, this.switchAttribute.NameType);
            this.type = propertyDescriptor.PropertyType;
            this.converter = propertyDescriptor.Converter;
            this.summary = provider.GetSummary(propertyDescriptor);
            this.description = provider.GetDescription(propertyDescriptor);
            this.defaultValue = propertyDescriptor.GetDefaultValue();
            this.valueSetter = new PropertyValueSetter(this, propertyDescriptor);
        }

        internal SwitchDescriptor(ParameterInfo parameterInfo)
        {
            var provider = CommandDescriptor.GetUsageDescriptionProvider(parameterInfo.Member.DeclaringType);
            this.switchAttribute = new CommandSwitchAttribute() { Required = true, NameType = SwitchNameTypes.Name, };

            this.switchType = SwitchTypes.Parameter;
            this.originalName = parameterInfo.Name;
            this.name = parameterInfo.Name.ToSpinalCase();
            this.shortName = string.Empty;
            this.displayName = parameterInfo.GetDisplayName();
            this.VerifyName(ref this.name, ref this.shortName, ref this.displayName, this.switchAttribute.NameType);
            this.type = parameterInfo.ParameterType;
            this.converter = parameterInfo.GetConverter();
            this.summary = provider.GetSummary(parameterInfo);
            this.description = provider.GetDescription(parameterInfo);
            this.defaultValue = parameterInfo.DefaultValue;
            this.valueSetter = new ParameterValueSetter(this, parameterInfo);
        }

        /// <summary>
        /// 인자와 스위치가 한 문자열내에 포함되어 있을때 이를 구분하는 문자를 가져옵니다.
        /// </summary>
        /// <remarks>
        /// 이 속성의 값이 null이면 인자와 스위치는 공백으로 구분되어져 있습니다.
        /// <seealso cref="Char.MinValue"/>일 경우에는 스위치와 인자가 하나의 단어로 이루어져 있는 상태입니다.
        /// 그외에는 <see cref="Char.IsPunctuation"/>의 값이 true인 문자로 구분되어집니다.
        /// </remarks>
        public char? ArgSeperator
        {
            get { return this.switchAttribute.GetArgSeperator(); }
        }

        public string OriginalName
        {
            get { return this.originalName; }
        }

        /// <summary>
        /// 스위치의 이름을 가져옵니다.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 스위치의 짧은 이름을 가져옵니다.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
        }

        public string NamePattern
        {
            get
            {
                if (this.name == string.Empty)
                    return string.Empty;
                return CommandSwitchAttribute.SwitchDelimiter + this.name;
            }
        }

        public string ShortNamePattern
        {
            get
            {
                if (this.shortName == string.Empty)
                    return string.Empty;
                return CommandSwitchAttribute.ShortSwitchDelimiter + this.shortName;
            }
        }

        public string DisplayName
        {
            get { return this.displayName; }
        }

        /// <summary>
        /// 스위치의 부가적인 설명을 가져옵니다.
        /// </summary>
        public string Summary
        {
            get { return this.summary; }
        }

        /// <summary>
        /// 스위치의 부가적인 설명을 가져옵니다.
        /// </summary>
        public string Description
        {
            get { return this.description; }
        }

        public object DefaultValue
        {
            get { return this.defaultValue; }
        }

        public string ArgTypeSummary
        {
            get
            {
                Type elementType = ListParser.GetItemType(this.type);
                if (elementType != null)
                {
                    return elementType.GetSimpleName() + ", ...";
                }
                return this.ArgType.GetSimpleName();
            }
        }

        /// <summary>
        /// 분석할때 해당 스위치가 꼭 필요한지에 대한 여부를 가져옵니다.
        /// </summary>
        public bool Required
        {
            get { return this.switchAttribute.Required; }
        }

        /// <summary>
        /// 해당 스위치가 가지고 있는 인자의 타입을 가져옵니다.
        /// </summary>
        public Type ArgType
        {
            get { return this.type; }
        }

        public TypeConverter Converter
        {
            get { return this.converter; }
        }

        public SwitchTypes SwitchType
        {
            get { return this.switchType; }
        }

        private string BuildPattern()
        {
            var quotes = string.Format(@"(""(?<{0}>.*)"")", SwitchDescriptor.ArgGroupName);
            var normal = string.Format(@"(?<{0}>(\S)+)", SwitchDescriptor.ArgGroupName);

            var pattern = string.Empty;
            if (this.Name != string.Empty && this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>({1}{2}|{3}{4}))", SwitchDescriptor.SwitchGroupName, CommandSwitchAttribute.SwitchDelimiter, this.Name, CommandSwitchAttribute.ShortSwitchDelimiter, this.ShortName);
            }
            else if (this.Name != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", SwitchDescriptor.SwitchGroupName, CommandSwitchAttribute.SwitchDelimiter, this.Name);
            }
            else if (this.ShortName != string.Empty)
            {
                pattern = string.Format(@"^(?<{0}>{1}{2})", SwitchDescriptor.SwitchGroupName, CommandSwitchAttribute.ShortSwitchDelimiter, this.ShortName);
            }

            var argSeperator = this.switchAttribute.GetArgSeperator();
            if (this.ArgType != typeof(bool) || argSeperator != null)
            {
                if (argSeperator == null)
                {
                    pattern += string.Format(@"(((\s+)({0}|{1}))|($))", quotes, normal);
                }
                else
                {
                    if (argSeperator != char.MinValue)
                        pattern += argSeperator;
                    pattern += string.Format(@"(({0}|{1})|$)", quotes, normal);
                }
            }
            else
            {
                pattern += @"((\s+)|$)";
            }

            return pattern;
        }

        private void VerifyName(ref string name, ref string shortName, ref string displayName, SwitchNameTypes nameType)
        {
            name = name.ToSpinalCase();
            if (this.switchAttribute.NameType == SwitchNameTypes.Name)
            {
                shortName = string.Empty;
            }
            else if (this.switchAttribute.NameType == SwitchNameTypes.ShortName)
            {
                if (shortName == string.Empty)
                    throw new ArgumentException("짧은 이름이 존재하지 않습니다.");
                name = string.Empty;
            }
            else
            {

            }

            if (displayName == string.Empty)
                displayName = name;
        }

        private string Pattern
        {
            get
            {
                if (this.pattern == null)
                    this.pattern = this.BuildPattern();
                return this.pattern;
            }
        }

        internal void SetValue(object instance, string arg)
        {
            this.valueSetter.SetValue(instance, arg);
        }

        internal object GetVaue(object instance)
        {
            return this.valueSetter.GetValue(instance);
        }

        internal string TryMatch(string switchLine, ref string parsed)
        {
            var match = Regex.Match(switchLine, this.Pattern, RegexOptions.ExplicitCapture);
            if (match.Success == false)
                return null;
            parsed = match.Value;
            return match.Groups[SwitchDescriptor.ArgGroupName].Value;
        }

        #region setters

        abstract class ValueSetter
        {
            public abstract void SetValue(object instance, string arg);

            public abstract object GetValue(object instance);
        }

        class PropertyValueSetter : ValueSetter
        {
            private readonly PropertyDescriptor propertyDescriptor;
            private readonly SwitchDescriptor switchDescriptor;

            public PropertyValueSetter(SwitchDescriptor switchDescriptor, PropertyDescriptor propertyDescriptor)
            {
                this.switchDescriptor = switchDescriptor;
                this.propertyDescriptor = propertyDescriptor;
            }

            public override void SetValue(object instance, string arg)
            {
                var value = this.propertyDescriptor.GetValue(instance);

                var parser = Parser.GetParser(this.propertyDescriptor);
                var newValue = parser.Parse(this.switchDescriptor, arg, value);

                if (value != newValue && this.propertyDescriptor.IsReadOnly == false)
                    this.propertyDescriptor.SetValue(instance, newValue);
            }

            public override object GetValue(object instance)
            {
                return this.propertyDescriptor.GetValue(instance);
            }
        }

        class PropertyInfoValueSetter : ValueSetter
        {
            private readonly PropertyInfo propertyInfo;
            private readonly SwitchDescriptor switchDescriptor;

            public PropertyInfoValueSetter(SwitchDescriptor switchDescriptor, PropertyInfo propertyInfo)
            {
                this.switchDescriptor = switchDescriptor;
                this.propertyInfo = propertyInfo;
            }

            public override void SetValue(object instance, string arg)
            {
                var value = this.propertyInfo.GetValue(instance, null);

                var parser = Parser.GetParser(this.propertyInfo);
                var newValue = parser.Parse(this.switchDescriptor, arg, value);

                if (value != newValue && this.propertyInfo.CanWrite == true)
                    this.propertyInfo.SetValue(instance, newValue, null);
            }

            public override object GetValue(object instance)
            {
                return this.propertyInfo.GetValue(instance, null);
            }
        }

        class ParameterValueSetter : ValueSetter
        {
            private readonly ParameterInfo parameterInfo;
            private readonly SwitchDescriptor switchDescriptor;
            private object value = DBNull.Value;

            public ParameterValueSetter(SwitchDescriptor switchDescriptor, ParameterInfo parameterInfo)
            {
                this.switchDescriptor = switchDescriptor;
                this.parameterInfo = parameterInfo;
            }

            public override void SetValue(object instance, string arg)
            {
                var parser = Parser.GetParser(this.parameterInfo);
                this.value = parser.Parse(this.switchDescriptor, arg, value);
            }

            public override object GetValue(object instance)
            {
                if (this.value == DBNull.Value)
                {
                    return this.parameterInfo.DefaultValue;
                }
                return this.value;
            }
        }

        #endregion
    }
}
