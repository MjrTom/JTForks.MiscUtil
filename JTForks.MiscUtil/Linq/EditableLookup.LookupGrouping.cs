// <copyright file="EditableLookup.LookupGrouping.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System.Collections.Generic;
    using System.Linq;

    public partial class EditableLookup<TKey, TElement>
    {
        internal sealed class LookupGrouping(TKey key) : IGrouping<TKey, TElement>
        {
            private readonly List<TElement> items = [];
            public TKey Key { get; } = key;

            public int Count => this.items.Count;
            public void Add(TElement item)
            {
                this.items.Add(item);
            }

            public bool Contains(TElement item)
            {
                return this.items.Contains(item);
            }

            public bool Remove(TElement item)
            {
                return this.items.Remove(item);
            }

            public void TrimExcess()
            {
                this.items.TrimExcess();
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                return this.items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
