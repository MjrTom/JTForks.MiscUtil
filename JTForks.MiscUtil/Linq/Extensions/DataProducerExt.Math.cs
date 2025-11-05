// <copyright file="DataProducerExt.Math.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq.Extensions
{
    using System;
    using System.Numerics;

    public static partial class DataProducerExt
    {
        /// <summary>
        /// Returns a future to the sum of a sequence of values that are
        /// obtained by taking a transform of the input sequence
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <remarks>Null values are removed from the sum</remarks>
        public static IFuture<TResult> Sum<TSource, TResult>(this IDataProducer<TSource> source, Func<TSource, TResult> selector)
            where TResult : INumber<TResult>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            var ret = new Future<TResult>();
            TResult sum = TResult.Zero;
            source.DataProduced += item => sum += selector(item);
            source.EndOfData += () => ret.Value = sum;
            return ret;
        }
        /// <summary>
        /// Returns a future to the sum of a sequence of values
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <remarks>Null values are removed from the sum</remarks>
        public static IFuture<TSource> Sum<TSource>(this IDataProducer<TSource> source)
            where TSource : INumber<TSource>
        {
            return Sum<TSource, TSource>(source, x => x);
        }
        /// <summary>
        /// Returns a future to the average of a sequence of values that are
        /// obtained by taking a transform of the input sequence
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <remarks>Null values are removed from the average</remarks>
        public static IFuture<TResult> Average<TSource, TResult>(this IDataProducer<TSource> source, Func<TSource, TResult> selector)
            where TResult : IFloatingPoint<TResult>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            var ret = new Future<TResult>();
            TResult sum = TResult.Zero;
            var count = 0;
            source.DataProduced += item =>
            {
                sum += selector(item);
                count++;
            };
            source.EndOfData += () =>
            {
                ret.Value = count == 0 ? TResult.Zero : sum / TResult.CreateChecked(count);
            };
            return ret;
        }

        /// <summary>
        /// Returns a future to the average of a sequence of values
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <remarks>Null values are removed from the average</remarks>
        public static IFuture<TSource> Average<TSource>(this IDataProducer<TSource> source) where TSource : IFloatingPoint<TSource>
        {
            return Average<TSource, TSource>(source, x => x);
        }

        /// <summary>
        /// Returns a future to the maximum of a sequence of values that are
        /// obtained by taking a transform of the input sequence, using the default comparer, using the default comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <remarks>Null values are removed from the maximum</remarks>
        public static IFuture<TResult?> Max<TSource, TResult>
            (this IDataProducer<TSource> source,
             Func<TSource, TResult> selector) where TResult : IComparisonOperators<TResult, TResult, bool>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source.Select(selector).Max();
        }

        /// <summary>
        /// Returns a future to the maximum of a sequence of values, using the default comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <remarks>Null values are removed from the maximum</remarks>
        public static IFuture<TSource?> Max<TSource>(this IDataProducer<TSource> source)
            where TSource : IComparisonOperators<TSource, TSource, bool>
        {
            ArgumentNullException.ThrowIfNull(source);

            var ret = new Future<TSource?>();
            TSource? max = default;

            source.DataProduced += item =>
            {
                if (max is null || item > max)
                {
                    max = item;
                }
            };
            source.EndOfData += () => ret.Value = max;

            return ret;
        }

        /// <summary>
        /// Returns a future to the minimum of a sequence of values that are
        /// obtained by taking a transform of the input sequence, using the default comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <remarks>Null values are removed from the minimum</remarks>
        public static IFuture<TResult?> Min<TSource, TResult>
           (this IDataProducer<TSource> source,
            Func<TSource, TResult> selector)
            where TResult : IComparisonOperators<TResult, TResult, bool>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source.Select(selector).Min();
        }

        /// <summary>
        /// Returns a future to the minimum of a sequence of values, using the default comparer
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <remarks>Null values are removed from the minimum</remarks>
        public static IFuture<TSource?> Min<TSource>(this IDataProducer<TSource> source)
            where TSource : IComparisonOperators<TSource, TSource, bool>
        {
            ArgumentNullException.ThrowIfNull(source);

            var ret = new Future<TSource?>();
            TSource? min = default;

            source.DataProduced += item =>
            {
                if (min is null || item < min)
                {
                    min = item;
                }
            };
            source.EndOfData += () => ret.Value = min;

            return ret;
        }

    }
}
