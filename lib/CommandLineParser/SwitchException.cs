using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    /// <summary>
    /// 유효하지 않은 스위치 또는 잘못된 인자가 사용되었을때 발생하는 예외를 나타냅니다.
    /// </summary>
    public class SwitchException : Exception
    {
        string switchName = string.Empty;

        public SwitchException()
        {
            
        }

        public SwitchException(string message)
             : base(message)
        {

        }

        public SwitchException(string message, string switchName)
            : base(message)
        {
            this.switchName = switchName;
        }

        public SwitchException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public SwitchException(string message, string switchName, Exception innerException)
            : base(message, innerException)
        {
            this.switchName = switchName;
        }
        
        public string SwitchName
        {
            get { return this.switchName; }
        }
    }
}
