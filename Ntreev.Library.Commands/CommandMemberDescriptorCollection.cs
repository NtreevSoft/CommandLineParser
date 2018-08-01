//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Ntreev.Library.Commands.Properties;

namespace Ntreev.Library.Commands
{
    public class CommandMemberDescriptorCollection : IEnumerable<CommandMemberDescriptor>
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
                        orderby item.IsRequired descending
                        orderby item.IsExplicit
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
