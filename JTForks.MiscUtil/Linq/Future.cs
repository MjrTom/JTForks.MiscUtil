// <copyright file="Future.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Poor-man's version of a Future. This wraps a result which *will* be
    /// available in the future. It's up to the caller/provider to make sure
    /// that the value has been specified by the time it's requested.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Future<T> : IFuture<T>
    {
        private T value = default!;
        private bool valueSet;
        /// <summary>
        /// Returns the value of the future, once it has been set
        /// </summary>
        /// <exception cref="InvalidOperationException">If the value is not yet available</exception>
        public T Value
        {
            get
            {
                return !this.valueSet ? throw new InvalidOperationException("No value has been set yet") : this.value;
            }
            set
            {
                if (this.valueSet)
                {
                    throw new InvalidOperationException("Value has already been set");
                }

                this.valueSet = true;
                this.value = value;
            }
        }

        /// <summary>
        /// Returns a string representation of the value if available, null otherwise
        /// </summary>
        /// <returns>A string representation of the value if available, null otherwise</returns>
        public override string ToString()
        {
            return this.valueSet ? Convert.ToString(this.value, CultureInfo.InvariantCulture) ?? string.Empty : "(unavailable)";
        }
    }
}
