using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Ntreev.Library.Commands
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
                            where item.OriginalName == name
                            select item;

                if (query.Count() == 0)
                    return null;

                return query.First();
            }
        }

        public SwitchDescriptor this[int index]
        {
            get { return this.descriptors[index]; }
        }

        public int Count
        {
            get { return this.descriptors.Count; }
        }

        internal void Sort()
        {
            var query = from item in this.descriptors
                        orderby item.Required descending
                        select item;

            var items = query.ToArray();
            this.descriptors.Clear();
            this.descriptors.AddRange(items);
        }

        internal void Add(SwitchDescriptor descriptor)
        {
            foreach(var item in this.descriptors)
            {
                if(item.Name != string.Empty && descriptor.Name != string.Empty && descriptor.Name == item.Name)
                {
                    throw new ArgumentException(string.Format("{0} 은(는) 이미 존재하는 이름입니다.", descriptor.Name));
                }

                if (item.ShortName != string.Empty && descriptor.ShortName != string.Empty && descriptor.ShortName == item.ShortName)
                {
                    throw new ArgumentException(string.Format("{0} 은(는) 이미 존재하는 이름입니다.", descriptor.ShortName));
                }
            }

            this.descriptors.Add(descriptor);
        }

        #region IEnumerable

        IEnumerator<SwitchDescriptor> IEnumerable<SwitchDescriptor>.GetEnumerator()
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
