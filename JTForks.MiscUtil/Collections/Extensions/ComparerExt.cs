// <copyright file="ComparerExt.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections.Extensions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extensions to IComparer
    /// </summary>
    public static class ComparerExt
    {
        /// <summary>
        /// Reverses the original comparer; if it was already a reverse comparer,
        /// the previous version was reversed (rather than reversing twice).
        /// In other words, for any comparer X, X==X.Reverse().Reverse().
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        public static IComparer<T> Reverse<T>(this IComparer<T> original)
        {
            return original is ReverseComparer<T> originalAsReverse ? originalAsReverse.OriginalComparer : new ReverseComparer<T>(original);
        }

        /// <summary>
        /// Combines a comparer with a second comparer to implement composite sort behavior.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstComparer"></param>
        /// <param name="secondComparer"></param>
        public static IComparer<T> ThenBy<T>(this IComparer<T> firstComparer, IComparer<T> secondComparer)
        {
            return new LinkedComparer<T>(firstComparer, secondComparer);
        }

        /// <summary>
        /// Combines a comparer with a projection to implement composite sort behavior.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="firstComparer"></param>
        /// <param name="projection"></param>
        public static IComparer<T> ThenBy<T, TKey>(this IComparer<T> firstComparer, Func<T, TKey> projection)
        {
            return new LinkedComparer<T>(firstComparer, new ProjectionComparer<T, TKey>(projection));
        }
    }
}
