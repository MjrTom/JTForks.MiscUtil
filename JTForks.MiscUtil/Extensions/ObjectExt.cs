// <copyright file="ObjectExt.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Extensions
{
    using System;

    /// <summary>
    /// Extension methods on all reference types.
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Throws an ArgumentNullException if the given data item is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The item to check for nullity.</param>
        /// <param name="name">The name to use when throwing an exception, if necessary</param>
        public static void ThrowIfNull<T>(this T data, string name)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(data, name);
        }

        /// <summary>
        /// Throws an ArgumentNullException if the given data item is null.
        /// No parameter name is specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The item to check for nullity.</param>
        public static void ThrowIfNull<T>(this T data)
            where T : class
        {
            ArgumentNullException.ThrowIfNull(data);
        }
    }
}
