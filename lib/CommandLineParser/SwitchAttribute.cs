using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ntreev.Library
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchAttribute : Attribute
    {
        static char switchDelimiter = '/';

        static internal SwitchAttribute DefaultValue = new SwitchAttribute(null);

        string shortName = string.Empty;
        char? argSeperator = null;
       
        public static char SwitchDelimiter
        {
            get
            {
                return SwitchAttribute.switchDelimiter;
            }
            set
            {
                if (char.IsPunctuation(value) == false)
                    throw new Exception("SwitchDelimiter는 문장부호여야만 합니다.");
                SwitchAttribute.switchDelimiter = value;
            }
        }

        internal char? GetArgSeperator()
        {
            return this.argSeperator;
        }

        public SwitchAttribute()
        {
            this.Required = false;
            this.MutuallyExclusive = string.Empty;

            foreach (char item in this.shortName)
            {
                if (char.IsLetterOrDigit(item) == false)
                    throw new Exception("스위치의 이름은 문자 또는 숫자여야만 합니다.");
            }
        }

        public SwitchAttribute(string shortName)
            : this()
        {
            this.shortName = shortName == null ? string.Empty : shortName;
        }

        public string ShortName
        {
            get { return this.shortName; }
        }

        public bool Required { get; set; }

        public char ArgSeperator
        {
            get
            {
                if (this.argSeperator == null)
                    return char.MinValue;
                return (char)this.argSeperator; 
            }
            set 
            {
                if (value != char.MinValue)
                {
                    if (char.IsPunctuation(value) == false)
                        throw new Exception("ArgSeperator는 문장부호여야만 합니다.");
                    if (value == SwitchAttribute.SwitchDelimiter)
                        throw new Exception("ArgSeperator는 SwitchDelimiter과 다른 문자여야 합니다.");
                }
                this.argSeperator = (char)value; 
            }
        }

        public string MutuallyExclusive { get; set; }

        public Type UsageProvider { get; set; }
    }

    ///// <summary>
    ///// 스위치의 타입을 나타냅니다.
    ///// </summary>
    //enum SwitchType
    //{
    //    /// <summary>
    //    /// 해당 스위치는 인자를 가질수 없으며, 존재 여부만 확인합니다.
    //    /// </summary>
    //    Toggle,

    //    /// <summary>
    //    /// 해당 스위치는 한개의 인자를 가지고 있습니다.
    //    /// </summary>
    //    Arg,

    //    /// <summary>
    //    /// 해당 스위치의 인자가 스위치에 포함이 되어 있습니다.
    //    /// </summary>
    //    /// <example>
    //    /// /v5 /type"path" /Lv:5
    //    /// </example>
    //    ArgIncluded,
    //}
}
