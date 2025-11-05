// <copyright file="NonNullable.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    using System;

    /// <summary>
    /// Encapsulates a reference compatible with the type parameter. The reference
    /// is guaranteed to be non-null unless the value has been created with the
    /// parameterless constructor (e.g. as the default value of a field or array).
    /// Implicit conversions are available to and from the type parameter. The
    /// conversion to the non-nullable type will throw ArgumentNullException
    /// when presented with a null reference. The conversion from the non-nullable
    /// type will throw NullReferenceException if it contains a null reference.
    /// This type is a value type (to avoid taking any extra space) and as the CLR
    /// unfortunately has no knowledge of it, it will be boxed as any other value
    /// type. The conversions are also available through the Value property and the
    /// parameterized constructor.
    /// </summary>
    /// <typeparam name="T">Type of non-nullable reference to encapsulate</typeparam>
    /// <param name="value"></param>
    /// <remarks>
    /// Creates a non-nullable value encapsulating the specified reference.
    /// </remarks>
    public readonly struct NonNullable<T>(T value) : IEquatable<NonNullable<T>>
        where T : class
    {
        private readonly T value = value ?? throw new ArgumentNullException("value");

        /// <summary>
        /// Retrieves the encapsulated value, or throws a NullReferenceException if
        /// this instance was created with the parameterless constructor or by default.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public readonly T Value => this.value ?? throw new InvalidOperationException();

        /// <summary>
        /// Implicit conversion from the specified reference.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator NonNullable<T>(T value)
        {
            return new NonNullable<T>(value);
        }

        /// <summary>
        /// Implicit conversion to the type parameter from the encapsulated value.
        /// </summary>
        /// <param name="wrapper"></param>
        public static implicit operator T(NonNullable<T> wrapper)
        {
            return wrapper.Value;
        }

        /// <summary>
        /// Equality operator, which performs an identity comparison on the encapsulated
        /// references. No exception is thrown even if the references are null.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static bool operator ==(NonNullable<T> first, NonNullable<T> second)
        {
            return first.value == second.value;
        }

        /// <summary>
        /// Inequality operator, which performs an identity comparison on the encapsulated
        /// references. No exception is thrown even if the references are null.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        public static bool operator !=(NonNullable<T> first, NonNullable<T> second)
        {
            return first.value != second.value;
        }

        /// <summary>
        /// Equality is deferred to encapsulated references, but there is no equality
        /// between a NonNullable[T] and a T. This method never throws an exception,
        /// even if a null reference is encapsulated.
        /// </summary>
        /// <param name="obj"></param>
        public override bool Equals(object? obj)
        {
            return obj is NonNullable<T> && this.Equals((NonNullable<T>)obj);
        }

        /// <summary>
        /// Type-safe (and non-boxing) equality check.
        /// </summary>
        /// <param name="other"></param>
        public readonly bool Equals(NonNullable<T> other)
        {
            return object.Equals(this.value, other.value);
        }

        /// <summary>
        /// Defers to the GetHashCode implementation of the encapsulated reference, or 0 if
        /// the reference is null.
        /// </summary>
        public override readonly int GetHashCode()
        {
            return this.value == null ? 0 : this.value.GetHashCode();
        }

        /// <summary>
        /// Defers to the ToString implementation of the encapsulated reference, or an
        /// empty string if the reference is null.
        /// </summary>
        public override readonly string ToString()
        {
            return this.value?.ToString() ?? "";
        }
    }
}
