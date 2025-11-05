// <copyright file="SmartEnumerable.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Static class to make creation easier. If possible though, use the extension
    /// method in SmartEnumerableExt.
    /// </summary>
    public static class SmartEnumerable
    {
        /// <summary>
        /// Extension method to make life easier.
        /// </summary>
        /// <typeparam name="T">Type of enumerable</typeparam>
        /// <param name="source">Source enumerable</param>
        /// <returns>A new SmartEnumerable of the appropriate type</returns>
        public static SmartEnumerable<T> Create<T>(IEnumerable<T> source)
        {
            return new SmartEnumerable<T>(source);
        }
    }

    /// <summary>
    /// Type chaining an IEnumerable&lt;T&gt; to allow the iterating code
    /// to detect the first and last entries simply.
    /// </summary>
    /// <typeparam name="T">Type to iterate over</typeparam>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="enumerable">Collection to enumerate. Must not be null.</param>
    public class SmartEnumerable<T>(IEnumerable<T> enumerable) : IEnumerable<SmartEnumerable<T>.Entry>
    {
        /// <summary>
        /// Enumerable we proxy to
        /// </summary>
        private readonly IEnumerable<T> enumerable = enumerable ?? throw new ArgumentNullException("enumerable");

        /// <summary>
        /// Returns an enumeration of Entry objects, each of which knows
        /// whether it is the first/last of the enumeration, as well as the
        /// current value.
        /// </summary>
        public IEnumerator<Entry> GetEnumerator()
        {
            using IEnumerator<T> enumerator = this.enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            var isFirst = true;
            var isLast = false;
            var index = 0;
            while (!isLast)
            {
                T current = enumerator.Current;
                isLast = !enumerator.MoveNext();
                yield return new Entry(isFirst, isLast, current, index++);
                isFirst = false;
            }
        }

        /// <summary>
        /// Non-generic form of GetEnumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Represents each entry returned within a collection,
        /// containing the value and whether it is the first and/or
        /// the last entry in the collection's. enumeration
        /// </summary>
        public class Entry
        {
            internal Entry(bool isFirst, bool isLast, T value, int index)
            {
                this.IsFirst = isFirst;
                this.IsLast = isLast;
                this.Value = value;
                this.Index = index;
            }

            /// <summary>
            /// The value of the entry.
            /// </summary>
            public T Value { get; }

            /// <summary>
            /// Whether or not this entry is first in the collection's enumeration.
            /// </summary>
            public bool IsFirst { get; }

            /// <summary>
            /// Whether or not this entry is last in the collection's enumeration.
            /// </summary>
            public bool IsLast { get; }

            /// <summary>
            /// The 0-based index of this entry (i.e. how many entries have been returned before this one)
            /// </summary>
            public int Index { get; }
        }
    }
}
