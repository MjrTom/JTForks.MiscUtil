// <copyright file="PartialComparer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    using System.Collections.Generic;

    /// <summary>
    /// Class to provide partial comparisons, which can be useful when
    /// implementing full comparisons in other classes.
    /// </summary>
    public static class PartialComparer
    {
        /// <summary>
        /// Provides comparisons for just the references, returning 0
        /// if the arguments are the same reference, -1 if just the first is null,
        /// and 1 if just the second is null. Otherwise, this method returns null.
        /// It can be used to make life easier for an initial comparison 
        /// before comparing individual components of an object.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare</typeparam>
        /// <param name="first">The first object to compare</param>
        /// <param name="second">The second object to compare</param>
        public static int? ReferenceCompare<T>(T first, T second)
            where T : class
        {
            return first == second ? 0 : first == null ? -1 : second == null ? 1 : null;
        }

        /// <summary>
        /// Compares two instances of T using the default comparer for T,
        /// returning a non-null value in the case of inequality, or null 
        /// where the default comparer would return 0. This aids chained 
        /// comparisons (where if the first values are equal, you move 
        /// on to the next ones) if you use the null coalescing operator.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare</typeparam>
        /// <param name="first">The first object to compare</param>
        /// <param name="second">The second object to compare</param>
        public static int? Compare<T>(T first, T second)
        {
            return Compare(Comparer<T>.Default, first, second);
        }

        /// <summary>
        /// Compares two instances of T using the specified comparer for T,
        /// returning a non-null value in the case of inequality, or null 
        /// where the comparer would return 0. This aids chained 
        /// comparisons (where if the first values are equal, you move 
        /// on to the next ones) if you use the null coalescing operator.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare</typeparam>
        /// <param name="comparer">The comparer to use</param>
        /// <param name="first">The first object to compare</param>
        /// <param name="second">The second object to compare</param>
        public static int? Compare<T>(IComparer<T> comparer, T first, T second)
        {
            var ret = comparer.Compare(first, second);
            return ret == 0 ? null : ret;
        }

        /// <summary>
        /// Compares two instances of T, returning true if they are definitely
        /// the same (i.e. the same references), false if exactly one reference is
        /// null, or null otherwise. This aids implementing equality operations.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare</typeparam>
        /// <param name="first">The first object to compare</param>
        /// <param name="second">The second object to compare</param>
        public static bool? Equals<T>(T first, T second)
            where T : class
        {
            return first == second ? true : first == null || second == null ? false : null;
        }
    }
}
