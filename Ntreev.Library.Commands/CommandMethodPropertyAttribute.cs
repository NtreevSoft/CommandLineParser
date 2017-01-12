using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    /// <summary>
    /// SubCommand로 사용할 메소드에 추가적으로 사용할 속성을 설정합니다.
    /// 속성의 이름은 여러개를 설정할 수 있으며 해당 클래스내에 CommandSwitch 특성을 갖고 있는 public 속성이여야만 합니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandMethodPropertyAttribute : Attribute
    {
        private readonly string[] propertyNames;

        public CommandMethodPropertyAttribute(params string[] propertyNames)
        {
            if (propertyNames.Any() == false)
                throw new InvalidOperationException("최소 1개 이상의 속성이 설정되어야만 합니다.");
            this.propertyNames = propertyNames;
        }

        public string[] PropertyNames
        {
            get { return this.propertyNames; }
        }
    }
}
