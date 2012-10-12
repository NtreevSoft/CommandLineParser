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

namespace Ntreev.Library
{
    /// <summary>
    /// 스위치의 정보를 담고 있는 클래스입니다.
    /// </summary>
    public sealed class SwitchDescriptor
    {
        public const string SwitchGroupName = "switch";
        public const string ArgGroupName = "arg";

        private readonly CommandSwitchAttribute switchAttribute;
        //private readonly PropertyDescriptor propertyDescriptor;
        private string pattern;
        private SwitchUsageProvider usageProvider;

        private readonly string name;
        private readonly string displayName;
        private readonly Type type;
        private readonly TypeConverter converter;
        private readonly string description;
        private readonly IValueSetter valueSetter;

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

        /// <summary>
        /// 스위치의 사용 방법을 제공하는 제공자의 인스턴스를 가져옵니다.
        /// </summary>
        public SwitchUsageProvider UsageProvider
        {
            get { return this.usageProvider; }
        }

        /// <summary>
        /// 스위치의 짧은 이름을 가져옵니다.
        /// </summary>
        public string ShortName
        {
            get
            {
                return this.switchAttribute.ShortName;
            }
        }

        /// <summary>
        /// 스위치의 이름을 가져옵니다.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 스위치의 표시 이름을 가져옵니다.
        /// </summary>
        public string DisplayName
        {
            get { return this.displayName; }
        }

        /// <summary>
        /// 스위치의 부가적인 설명을 가져옵니다.
        /// </summary>
        public string Description
        {
            get { return this.description; }
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

        internal SwitchDescriptor(PropertyInfo propertyInfo)
        {
            this.switchAttribute = propertyInfo.GetCommandSwitchAttribute();

            if (this.switchAttribute.UsageProvider == null)
            {
                this.usageProvider = new InternalSwitchUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                    null,
                    this.switchAttribute.UsageProvider,
                    new Type[] { typeof(SwitchDescriptor), },
                    new object[] { this, }) as SwitchUsageProvider;
            }

            this.name = propertyInfo.Name;
            this.displayName = propertyInfo.GetDisplayName();
            this.type = propertyInfo.PropertyType;
            this.converter = propertyInfo.GetConverter();
            this.description = propertyInfo.GetDescription();;
            this.valueSetter = new PropertyInfoValueSetter(this, propertyInfo);
        }

        internal SwitchDescriptor(PropertyDescriptor propertyDescriptor)
        {
            this.switchAttribute = propertyDescriptor.GetCommandSwitchAttribute();

            if (this.switchAttribute.UsageProvider == null)
            {
                this.usageProvider = new InternalSwitchUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                    null,
                    this.switchAttribute.UsageProvider,
                    new Type[] { typeof(SwitchDescriptor), },
                    new object[] { this, }) as SwitchUsageProvider;
            }

            this.name = propertyDescriptor.Name;
            this.displayName = propertyDescriptor.DisplayName;
            this.type = propertyDescriptor.PropertyType;
            this.converter = propertyDescriptor.Converter;
            this.description = propertyDescriptor.Description;
            this.valueSetter = new PropertyValueSetter(this, propertyDescriptor);
        }

        internal SwitchDescriptor(ParameterInfo parameterInfo)
        {
            this.switchAttribute = parameterInfo.GetCommandSwitchAttribute();

            if (this.switchAttribute.UsageProvider == null)
            {
                this.usageProvider = new InternalSwitchUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                    null,
                    this.switchAttribute.UsageProvider,
                    new Type[] { typeof(SwitchDescriptor), },
                    new object[] { this, }) as SwitchUsageProvider;
            }

