// <copyright file="ProjectionEqualityComparer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using MiscUtil.Extensions;

    /// <summary>
    /// Non-generic class to produce instances of the generic class,
    /// optionally using type inference.
    /// </summary>
    public static class ProjectionEqualityComparer
    {
        /// <summary>
        /// Creates an instance of ProjectionEqualityComparer using the specified projection.
        /// </summary>
        /// <typeparam name="TSource">Type parameter for the elements to be compared.</typeparam>
        /// <typeparam name="TKey">Type parameter for the keys to be compared, after being projected from the elements.</typeparam>
        /// <param name="projection">Projection to use when determining the key of an element.</param>
        /// <returns>A comparer which will compare elements by projecting each element to its key, and comparing keys.</returns>
        public static ProjectionEqualityComparer<TSource, TKey> Create<TSource, TKey>(Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }

        /// <summary>
        /// Creates an instance of ProjectionEqualityComparer using the specified projection.
        /// The ignored parameter is solely present to aid type inference.
        /// </summary>
        /// <typeparam name="TSource">Type parameter for the elements to be compared.</typeparam>
        /// <typeparam name="TKey">Type parameter for the keys to be compared, after being projected from the elements.</typeparam>
        /// <param name="ignored">Value is ignored - type may be used by type inference.</param>
        /// <param name="projection">Projection to use when determining the key of an element.</param>
        /// <returns>A comparer which will compare elements by projecting each element to its key, and comparing keys.</returns>
        public static ProjectionEqualityComparer<TSource, TKey> Create<TSource, TKey>(
            TSource ignored,
            Func<TSource, TKey> projection)
        {
            return new ProjectionEqualityComparer<TSource, TKey>(projection);
        }

    }

    /// <summary>
    /// Comparer which projects each element of the comparison to a key, and then compares
    /// those keys using the specified (or default) comparer for the key type.
    /// </summary>
    /// <typeparam name="TSource">Type of elements which this comparer will be asked to compare.</typeparam>
    /// <typeparam name="TKey">Type of the key projected from the element.</typeparam>
    public class ProjectionEqualityComparer<TSource, TKey> : IEqualityComparer<TSource>, System.Collections.IEqualityComparer
    {
        private readonly Func<TSource, TKey> projection;
        private readonly IEqualityComparer<TKey> comparer;

        /// <summary>
        /// Creates a new instance using the specified projection, which must not be null.
        /// The default comparer for the projected type is used.
        /// </summary>
        /// <param name="projection">Projection to use during comparisons.</param>
        public ProjectionEqualityComparer(Func<TSource, TKey> projection)
            : this(projection, EqualityComparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Creates a new instance using the specified projection, which must not be null.
        /// </summary>
        /// <param name="projection">Projection to use during comparisons.</param>
        /// <param name="comparer">The comparer to use on the keys. May be null, in
        /// which case the default comparer will be used.</param>
        public ProjectionEqualityComparer(Func<TSource, TKey> projection, IEqualityComparer<TKey>? comparer)
        {
            projection.ThrowIfNull(nameof(projection));
            this.comparer = comparer ?? EqualityComparer<TKey>.Default;
            this.projection = projection;
        }

        /// <summary>
        /// Compares the two specified values for equality by applying the projection
        /// to each value and then using the equality comparer on the resulting keys.
        /// Null references are never passed to the projection.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TSource? x, TSource? y)
        {
            return (x == null && y == null) || (x != null && y != null && this.comparer.Equals(this.projection(x), this.projection(y)));
        }

        /// <summary>
        /// Produces a hash code for the given value by projecting it and
        /// then asking the equality comparer to find the hash code of
        /// the resulting key.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode([DisallowNull] TSource obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            return this.comparer.GetHashCode(this.projection(obj)!);
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <exception cref="ArgumentException">
        ///   <paramref name="x" /> and <paramref name="y" /> are of different types and neither one can handle comparisons with the other.</exception>
        /// <returns>
        ///   <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />.</returns>
        public new bool Equals(object? x, object? y)
        {
            return x == y || (x != null && y != null && (x is TSource a
                && y is TSource b
                ? this.Equals(a, b)
                :
            throw new ArgumentException("", nameof(x))));
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="object" /> for which a hash code is to be returned.</param>
        /// <exception cref="ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is <see langword="null" />.</exception>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(object obj)
        {
            return obj == null ? 0 : obj is TSource x ? this.GetHashCode(x) : throw new ArgumentException("", nameof(obj));
        }
    }
}
