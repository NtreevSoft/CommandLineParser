using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    enum CommandPropertyTypes
    {
        /// <summary>
        /// 일반적인 형태로 스위치와 값을 입력받습니다.
        /// </summary>
        /// <example>
        /// cmd.exe --switch value
        /// </example>
        None = 0,

        /// <summary>
        /// 스위치 문이 필요하지 않고 값만 입력받습니다.
        /// </summary>
        /// <example>
        /// cmd.exe value1 value2
        /// </example>
        IsRequired = 1,

        /// <summary>
        /// 값이 없어도 기본값이 존재하면 기본값으로 대체합니다.
        /// </summary>
        /// <example>
        /// cmd.exe --switch value1   
        /// or
        /// cmd.exe --switch
        /// </example>
        IsImplicit = 2,

        /// <summary>
        /// 값이 필요하지 않으며 스위치값만 입력받습니다. 해당 속성은 기본값이 존재해야 합니다.
        /// </summary>
        /// /// <example>
        /// cmd.exe --switch
        /// </example>
        IsToggle = 3,
    }
}
