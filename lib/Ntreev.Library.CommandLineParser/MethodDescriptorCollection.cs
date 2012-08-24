using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    class MethodDescriptorCollection : List<MethodDescriptor>
    {
        public MethodDescriptorCollection()
        {

        }

        public MethodDescriptor this[string name]
        {
            get
            {
                var query = from item in this
                            where item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                            select item;

                if (query.Count() == 0)
                    return null;

                return query.ToArray()[0];
            }
        }

    }
}
