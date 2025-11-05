// <copyright file="EventArgs.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    /// <summary>
    /// Generic event argument taking a single value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// Creates a new EventArgs&lt;T&gt; with the specified value.
    /// </remarks>
    /// <param name="value">The Value of the EventArgs&lt;T&gt; instance.</param>
    public class EventArgs<T>(T value) : System.EventArgs
    {
        /// <summary>
        /// The typed value of the EventArgs&lt;T&gt;
        /// </summary>
        public T Value { get; } = value;
    }
}
