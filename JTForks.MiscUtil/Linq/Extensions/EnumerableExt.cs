// <copyright file="EnumerableExt.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    /// Further extensions to IEnumerable{T}.
    /// </summary>
    public static class EnumerableExt
    {
        /// <summary>
        /// Groups and executes a pipeline for a single result per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult>> GroupWithPipeline<TElement, TKey, TResult>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             Func<IDataProducer<TElement>, IFuture<TResult>> pipeline)
            where TKey : notnull
        {
            return source.GroupWithPipeline(keySelector, EqualityComparer<TKey>.Default, pipeline);
        }

        /// <summary>
        /// Groups and executes a pipeline for a single result per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult>> GroupWithPipeline<TElement, TKey, TResult>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             IEqualityComparer<TKey> comparer,
             Func<IDataProducer<TElement>, IFuture<TResult>> pipeline)
            where TKey : notnull
        {
            var keyMap = new Dictionary<TKey, DataProducer<TElement>>(comparer);
            var results = new List<KeyValueTuple<TKey, IFuture<TResult>>>();

            foreach (TElement element in source)
            {
                TKey key = keySelector(element);
                if (!keyMap.TryGetValue(key, out DataProducer<TElement>? producer) || producer is null)
                {
                    producer = new DataProducer<TElement>();
                    keyMap[key] = producer;
                    results.Add(new KeyValueTuple<TKey, IFuture<TResult>>(key, pipeline(producer)));
                }
                producer.Produce(element);
            }

            foreach (DataProducer<TElement> producer in keyMap.Values)
            {
                producer.End();
            }

            foreach (KeyValueTuple<TKey, IFuture<TResult>> result in results)
            {
                yield return new KeyValueTuple<TKey, TResult>(result.Key, result.Value.Value);
            }
        }

        /// <summary>
        /// Groups and executes a pipeline for two results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2>> GroupWithPipeline<TElement, TKey, TResult1, TResult2>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2)
            where TKey : notnull
        {
            return source.GroupWithPipeline(keySelector, EqualityComparer<TKey>.Default, pipeline1, pipeline2);
        }

        /// <summary>
        /// Groups and executes a pipeline for two results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2>> GroupWithPipeline<TElement, TKey, TResult1, TResult2>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             IEqualityComparer<TKey> comparer,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2)
            where TKey : notnull
        {
            var keyMap = new Dictionary<TKey, DataProducer<TElement>>(comparer);
            var results = new List<KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>>>();

            foreach (TElement element in source)
            {
                TKey key = keySelector(element);
                if (!keyMap.TryGetValue(key, out DataProducer<TElement>? producer) || producer is null)
                {
                    producer = new DataProducer<TElement>();
                    keyMap[key] = producer;
                    results.Add(new KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>>(key, pipeline1(producer), pipeline2(producer)));
                }
                producer.Produce(element);
            }

            foreach (DataProducer<TElement> producer in keyMap.Values)
            {
                producer.End();
            }

            foreach (KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>> result in results)
            {
                yield return new KeyValueTuple<TKey, TResult1, TResult2>(result.Key, result.Value1.Value, result.Value2.Value);
            }
        }

        /// <summary>
        /// Groups and executes a pipeline for three results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <typeparam name="TResult3"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2, TResult3>> GroupWithPipeline<TElement, TKey, TResult1, TResult2, TResult3>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2,
             Func<IDataProducer<TElement>, IFuture<TResult3>> pipeline3)
            where TKey : notnull
        {
            return source.GroupWithPipeline(keySelector, EqualityComparer<TKey>.Default, pipeline1, pipeline2, pipeline3);
        }


        /// <summary>
        /// Groups and executes a pipeline for three results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <typeparam name="TResult3"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2, TResult3>> GroupWithPipeline<TElement, TKey, TResult1, TResult2, TResult3>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             IEqualityComparer<TKey> comparer,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2,
             Func<IDataProducer<TElement>, IFuture<TResult3>> pipeline3)
            where TKey : notnull
        {
            var keyMap = new Dictionary<TKey, DataProducer<TElement>>(comparer);
            var results = new List<KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>>>();

            foreach (TElement element in source)
            {
                TKey key = keySelector(element);
                if (!keyMap.TryGetValue(key, out DataProducer<TElement>? producer) || producer is null)
                {
                    producer = new DataProducer<TElement>();
                    keyMap[key] = producer;
                    results.Add(new KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>>(key, pipeline1(producer), pipeline2(producer), pipeline3(producer)));
                }
                producer.Produce(element);
            }

            foreach (DataProducer<TElement> producer in keyMap.Values)
            {
                producer.End();
            }

            foreach (KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>> result in results)
            {
                yield return new KeyValueTuple<TKey, TResult1, TResult2, TResult3>(result.Key, result.Value1.Value, result.Value2.Value, result.Value3.Value);
            }
        }

        /// <summary>
        /// Groups and executes a pipeline for four results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <typeparam name="TResult3"></typeparam>
        /// <typeparam name="TResult4"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2, TResult3, TResult4>> GroupWithPipeline<TElement, TKey, TResult1, TResult2, TResult3, TResult4>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2,
             Func<IDataProducer<TElement>, IFuture<TResult3>> pipeline3,
             Func<IDataProducer<TElement>, IFuture<TResult4>> pipeline4)
            where TKey : notnull
        {
            return source.GroupWithPipeline(keySelector, EqualityComparer<TKey>.Default, pipeline1, pipeline2, pipeline3, pipeline4);
        }

        /// <summary>
        /// Groups and executes a pipeline for four results per group
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <typeparam name="TResult3"></typeparam>
        /// <typeparam name="TResult4"></typeparam>
        public static IEnumerable<KeyValueTuple<TKey, TResult1, TResult2, TResult3, TResult4>> GroupWithPipeline<TElement, TKey, TResult1, TResult2, TResult3, TResult4>
            (this IEnumerable<TElement> source,
             Func<TElement, TKey> keySelector,
             IEqualityComparer<TKey> comparer,
             Func<IDataProducer<TElement>, IFuture<TResult1>> pipeline1,
             Func<IDataProducer<TElement>, IFuture<TResult2>> pipeline2,
             Func<IDataProducer<TElement>, IFuture<TResult3>> pipeline3,
             Func<IDataProducer<TElement>, IFuture<TResult4>> pipeline4)
            where TKey : notnull
        {
            var keyMap = new Dictionary<TKey, DataProducer<TElement>>(comparer);
            var results = new List<KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>, IFuture<TResult4>>>();

            foreach (TElement element in source)
            {
                TKey key = keySelector(element);
                if (!keyMap.TryGetValue(key, out DataProducer<TElement>? producer) || producer is null)
                {
                    producer = new DataProducer<TElement>();
                    keyMap[key] = producer;
                    results.Add(new KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>, IFuture<TResult4>>(key, pipeline1(producer), pipeline2(producer), pipeline3(producer), pipeline4(producer)));
                }
                producer.Produce(element);
            }

            foreach (DataProducer<TElement> producer in keyMap.Values)
            {
                producer.End();
            }

            foreach (KeyValueTuple<TKey, IFuture<TResult1>, IFuture<TResult2>, IFuture<TResult3>, IFuture<TResult4>> result in results)
            {
                yield return new KeyValueTuple<TKey, TResult1, TResult2, TResult3, TResult4>(result.Key, result.Value1.Value, result.Value2.Value, result.Value3.Value, result.Value4.Value);
            }
        }
        /// <summary>
        /// Computes the sum of a sequence of values
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <remarks>The values in the sequence must support the Add operator</remarks>
        public static TSource Sum<TSource>(this IEnumerable<TSource> source)
            where TSource : INumber<TSource>
        {
            return Sum(source, x => x);
        }
        /// <summary>
        /// Computes the sum of the sequence of values that are
        /// obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <remarks>The values returned by the transform function must support the Add operator</remarks>
        public static TValue Sum<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
            where TValue : INumber<TValue>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            TValue sum = TValue.Zero;
            foreach (TSource item in source)
            {
                sum += selector(item);
            }
            return sum;
        }

        /// <summary>
        /// Computes the mean average of a sequence of values
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <remarks>The values in the sequence must support the Add and Divide(Int32) operators</remarks>
        public static TSource Average<TSource>(this IEnumerable<TSource> source)
            where TSource : IFloatingPoint<TSource>
        {
            return Average(source, x => x);
        }
        /// <summary>
        /// Computes the mean average of the sequence of values that are
        /// obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <remarks>The values returned by the transform function must support the Add and Divide(Int32) operators</remarks>
        public static TValue Average<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
            where TValue : IFloatingPoint<TValue>
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);
            var count = 0;
            TValue sum = TValue.Zero;
            foreach (TSource item in source)
            {
                sum += selector(item);
                count++;
            }
            return count == 0 ? TValue.Zero : sum / TValue.CreateChecked(count);
        }
        /// <summary>
        /// Computes the maximum (using a custom comparer) of a sequence of values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        public static TSource? Max<TSource>(this IEnumerable<TSource> source)
            where TSource : IComparisonOperators<TSource, TSource, bool>
        {
            TSource? max = default;
            foreach (TSource item in source)
            {
                if (max is null || item > max)
                {
                    max = item;
                }
            }
            return max;
        }
        /// <summary>
        /// Computes the maximum (using a custom comparer) of the sequence of values that are
        /// obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public static TValue? Max<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
            where TValue : IComparisonOperators<TValue, TValue, bool>
        {
            return source.Select(selector).Max();
        }
        /// <summary>
        /// Computes the minimum (using a custom comparer) of a sequence of values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        public static TSource? Min<TSource>(this IEnumerable<TSource> source)
            where TSource : IComparisonOperators<TSource, TSource, bool>
        {
            TSource? min = default;
            foreach (TSource item in source)
            {
                if (min is null || item < min)
                {
                    min = item;
                }
            }
            return min;
        }
        /// <summary>
        /// Computes the minimum (using a custom comparer) of the sequence of values that are
        /// obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        public static TValue? Min<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector)
            where TValue : IComparisonOperators<TValue, TValue, bool>
        {
            return source.Select(selector).Min();
        }
    }
}
