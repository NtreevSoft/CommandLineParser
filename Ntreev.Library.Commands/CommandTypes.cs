using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [Flags]
    public enum CommandTypes
    {
        None = 0,

        /// <summary>
        /// 하위 명령을 가질수 있습니다.
        /// </summary>
        HasSubCommand = 1,
    }
}
