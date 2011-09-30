using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    /// <summary>
    /// 파싱 옵션을 설정하는 데 사용하는 열거형 값을 제공합니다.
    /// </summary>
    [Flags]
    public enum ParsingOptions : int
    {
        /// <summary>
        /// 옵션이 설정되지 않도록 지정합니다.
        /// </summary>
        None = 0,

        /// <summary>
        /// 짧은 이름을 가진 특성의 스위치만 파싱합니다.
        /// </summary>
        ShortNameOnly = 1,

        /// <summary>
        /// 파싱할때 스위치 이름의 대소문자를 구분합니다.
        /// </summary>
        CaseSensitive = 2,


    }
}
