using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// 메소드 호출시 명령문에 속성 스위치를 추가하는 특성을 나타냅니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandMethodSwitchAttribute : Attribute
    {
        private readonly string[] propertyNames;

        public CommandMethodSwitchAttribute(string propertyName)
        {
            this.propertyNames = new string[] { propertyName, };
        }

        public CommandMethodSwitchAttribute(params string[] propertyNames)
        {
            this.propertyNames = propertyNames;
        }

        public string[] PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
