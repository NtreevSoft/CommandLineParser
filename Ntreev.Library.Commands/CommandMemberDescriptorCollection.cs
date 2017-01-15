using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public class CommandMemberDescriptorCollection : IReadOnlyList<CommandMemberDescriptor>
    {
        private readonly List<CommandMemberDescriptor> descriptors = new List<CommandMemberDescriptor>();

        internal CommandMemberDescriptorCollection()
        {

        }

        public CommandMemberDescriptor this[string name]
        {
            get
            {
                var query = from item in descriptors
                            where item.DescriptorName == name
                            select item;

                if (query.Any() == false)
                    throw new KeyNotFoundException(string.Format(Resources.MemberDoesNotExist_Format, name));

                return query.First();
            }
        }

        public CommandMemberDescriptor this[int index]
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
                        orderby item.DefaultValue == DBNull.Value descending
                        orderby item.Required descending
                        orderby item is CommandMemberArrayDescriptor
                        select item;

            var items = query.ToArray();
            this.descriptors.Clear();
            this.descriptors.AddRange(items);
        }

        internal void Add(CommandMemberDescriptor descriptor)
        {
            foreach (var item in this.descriptors)
            {
                if (item.Name != string.Empty && descriptor.Name != string.Empty && descriptor.Name == item.Name)
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

        internal void AddRange(IEnumerable<CommandMemberDescriptor> descriptors)
        {
            foreach(var item in descriptors)
            {
                this.Add(item);
            }
        }

        #region IEnumerable

        IEnumerator<CommandMemberDescriptor> IEnumerable<CommandMemberDescriptor>.GetEnumerator()
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
