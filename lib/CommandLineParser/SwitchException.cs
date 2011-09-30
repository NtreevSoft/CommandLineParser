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

        /// <summary>
        /// <seealso cref="SwitchException"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public SwitchException()
        {
            
        }

        /// <summary>
        /// 오류 메세지를 사용하여 <seealso cref="SwitchException"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="message">오류 메세지를 나타내는 문자열입니다.</param>
        public SwitchException(string message)
             : base(message)
        {

        }

        /// <summary>
        /// 오류 메세지와 오류의 원인이 되는 스위치의 이름을 사용하여 <seealso cref="SwitchException"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="message">오류 메세지를 나타내는 문자열입니다.</param>
        /// <param name="switchName">오류의 원인이 되는 스위치의 이름입니다.</param>
        public SwitchException(string message, string switchName)
            : base(message)
        {
            this.switchName = switchName;
        }

        /// <summary>
        /// 오류 메세지와 해당 예외의 근본 원인인 내부 예외에 대한 참조를 사용하여 <seealso cref="SwitchException"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="message">오류 메세지를 나타내는 문자열입니다.</param>
        /// <param name="innerException">해당 예외의 근본 원인인 내부 예외에 대한 인스턴스입니다.</param>
        public SwitchException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// 오류 메세지와 오류의 원인이 되는 스위치의 이름 그리고 
        /// 해당 예외의 근본 원인인 내부 예외에 대한 참조를 사용하여 <seealso cref="SwitchException"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="message">오류 메세지를 나타내는 문자열입니다.</param>
        /// <param name="switchName">오류의 원인이 되는 스위치의 이름입니다.</param>
        /// <param name="innerException">해당 예외의 근본 원인인 내부 예외에 대한 인스턴스입니다.</param>
        public SwitchException(string message, string switchName, Exception innerException)
            : base(message, innerException)
        {
            this.switchName = switchName;
        }
        
        /// <summary>
        /// 오류의 원인이 되었던 스위치의 이름을 가져옵니다.
        /// </summary>
        public string SwitchName
        {
            get { return this.switchName; }
        }
    }
}
