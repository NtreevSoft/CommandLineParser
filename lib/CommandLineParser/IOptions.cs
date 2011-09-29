using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    public interface IOptions
    {
        object Instance
        {
            get;
        }

        SwitchAttributeCollection SwitchAttributes
        {
            get;
        }
    }
}
