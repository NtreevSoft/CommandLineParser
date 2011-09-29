using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    /// <summary>
    /// 필요한 스위치가 없을때 발생하는 예외를 나타냅니다.
    /// </summary>
    public class MissingSwitchException : SwitchException
    {
        public MissingSwitchException()
        {
            
        }

        public MissingSwitchException(string message)
             : base(message)
        {

        }

        public MissingSwitchException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public MissingSwitchException(string message, string switchName)
            : base(message, switchName)
        {
            
        }

        public MissingSwitchException(string message, string switchName, Exception innerException)
            : base(message, switchName, innerException)
        {

        }
    }
}
