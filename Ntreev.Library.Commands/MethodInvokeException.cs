using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    public class MethodInvokeException : Exception
    {
        public MethodInvokeException(string methodName, Exception exception)
            : base(methodName + " 메소드 실행중 처리되지 않은 예외가 발생하였습니다", exception)
        {
            
        }
    }
}
