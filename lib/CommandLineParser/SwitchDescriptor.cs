using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Ntreev.Library
{
    public class SwitchDescriptor
    {
        readonly PropertyDescriptor propertyDescriptor;
        readonly SwitchAttribute switchAttribute;
        bool parsed = false;
        UsageProvider usageProvider;

        internal void Parse(string arg, object instance)
        {
            Type type = this.propertyDescriptor.PropertyType;
            object value = null;

            if (type == typeof(bool) && this.switchAttribute.GetArgSeperator() == null) /// 한가지 예외. 이것은 도저히 방법이 없다.
            {
                value = true;
            }
            else
            {
                TypeConverter typeConverter = this.propertyDescriptor.Converter;
                if (typeConverter.CanConvertFrom(typeof(string)) == false)
                    throw new NotSupportedException("타입컨버터에서 문자열에 의한 변환이 지원되질 않습니다.");

                try
                {
                    value = typeConverter.ConvertFrom(arg);
                }
                catch (Exception e)
                {
                    throw new SwitchException("잘못된 인수 형식입니다.", this.Name, e);
                }
            }

            if (value != null)
                this.propertyDescriptor.SetValue(instance, value);
            this.parsed = true;
        }

        internal string GetPattern(string switchGroupName, string argGroupName)
        {
            string quotes = string.Format(@"(""(?<{0}>.*)"")", argGroupName);
            string normal = string.Format(@"(?<{0}>(\S)+)", argGroupName);

            string pattern;
            if(this.ShortName == string.Empty)
                pattern = string.Format(@"^{0}(?<{1}>{2})", SwitchAttribute.SwitchDelimiter, switchGroupName, this.Name);
            else
                pattern = string.Format(@"^{0}(?<{1}>({2}|{3}))", SwitchAttribute.SwitchDelimiter, switchGroupName, this.Name, this.ShortName);

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

        internal SwitchDescriptor(PropertyDescriptor propertyDescriptor, SwitchAttribute optionAttribute)
        {
            this.propertyDescriptor = propertyDescriptor;
            this.switchAttribute = optionAttribute;

            if (optionAttribute.UsageProvider == null)
            {
                this.usageProvider = new InternalUsageProvider(this);
            }
            else
            {
                this.usageProvider = TypeDescriptor.CreateInstance(
                    null, 
                    optionAttribute.UsageProvider,
                    new Type[] { typeof(SwitchDescriptor), },
                    new object[] { this, }) as UsageProvider;
            }

        }

        public char? ArgSeperator
        {
            get { return this.switchAttribute.GetArgSeperator(); }
        }

        public UsageProvider UsageProvider
        {
            get { return this.usageProvider; }
        }

        public string ShortName
        {
            get
            {
                return this.switchAttribute.ShortName;
            }
        }

        public string Name
        {
            get { return this.propertyDescriptor.Name; }
        }

        public string DisplayName
        {
            get { return this.propertyDescriptor.DisplayName; }
        }

        public string Description
        {
            get
            {
                return this.propertyDescriptor.Description;
            }
        }

        public bool Required
        {
            get { return this.switchAttribute.Required; }
        }

        public string MutuallyExclusive
        {
            get { return this.switchAttribute.MutuallyExclusive; }
        }

        public bool Parsed
        {
            get { return this.parsed; }
        }

        public Type ArgType
        {
            get { return this.propertyDescriptor.PropertyType; }
        }
    }
}
    

