using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    public abstract class UsageProvider
    {
        readonly SwitchDescriptor switchDescriptor;

        public UsageProvider(SwitchDescriptor switchDescriptor)
        {
            this.switchDescriptor = switchDescriptor;
        }

        protected SwitchDescriptor SwitchDescriptor
        {
            get { return this.switchDescriptor; }
        }

        public abstract string Usage
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public virtual string ArgumentTypeDescription
        {
            get { return string.Empty; }
        }
    }
}
