using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UsageBrowsableAttribute : Attribute
    {
        private readonly bool isBrowsable;

        public UsageBrowsableAttribute(bool isBrowsable)
        {
            this.isBrowsable = isBrowsable;
        }

        public bool IsBrowsable
        {
            get { return this.isBrowsable; }
        }
    }
}
