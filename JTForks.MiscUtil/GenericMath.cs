// <copyright file="GenericMath.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    using System.Numerics;

    /// <summary>
    /// Generic math equivalents of System.Math.
    /// (Calling this just Math makes far too much mess.)
    /// </summary>
    public static class GenericMath
    {
        /// <summary>
        /// Returns the absolute value of a specified number.
        /// </summary>
        /// <typeparam name="T">Type to calculate with</typeparam>
        /// <param name="input">Input to return the absolute value of.</param>
        /// <returns>The input value if it is greater than or equal to the default value of T,
        /// or the negated input value otherwise</returns>
        public static T Abs<T>(T input)
            where T : INumber<T>
        {
            return T.Abs(input);
        }

        /// <summary>
        /// Returns whether or not two inputs are "close" to each other with respect to a given delta.
        /// </summary>
        /// <remarks>
        /// This implementation currently does no overflow checking - if (input1-input2) overflows, it
        /// could yield the wrong result.
        /// </remarks>
        /// <typeparam name="T">Type to calculate with</typeparam>
        /// <param name="input1">First input value</param>
        /// <param name="input2">Second input value</param>
        /// <param name="delta">Permitted range (exclusive)</param>
        /// <returns>True if Abs(input1-input2) is less than or equal to delta; false otherwise.</returns>
        public static bool WithinDelta<T>(T input1, T input2, T delta)
            where T : INumber<T>
        {
            return Abs(input1 - input2) <= delta;
        }
    }
}
