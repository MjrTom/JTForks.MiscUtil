// <copyright file="KeyValueTuple.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    /// <summary>
    /// Generic tuple for a key and a single value
    /// </summary>
    /// <typeparam name="TKey">The Type of the key</typeparam>
    /// <typeparam name="T">The Type of the value</typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <remarks>
    /// Creates a new tuple with the given key and value
    /// </remarks>
    public readonly struct KeyValueTuple<TKey, T>(TKey key, T value)
    {
        /// <summary>
        /// The key for the tuple
        /// </summary>
        public readonly TKey Key { get; } = key;
        /// <summary>
        /// The value for the tuple
        /// </summary>
        public readonly T Value { get; } = value;
    }

    /// <summary>
    /// Generic tuple for a key and a pair of values
    /// </summary>
    /// <typeparam name="TKey">The Type of the key</typeparam>
    /// <typeparam name="T1">The Type of the first value</typeparam>
    /// <typeparam name="T2">The Type of the second value</typeparam>
    /// <param name="key"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>    
    /// <remarks>
    /// Creates a new tuple with the given key and values
    /// </remarks>
    public readonly struct KeyValueTuple<TKey, T1, T2>(TKey key, T1 value1, T2 value2)
    {

        /// <summary>
        /// The key for the tuple
        /// </summary>
        public readonly TKey Key { get; } = key;
        /// <summary>
        /// The first value
        /// </summary>
        public readonly T1 Value1 { get; } = value1;
        /// <summary>
        /// The second value
        /// </summary>
        public readonly T2 Value2 { get; } = value2;
    }

    /// <summary>
    /// Generic tuple for a key and a trio of values
    /// </summary>
    /// <typeparam name="TKey">The Type of the key</typeparam>
    /// <typeparam name="T1">The Type of the first value</typeparam>
    /// <typeparam name="T2">The Type of the second value</typeparam>
    /// <typeparam name="T3">The Type of the third value</typeparam>
    /// <param name="key"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <remarks>
    /// Creates a new tuple with the given key and values
    /// </remarks>
    public readonly struct KeyValueTuple<TKey, T1, T2, T3>(TKey key, T1 value1, T2 value2, T3 value3)
    {
        /// <summary>
        /// The key for the tuple
        /// </summary>
        public readonly TKey Key { get; } = key;
        /// <summary>
        /// The first value
        /// </summary>
        public readonly T1 Value1 { get; } = value1;
        /// <summary>
        /// The second value
        /// </summary>
        public readonly T2 Value2 { get; } = value2;
        /// <summary>
        /// The third value
        /// </summary>
        public readonly T3 Value3 { get; } = value3;
    }

    /// <summary>
    /// Generic tuple for a key and a quartet of values
    /// </summary>
    /// <typeparam name="TKey">The Type of the key</typeparam>
    /// <typeparam name="T1">The Type of the first value</typeparam>
    /// <typeparam name="T2">The Type of the second value</typeparam>
    /// <typeparam name="T3">The Type of the third value</typeparam>
    /// <typeparam name="T4">The Type of the fourth value</typeparam>
    /// <param name="key"></param>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="value3"></param>
    /// <param name="value4"></param>
    /// <remarks>
    /// Creates a new tuple with the given key and values
    /// </remarks>
    public readonly struct KeyValueTuple<TKey, T1, T2, T3, T4>(TKey key, T1 value1, T2 value2, T3 value3, T4 value4)
    {

        /// <summary>
        /// The key for the tuple
        /// </summary>
        public readonly TKey Key { get; } = key;
        /// <summary>
        /// The first value
        /// </summary>
        public readonly T1 Value1 { get; } = value1;
        /// <summary>
        /// The second value
        /// </summary>
        public readonly T2 Value2 { get; } = value2;
        /// <summary>
        /// The third value
        /// </summary>
        public readonly T3 Value3 { get; } = value3;
        /// <summary>
        /// The fourth value
        /// </summary>
        public readonly T4 Value4 { get; } = value4;
    }
}
