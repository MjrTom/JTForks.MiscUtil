// <copyright file="ReverseComparer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System.Collections.Generic;
    using MiscUtil.Extensions;

    /// <summary>
    /// Implementation of IComparer{T} based on another one;
    /// this simply reverses the original comparison.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>, System.Collections.IComparer
    {
        /// <summary>
        /// Returns the original comparer; this can be useful to avoid multiple
        /// reversals.
        /// </summary>
        public IComparer<T> OriginalComparer { get; }

        /// <summary>
        /// Creates a new reversing comparer.
        /// </summary>
        /// <param name="original">The original comparer to use for comparisons.</param>
        public ReverseComparer(IComparer<T> original)
        {
            original.ThrowIfNull("original");
            this.OriginalComparer = original;
        }

        /// <summary>
        /// Returns the result of comparing the specified values using the original
        /// comparer, but reversing the order of comparison.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int Compare(T? x, T? y)
        {
            return this.OriginalComparer.Compare(y, x);
        }

        /// <inheritdoc/>
        public int Compare(object? x, object? y)
        {
            return x == y
                ? 0
                : x == null
                ? -1
                : y == null
                ? 1
                : x is T a
                && y is T b
                ? this.Compare(a, b)
                :
            throw new System.ArgumentException("", nameof(x));
        }
    }
}
