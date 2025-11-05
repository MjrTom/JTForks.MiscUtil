// <copyright file="IFuture.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    /// <summary>
    /// Class representing a value which will be available some time in the future.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFuture<T>
    {
        /// <summary>
        /// Retrieves the value, if available, and throws InvalidOperationException
        /// otherwise.
        /// </summary>
        T Value { get; }
    }
}
