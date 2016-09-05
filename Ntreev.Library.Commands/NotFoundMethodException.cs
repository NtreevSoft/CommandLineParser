using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library.Commands
{
    public class NotFoundMethodException : Exception
    {
        public NotFoundMethodException(string methodName)
            : base(methodName + "는 존재하지 않는 명령입니다.")
        {

        }

        public NotFoundMethodException()
            : base("명령을 찾을 수가 없습니다.")
        {

        }
    }
}
