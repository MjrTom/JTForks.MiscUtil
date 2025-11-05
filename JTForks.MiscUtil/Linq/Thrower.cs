// <copyright file="Thrower.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System;

    /// <summary>
    /// Helper class to allow exceptions to be thrown from within
    /// expression trees.
    /// </summary>
    internal static class Thrower
    {
        /// <summary>
        /// Throws the given exception, and returns a value of the given
        /// generic type. The return value is only to satisfy the compiler;
        /// the method will never actually return.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        internal static T Throw<T>(Exception e)
        {
            throw e;
        }
    }
}
