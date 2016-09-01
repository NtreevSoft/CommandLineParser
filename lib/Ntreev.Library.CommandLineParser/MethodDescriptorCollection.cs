using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntreev.Library
{
    public class MethodDescriptorCollection : IReadOnlyList<MethodDescriptor>
    {
        private readonly List<MethodDescriptor> descriptors = new List<MethodDescriptor>();

        internal MethodDescriptorCollection()
        {

        }

        public MethodDescriptor this[string name]
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

        public MethodDescriptor this[int index]
        {
            get { return this.descriptors[index]; }
        }

        public int Count
        {
            get { return this.descriptors.Count; }
        }

        internal void Add(MethodDescriptor item)
        {
            this.descriptors.Add(item);
        }

        #region IEnumerable

        IEnumerator<MethodDescriptor> IEnumerable<MethodDescriptor>.GetEnumerator()
        {
            return this.descriptors.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.descriptors.GetEnumerator();
        }

        #endregion
    }
}
