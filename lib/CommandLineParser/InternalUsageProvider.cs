using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    class InternalUsageProvider : UsageProvider
    {
        public InternalUsageProvider(SwitchDescriptor switchDescriptor)
            : base(switchDescriptor)
        {

        }

        public override string Usage
        {
            get 
            {
                string shortName = this.SwitchDescriptor.ShortName;
                string name = this.SwitchDescriptor.Name;

                string help;
                if(shortName == string.Empty)
                    help = string.Format("{0}{1}", SwitchAttribute.SwitchDelimiter, name);
                else
                    help = string.Format("{0}{1}", SwitchAttribute.SwitchDelimiter, shortName);

                char? argSeperator = this.SwitchDescriptor.ArgSeperator;
                Type argType = this.SwitchDescriptor.ArgType;
                if (argType != typeof(bool) || argSeperator != null)
                {
                    if (argSeperator == null)
                    {
                        help += string.Format(" <{0}>", argType.Name);
                    }
                    else
                    {
                        if (argSeperator != char.MinValue)
                            help += string.Format("<{0}{1}>", argSeperator, argType.Name);
                        else
                            help += string.Format("<{0}>", argType.Name);
                    }
                }
                return help;
            }
        }

        public override string Description
        {
            get 
            {
                string description = this.SwitchDescriptor.Description;

                if (this.SwitchDescriptor.DisplayName != this.SwitchDescriptor.Name)
                    description = this.SwitchDescriptor.DisplayName + " " + description;
                return description;

            }
        }

        public override string ArgumentTypeDescription
        {
            get
            {
                Type argType = this.SwitchDescriptor.ArgType;
                if (argType.IsEnum == false)
                    return base.ArgumentTypeDescription;

                string description = string.Empty;

                foreach(string name in Enum.GetNames(argType))
                {
                    if(description != string.Empty)
                    {
                        description += string.Format(", {0}", name);
                    }
                    else
                    {
                        description = name;
                    }
                }

                return string.Format("{0} is {1}", argType.Name, description);

            }
        }
    }
}
