// <copyright file="FutureProxy.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System;

    /// <summary>
    /// Provides a proxy for a future value, allowing for transformations.
    /// </summary>
    public static class FutureProxy
    {
        /// <summary>
        /// Creates a new FutureProxy from an existing future using
        /// the supplied transformation to obtain the value as needed.
        /// </summary>
        /// <typeparam name="TSource">The type of the source future.</typeparam>
        /// <typeparam name="TResult">The type of the result future.</typeparam>
        /// <param name="future">The source future.</param>
        /// <param name="projection">The transformation to apply to the source future's value.</param>
        /// <returns>A new FutureProxy with the transformed value.</returns>
        public static FutureProxy<TResult> FromFuture<TSource, TResult>(IFuture<TSource> future, Func<TSource, TResult> projection)
        {
            return new FutureProxy<TResult>(() => projection(future.Value));
        }
    }

    /// <summary>
    /// Implementation of IFuture which retrieves its value from a delegate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="fetcher">The delegate to fetch the value.</param>
    /// <remarks>
    /// Creates a new FutureProxy using the given method
    /// to obtain the value when needed.
    /// </remarks>
    public class FutureProxy<T>(Func<T> fetcher) : IFuture<T>
    {
        private readonly Func<T> fetcher = fetcher;

        /// <summary>
        /// Gets the value of the Future.
        /// </summary>
        public T Value => this.fetcher();
    }
}
