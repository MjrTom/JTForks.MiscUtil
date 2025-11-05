// <copyright file="EditableLookup.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System.Collections.Generic;
    using System.Linq;
    using MiscUtil.Extensions;

    /// <summary>
    /// Simple non-unique map wrapper
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <remarks>
    /// ApplyResultSelector (from Lookup[TKey, TElement] is not implemented,
    /// since the caller could just as easily (or more-so) use .Select() with
    /// a Func[IGrouping[TKey, TElement], TResult], since
    /// IGrouping[TKey, TElement] already includes both the "TKey Key"
    /// and the IEnumerable[TElement].
    /// </remarks>
    /// <remarks>
    /// Creates a new EditableLookup using the specified key-comparer
    /// </remarks>
    /// <param name="keyComparer"></param>
    public sealed partial class EditableLookup<TKey, TElement>(IEqualityComparer<TKey> keyComparer) : ILookup<TKey, TElement>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, LookupGrouping> groups = new(
                keyComparer ?? EqualityComparer<TKey>.Default);
        /// <summary>
        /// Creates a new EditableLookup using the default key-comparer
        /// </summary>
        public EditableLookup()
            : this(null!) { }

        /// <summary>
        /// Does the lookup contain any value(s) for the given key?
        /// </summary>
        /// <param name="key"></param>
        public bool Contains(TKey key)
        {
            return this.groups.TryGetValue(key, out LookupGrouping? group) && group.Count > 0;
        }

        /// <summary>
        /// Does the lookup the specific key/value pair?
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool Contains(TKey key, TElement value)
        {
            return this.groups.TryGetValue(key, out LookupGrouping? group) && group.Contains(value);
        }

        /// <summary>
        /// Adds a key/value pair to the lookup
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <remarks>If the value is already present it will be duplicated</remarks>
        public void Add(TKey key, TElement value)
        {
            if (!this.groups.TryGetValue(key, out LookupGrouping? group))
            {
                group = new LookupGrouping(key);
                this.groups.Add(key, group);
            }

            group.Add(value);
        }

        /// <summary>
        /// Adds a range of values against a single key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <remarks>Any values already present will be duplicated</remarks>
        public void AddRange(TKey key, IEnumerable<TElement> values)
        {
            values.ThrowIfNull("values");
            if (!this.groups.TryGetValue(key, out LookupGrouping? group))
            {
                group = new LookupGrouping(key);
                this.groups.Add(key, group);
            }

            foreach (TElement value in values)
            {
                group.Add(value);
            }

            if (group.Count == 0)
            {
                this.groups.Remove(key); // nothing there after all!
            }
        }

        /// <summary>
        /// Add all key/value pairs from the supplied lookup
        /// to the current lookup
        /// </summary>
        /// <param name="lookup"></param>
        /// <remarks>Any values already present will be duplicated</remarks>
        public void AddRange(ILookup<TKey, TElement> lookup)
        {
            lookup.ThrowIfNull("lookup"); ;
            foreach (IGrouping<TKey, TElement> group in lookup)
            {
                this.AddRange(group.Key, group);
            }
        }

        /// <summary>
        /// Remove all values from the lookup for the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>True if any items were removed, else false</returns>
        public bool Remove(TKey key)
        {
            return this.groups.Remove(key);
        }

        /// <summary>
        /// Remove the specific key/value pair from the lookup
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>True if the item was found, else false</returns>
        public bool Remove(TKey key, TElement value)
        {
            if (this.groups.TryGetValue(key, out LookupGrouping? group))
            {
                var removed = group.Remove(value);
                if (removed && group.Count == 0)
                {
                    this.groups.Remove(key);
                }

                return removed;
            }

            return false;
        }

        /// <summary>
        /// Trims the inner data-structure to remove
        /// any surplus space
        /// </summary>
        public void TrimExcess()
        {
            foreach (LookupGrouping group in this.groups.Values)
            {
                group.TrimExcess();
            }
        }

        /// <summary>
        /// Returns the number of dictinct keys in the lookup
        /// </summary>
        public int Count => this.groups.Count;

        private static readonly IEnumerable<TElement> Empty = [];
        /// <summary>
        /// Returns the set of values for the given key
        /// </summary>
        /// <param name="key"></param>
        public IEnumerable<TElement> this[TKey key] => this.groups.TryGetValue(key, out LookupGrouping? group) ? group : Empty;
        /// <summary>
        /// Returns the sequence of keys and their contained values
        /// </summary>
        public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
        {
            foreach (LookupGrouping group in this.groups.Values)
            {
                yield return group;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
