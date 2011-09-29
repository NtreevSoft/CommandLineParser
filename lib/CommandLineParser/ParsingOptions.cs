using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    [Flags]
    public enum ParsingOptions : int
    {
        None = 0,
        ShortNameOnly = 1,
        CaseSensitive = 2,


    }
}
