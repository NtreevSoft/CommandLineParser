using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ntreev.Library
{
    class SwitchDescriptorCollection : List<SwitchDescriptor>
    {
        public SwitchDescriptorCollection()
        {

        }

        public SwitchDescriptor this[string name]
        {
            get
            {
                var query = from item in this
                            where item.Name == name
                            select item;

                if (query.Count() == 0)
                    return null;

                return query.ToArray()[0];
            }
        }
    }
}
