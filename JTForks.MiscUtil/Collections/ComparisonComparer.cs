// <copyright file="ComparisonComparer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A utility class for creating Comparison delegates.
    /// </summary>
    public static class ComparisonComparer
    {
        /// <summary>
        /// Creates a Comparison delegate from the given Comparer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comparer">Comparer to use when the returned delegate is called. Must not be null.</param>
        /// <returns>A Comparison delegate which proxies to the given Comparer.</returns>
        public static Comparison<T> CreateComparison<T>(IComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            return comparer.Compare;
        }
    }

    /// <summary>
    /// Utility to build an IComparer implementation from a Comparison delegate,
    /// and a static method to do the reverse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Creates a new instance which will proxy to the given Comparison
    /// delegate when called.
    /// </remarks>
    /// <param name="comparison">Comparison delegate to proxy to. Must not be null.</param>
    public sealed class ComparisonComparer<T>(Comparison<T?> comparison) : IComparer<T>, System.Collections.IComparer
    {
        private readonly Comparison<T?> comparison = comparison ?? throw new ArgumentNullException(nameof(comparison));

        /// <summary>
        /// Implementation of IComparer.Compare which simply proxies
        /// to the originally specified Comparison delegate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public int Compare(T? x, T? y)
        {
            return this.comparison(x, y);
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
            throw new ArgumentException("", nameof(x));
        }
    }
}
