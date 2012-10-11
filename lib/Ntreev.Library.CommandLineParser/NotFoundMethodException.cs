using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    public class NotFoundMethodException : Exception
    {
        public NotFoundMethodException(string methodName)
            : base(methodName + "는 존재하지 않는 함수입니다.")
        {

        }
    }
}
