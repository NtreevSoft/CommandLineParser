using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class CommandMethodAttribute : Attribute
    {
        static internal CommandMethodAttribute DefaultValue = new CommandMethodAttribute();
        private readonly string name;

        public CommandMethodAttribute()
        {

        }

        public CommandMethodAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// 해당 메소드의 사용법 출력 제공자의 타입을 설정하거나 가져옵니다.
        /// </summary>
        public Type UsageProvider { get; set; }

        public string Name
        {
            get { return this.name; }
        }
    }
}
