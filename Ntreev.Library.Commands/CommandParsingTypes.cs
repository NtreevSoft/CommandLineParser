using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [Flags]
    public enum CommandParsingTypes
    {
        None = 0,

        /// <summary>
        /// 명령구문에 이름을 포함하지 않고 인자값만 전달합니다.
        /// </summary>
        OmitCommandName = 1,

        /// <summary>
        /// 설정되지 않은 속성은 초기화합니다.
        /// </summary>
        Initialize = 2,
    }
}
