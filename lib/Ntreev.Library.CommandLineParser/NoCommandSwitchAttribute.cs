using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NoCommandSwitchAttribute : Attribute
    {
        public NoCommandSwitchAttribute()
        {

        }
    }
}
