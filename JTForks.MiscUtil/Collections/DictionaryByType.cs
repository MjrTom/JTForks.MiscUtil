// <copyright file="DictionaryByType.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Map from types to instances of those types, e.g. int to 10 and
    /// string to "hi" within the same dictionary. This cannot be done
    /// without casting (and boxing for value types) as .NET cannot
    /// represent this relationship with generics in their current form.
    /// This class encapsulates the nastiness in a single place.
    /// </summary>
    public class DictionaryByType
    {
        private readonly Dictionary<Type, object> dictionary = [];

        /// <summary>
        /// Maps the specified type argument to the given value. If
        /// the type argument already has a value within the dictionary,
        /// ArgumentException is thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        public void Add<T>(T value)
            where T : notnull
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!this.dictionary.TryAdd(typeof(T), value))
            {
                throw new ArgumentException("Type already has a value in the dictionary", nameof(value));
            }
        }

        /// <summary>
        /// Maps the specified type argument to the given value. If
        /// the type argument already has a value within the dictionary, it
        /// is overwritten.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public void Put<T>(T value)
            where T : notnull
        {
            this.dictionary[typeof(T)] = value;
        }

        /// <summary>
        /// Attempts to fetch a value from the dictionary, throwing a
        /// KeyNotFoundException if the specified type argument has no
        /// entry in the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Get<T>()
            where T : notnull
        {
            return (T)this.dictionary[typeof(T)];
        }

        /// <summary>
        /// Attempts to fetch a value from the dictionary, returning false and
        /// setting the output parameter to the default value for T if it
        /// fails, or returning true and setting the output parameter to the
        /// fetched value if it succeeds.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        public bool TryGet<T>([MaybeNullWhen(false)] out T value)
            where T : notnull
        {
            if (this.dictionary.TryGetValue(typeof(T), out var tmp))
            {
                value = (T)tmp;
                return true;
            }

            value = default;
            return false;
        }
    }
}
