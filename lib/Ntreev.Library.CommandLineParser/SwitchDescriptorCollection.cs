using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ntreev.Library
{
    public class SwitchDescriptorCollection : IReadOnlyList<SwitchDescriptor>
    {
        private readonly List<SwitchDescriptor> descriptors = new List<SwitchDescriptor>();

        internal SwitchDescriptorCollection()
        {

        }

        public SwitchDescriptor this[string name]
        {
            get
            {
                var query = from item in descriptors
                            where string.Compare(item.Name, name, true) == 0
                            select item;

                if (query.Count() == 0)
                    return null;

                return query.First();
            }
        }

        internal IList<SwitchDescriptor> List
        {
            get { return this.descriptors; }
        }


        public SwitchDescriptor this[int index]
        {
            get { return this.descriptors[index]; }
        }

        public int Count
        {
            get { return this.descriptors.Count; }
        }

        IEnumerator<SwitchDescriptor> IEnumerable<SwitchDescriptor>.GetEnumerator()
        {
            return this.descriptors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.descriptors.GetEnumerator();
        }
    }
}
