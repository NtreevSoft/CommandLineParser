using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ntreev.Library
{
    /// <summary>
    /// 스위치의 속성을 지정합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SwitchAttribute : Attribute
    {
        static char switchDelimiter = '/';
        static internal SwitchAttribute DefaultValue = new SwitchAttribute();

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

        /// <summary>
        /// <seealso cref="SwitchAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
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

        /// <summary>
        /// 짧은 이름을 사용하여 <seealso cref="SwitchAttribute"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public SwitchAttribute(string shortName)
            : this()
        {
            this.shortName = shortName == null ? string.Empty : shortName;
        }

        /// <summary>
        /// 해당 스위치의 짧은 이름을 설정하거나 가져옵니다.
        /// </summary>
        public string ShortName
        {
            get { return this.shortName; }
        }

        /// <summary>
        /// 해당 스위치가 꼭 필요한지의 여부를 설정하거나 가져옵니다.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// 인자가 스위치에 포함되어 있을때 인자와 스위치를 구분하기 위한 문자를 설정하거나 가져옵니다.
        /// </summary>
        /// <remarks>
        /// /Level6 처럼 스위치와 인자의 구분이 필요가 없다면 <seealso cref="char.MinValue"/>를 설정하세요.
        /// </remarks>
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

        /// <summary>
        /// 해당 스위치의 사용법 출력 제공자의 타입을 설정하거나 가져옵니다.
        /// </summary>
        public Type UsageProvider { get; set; }
    }
}
