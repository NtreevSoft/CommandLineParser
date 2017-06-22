using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class ConsoleModeOnlyAttribute : Attribute
    {
        public ConsoleModeOnlyAttribute()
        {

        }
    }
}