            this.name = parameterInfo.Name;
            this.displayName = parameterInfo.GetDisplayName();
            this.type = parameterInfo.ParameterType;
            this.converter = parameterInfo.GetConverter();
            this.description = parameterInfo.GetDescription();
            this.valueSetter = new ParameterValueSetter(this, parameterInfo);
        }

        internal void SetValue(object instance, string arg)
        {
            this.valueSetter.SetValue(instance, arg);
        }

        internal object GetVaue(object instance)
        {
            return this.valueSetter.GetValue(instance);
        }

        internal string TryMatch(string switchLine, bool caseSensitive)
        {
            RegexOptions regexOptions = RegexOptions.ExplicitCapture;
            if (caseSensitive == false)
                regexOptions |= RegexOptions.IgnoreCase;

            Match match = Regex.Match(switchLine, this.Pattern, regexOptions);
            if (match.Success == false)
                return null;
            return match.Groups[SwitchDescriptor.ArgGroupName].Value;
        }

        private string BuildPattern()
        {
            string quotes = string.Format(@"(""(?<{0}>.*)"")", SwitchDescriptor.ArgGroupName);
            string normal = string.Format(@"(?<{0}>(\S)+)", SwitchDescriptor.ArgGroupName);

            string pattern;
            if (this.ShortName == string.Empty)
                pattern = string.Format(@"^{0}(?<{1}>{2})", CommandSwitchAttribute.SwitchDelimiter, SwitchDescriptor.SwitchGroupName, this.Name);
            else
                pattern = string.Format(@"^{0}(?<{1}>({2}|{3}))", CommandSwitchAttribute.SwitchDelimiter, SwitchDescriptor.SwitchGroupName, this.Name, this.ShortName);

            char? argSeperator = this.switchAttribute.GetArgSeperator();
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

        private string Pattern
        {
            get
            {
                if (this.pattern == null)
                    this.pattern = this.BuildPattern();
                return this.pattern;
            }
        }

        interface IValueSetter
        {
            void SetValue(object instance, string arg);
            object GetValue(object instance);
        }

        class PropertyValueSetter : IValueSetter
        {
            private readonly PropertyDescriptor propertyDescriptor;
            private readonly SwitchDescriptor switchDescriptor;

            public PropertyValueSetter(SwitchDescriptor switchDescriptor, PropertyDescriptor propertyDescriptor)
            {
                this.switchDescriptor = switchDescriptor;
                this.propertyDescriptor = propertyDescriptor;
            }

            public void SetValue(object instance, string arg)
            {
                object value = this.propertyDescriptor.GetValue(instance);

                Parser parser = Parser.GetParser(this.propertyDescriptor);
                object newValue = parser.Parse(this.switchDescriptor, arg, value);

                if (value != newValue && this.propertyDescriptor.IsReadOnly == false)
                    this.propertyDescriptor.SetValue(instance, newValue);
            }

            public object GetValue(object instance)
            {
                return this.propertyDescriptor.GetValue(instance);
            }
        }

        class PropertyInfoValueSetter : IValueSetter
        {
            private readonly PropertyInfo propertyInfo;
            private readonly SwitchDescriptor switchDescriptor;

            public PropertyInfoValueSetter(SwitchDescriptor switchDescriptor, PropertyInfo propertyInfo)
            {
                this.switchDescriptor = switchDescriptor;
                this.propertyInfo = propertyInfo;
            }

            public void SetValue(object instance, string arg)
            {
                object value = this.propertyInfo.GetValue(instance, null);

                Parser parser = Parser.GetParser(this.propertyInfo);
                object newValue = parser.Parse(this.switchDescriptor, arg, value);

                if (value != newValue && this.propertyInfo.CanWrite == true)
                    this.propertyInfo.SetValue(instance, newValue, null);
            }

            public object GetValue(object instance)
            {
                return this.propertyInfo.GetValue(instance, null);
            }
        }

        class ParameterValueSetter : IValueSetter
        {
            private readonly ParameterInfo parameterInfo;
            private readonly SwitchDescriptor switchDescriptor;
            private object value;
            private bool parsed = false;
            

            public ParameterValueSetter(SwitchDescriptor switchDescriptor, ParameterInfo parameterInfo)
            {
                this.switchDescriptor = switchDescriptor;
                this.parameterInfo = parameterInfo;
            }

            public void SetValue(object instance, string arg)
            {
                Parser parser = Parser.GetParser(this.parameterInfo);
                this.value = parser.Parse(this.switchDescriptor, arg, value);
                this.parsed = true;
            }

            public object GetValue(object instance)
            {
                if (this.parsed == false)
                {
                    object defaultValue;
                    if (this.parameterInfo.TryGetDefaultValue(out defaultValue) == true)
                        return defaultValue;
                }
                return this.value; 
            }
        }
    }
}