using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.Library.Commands
{
    [AttributeUsage(AttributeTargets.All)]
    public class SummaryAttribute : Attribute
    {
        private readonly string summary;

        public SummaryAttribute(string summary)
        {
            this.summary = summary;
        }

        public string Summary
        {
            get { return this.summary ?? string.Empty; }
        }
    }
}
