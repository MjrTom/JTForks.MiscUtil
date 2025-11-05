// <copyright file="DataProducerExt.Pipeline.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq.Extensions
{
    using System;
    using System.Collections.Generic;
    using MiscUtil.Collections;
    using MiscUtil.Collections.Extensions;

    public static partial class DataProducerExt
    {

        /// <summary>
        /// Filters a data-producer based on a predicate on each value
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The data-producer to be filtered</param>
        /// <param name="predicate">The condition to be satisfied</param>
        /// <returns>A filtered data-producer; only matching values will raise the DataProduced event</returns>
        public static IDataProducer<TSource> Where<TSource>(this IDataProducer<TSource> source,
            Func<TSource, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return source.Where((x, index) => predicate(x));
        }

        /// <summary>
        /// Filters a data-producer based on a predicate on each value; the index
        /// in the sequence is used in the predicate
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The data-producer to be filtered</param>
        /// <param name="predicate">The condition to be satisfied</param>
        /// <returns>A filtered data-producer; only matching values will raise the DataProduced event</returns>
        public static IDataProducer<TSource> Where<TSource>(this IDataProducer<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            DataProducer<TSource> ret = new();

            var index = 0;

            source.DataProduced += value =>
            {
                if (predicate(value, index++))
                {
                    ret.Produce(value);
                }
            };
            source.EndOfData += ret.End;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that yields the values from the sequence, or which yields the given
        /// singleton value if no data is produced.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="defaultValue">The default value to be yielded if no data is produced.</param>
        /// <param name="source">The source data-producer.</param>
        public static IDataProducer<TSource> DefaultIfEmpty<TSource>(this IDataProducer<TSource> source, TSource defaultValue)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataProducer<TSource> ret = new();

            var empty = true;
            source.DataProduced += value =>
            {
                empty = false;
                ret.Produce(value);
            };
            source.EndOfData += () =>
            {
                if (empty)
                {
                    ret.Produce(defaultValue);
                }

                ret.End();
            };
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that yields the values from the sequence, or which yields the default
        /// value for the Type if no data is produced.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The source data-producer.</param>
        public static IDataProducer<TSource?> DefaultIfEmpty<TSource>(this IDataProducer<TSource?> source)
        {
            return source.DefaultIfEmpty(default);
        }

        /// <summary>
        /// Returns a projection on the data-producer, using a transformation to
        /// map each element into a new form.
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TResult">The projected type</typeparam>
        /// <param name="source">The source data-producer</param>
        /// <param name="projection">The transformation to apply to each element.</param>
        public static IDataProducer<TResult> Select<TSource, TResult>(this IDataProducer<TSource> source,
                                                               Func<TSource, TResult> projection)
        {
            ArgumentNullException.ThrowIfNull(projection);
            return source.Select((t, index) => projection(t));
        }

        /// <summary>
        /// Returns a projection on the data-producer, using a transformation
        /// (involving the elements's index in the sequence) to
        /// map each element into a new form.
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TResult">The projected type</typeparam>
        /// <param name="source">The source data-producer</param>
        /// <param name="projection">The transformation to apply to each element.</param>
        public static IDataProducer<TResult> Select<TSource, TResult>(this IDataProducer<TSource> source,
                                                                      Func<TSource, int, TResult> projection)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(projection);

            DataProducer<TResult> ret = new();
            var index = 0;
            source.DataProduced += value => ret.Produce(projection(value, index++));
            source.EndOfData += ret.End;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that will yield a specified number of
        /// contiguous elements from the start of a sequence - i.e.
        /// "the first &lt;x&gt; elements".
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="count">The maximum number of elements to return.</param>
        /// <returns>A data-producer that will yield a specified number of
        /// contiguous elements from the start of a sequence - i.e.
        /// "the first &lt;x&gt; elements".</returns>
        public static IDataProducer<TSource> Take<TSource>(this IDataProducer<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataProducer<TSource> ret = new();

            Action completion = ret.End;
            void production(TSource value)
            {
                if (count > 0)
                {
                    ret.Produce(value);
                    count--;
                }

                if (count <= 0)
                {
                    source.EndOfData -= completion;
                    source.DataProduced -= production;
                    ret.End();
                }
            }

            source.DataProduced += production;
            source.EndOfData += completion;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that will ignore a specified number of
        /// contiguous elements from the start of a sequence, and yield
        /// all elements after this point - i.e.
        /// "elements from index &lt;x&gt; onwards".
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="count">The number of elements to ignore.</param>
        /// <returns>A data-producer that will yield all elements after this point.</returns>
        public static IDataProducer<TSource> Skip<TSource>(this IDataProducer<TSource> source, int count)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataProducer<TSource> ret = new();
            source.DataProduced += value =>
            {
                if (count > 0)
                {
                    count--;
                }
                else
                {
                    ret.Produce(value);
                }
            };
            source.EndOfData += ret.End;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that will yield
        /// elements a sequence as long as a condition
        /// is satisfied; when the condition fails for an element,
        /// that element and all subsequent elements are ignored.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="predicate">The condition to yield elements.</param>
        /// <returns>A data-producer that will yield elements as long as the condition is satisfied.</returns>
        public static IDataProducer<TSource> TakeWhile<TSource>(this IDataProducer<TSource> source, Func<TSource, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return source.TakeWhile((x, index) => predicate(x));
        }

        /// <summary>
        /// Returns a data-producer that will yield
        /// elements a sequence as long as a condition
        /// (involving the element's index in the sequence)
        /// is satisfied; when the condition fails for an element,
        /// that element and all subsequent elements are ignored.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="predicate">The condition to yield elements.</param>
        /// <returns>A data-producer that will yield elements as long as the condition is satisfied.</returns>
        public static IDataProducer<TSource> TakeWhile<TSource>(this IDataProducer<TSource> source, Func<TSource, int, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            DataProducer<TSource> ret = new();
            Action completion = ret.End;
            Action<TSource>? production = null;

            var index = 0;

            production = value =>
            {
                if (!predicate(value, index++))
                {
                    ret.End();
                    source.DataProduced -= production;
                    source.EndOfData -= completion;
                }
                else
                {
                    ret.Produce(value);
                }
            };

            source.DataProduced += production;
            source.EndOfData += completion;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that will ignore the
        /// elements from the start of a sequence while a condition
        /// is satisfied; when the condition fails for an element,
        /// that element and all subsequent elements are yielded.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="predicate">The condition to skip elements.</param>
        /// <returns>A data-producer that will yield elements after the condition fails.</returns>
        public static IDataProducer<TSource> SkipWhile<TSource>(this IDataProducer<TSource> source, Func<TSource, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            return source.SkipWhile((t, index) => predicate(t));
        }

        /// <summary>
        /// Returns a data-producer that will ignore the
        /// elements from the start of a sequence while a condition
        /// (involving the elements's index in the sequence)
        /// is satisfied; when the condition fails for an element,
        /// that element and all subsequent elements are yielded.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <param name="predicate">The condition to skip elements.</param>
        /// <returns>A data-producer that will yield elements after the condition fails.</returns>
        public static IDataProducer<TSource> SkipWhile<TSource>(this IDataProducer<TSource> source, Func<TSource, int, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            DataProducer<TSource> ret = new();
            Action completion = ret.End;

            var skipping = true;
            var index = 0;
            source.DataProduced += value =>
            {
                if (skipping)
                {
                    skipping = predicate(value, index++);
                }

                // Note - not an else clause!
                if (!skipping)
                {
                    ret.Produce(value);
                }
            };
            source.EndOfData += completion;
            return ret;
        }

        /// <summary>
        /// Returns a data-producer that yields the first instance of each unique
        /// value in the sequence; subsequent identical values are ignored.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The source data-producer.</param>
        /// <remarks>This will force the first instance of each unique value to be buffered.</remarks>
        /// <returns>A data-producer that yields the first instance of each unique value.</returns>
        public static IDataProducer<TSource> Distinct<TSource>(this IDataProducer<TSource> source)
        {
            return source.Distinct(EqualityComparer<TSource>.Default);
        }

        /// <summary>
        /// Returns a data-producer that yields the first instance of each unique
        /// value in the sequence; subsequent identical values are ignored.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The data-producer</param>
        /// <param name="comparer">Used to determine equality between values</param>
        /// <remarks>This will force the first instance of each unique value to be buffered</remarks>
        /// <returns>A data-producer that yields the first instance of each unique value.</returns>
        public static IDataProducer<TSource> Distinct<TSource>(this IDataProducer<TSource> source, IEqualityComparer<TSource> comparer)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(comparer);

            var ret = new DataProducer<TSource>();

            var set = new HashSet<TSource>(comparer);

            source.DataProduced += value =>
            {
                if (set.Add(value))
                {
                    ret.Produce(value);
                }
            };
            source.EndOfData += ret.End;
            return ret;
        }

        /// <summary>
        /// Reverses the order of a sequence
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source">The data-producer.</param>
        /// <returns>A data-producer that yields the sequence
        /// in the reverse order.</returns>
        /// <remarks>This will force all data to be buffered.</remarks>
        public static IDataProducer<TSource> Reverse<TSource>(this IDataProducer<TSource> source)
        {
            ArgumentNullException.ThrowIfNull(source);

            DataProducer<TSource> ret = new();

            // use List (rather than ToList) so we have a List<T> with
            // Reverse immediately available (more efficient, and 2.0 compatible)
            List<TSource> results = [];
            source.DataProduced += results.Add;
            source.EndOfData += () =>
            {
                List<TSource> items = new(results);
                items.Reverse();
                ret.ProduceAndEnd(items);
            };

            return ret;
        }

        /// <summary>
        /// Further orders the values from an ordered data-source by a transform on each term, ascending
        /// (the sort operation is only applied once for the combined ordering)
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer and ordering.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered</remarks>
        public static IOrderedDataProducer<TSource> ThenBy<TSource, TKey>(this IOrderedDataProducer<TSource> source, Func<TSource, TKey> selector)
        {
            return ThenBy(source, selector, Comparer<TKey>.Default, false);
        }

        /// <summary>
        /// Further orders the values from an ordered data-source by a transform on each term, ascending
        /// (the sort operation is only applied once for the combined ordering)
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer and ordering.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <param name="comparer">Comparer to compare the selected values.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered.</remarks>
        public static IOrderedDataProducer<TSource> ThenBy<TSource, TKey>(this IOrderedDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return ThenBy(source, selector, comparer, false);
        }

        /// <summary>
        /// Further orders the values from an ordered data-source by a transform on each term, descending
        /// (the sort operation is only applied once for the combined ordering)
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer and ordering.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered</remarks>
        public static IOrderedDataProducer<TSource> ThenByDescending<TSource, TKey>(this IOrderedDataProducer<TSource> source, Func<TSource, TKey> selector)
        {
            return ThenBy(source, selector, Comparer<TKey>.Default, true);
        }

        /// <summary>
        /// Further orders the values from an ordered data-source by a transform on each term, descending
        /// (the sort operation is only applied once for the combined ordering)
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer and ordering.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <param name="comparer">Comparer to compare the selected values.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered.</remarks>
        public static IOrderedDataProducer<TSource> ThenByDescending<TSource, TKey>(this IOrderedDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return ThenBy(source, selector, comparer, true);
        }

        /// <summary>
        /// Orders the values from a data-source by a transform on each term, ascending
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered</remarks>
        public static IOrderedDataProducer<TSource> OrderBy<TSource, TKey>(this IDataProducer<TSource> source, Func<TSource, TKey> selector)
        {
            return OrderBy(source, selector, Comparer<TKey>.Default, false);
        }

        /// <summary>
        /// Orders the values from a data-source by a transform on each term, ascending
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <param name="comparer">Comparer to compare the selected values.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered.</remarks>
        public static IOrderedDataProducer<TSource> OrderBy<TSource, TKey>(this IDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return OrderBy(source, selector, comparer, false);
        }

        /// <summary>
        /// Orders the values from a data-source by a transform on each term, descending
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered</remarks>
        public static IOrderedDataProducer<TSource> OrderByDescending<TSource, TKey>(this IDataProducer<TSource> source, Func<TSource, TKey> selector)
        {
            return OrderBy(source, selector, Comparer<TKey>.Default, true);
        }

        /// <summary>
        /// Orders the values from a data-source by a transform on each term, descending
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <param name="source">The original data-producer.</param>
        /// <param name="selector">Returns the value (for each term) by which to order the sequence.</param>
        /// <param name="comparer">Comparer to compare the selected values.</param>
        /// <returns>A data-producer that yields the sequence ordered
        /// by the selected value.</returns>
        /// <remarks>This will force all data to be buffered.</remarks>
        public static IOrderedDataProducer<TSource> OrderByDescending<TSource, TKey>(this IDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            return OrderBy(source, selector, comparer, true);
        }

        private static OrderedDataProducer<TSource> OrderBy<TSource, TKey>(IDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, bool descending)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(comparer);

            IComparer<TSource> itemComparer = new ProjectionComparer<TSource, TKey>(selector, comparer);
            if (descending)
            {
                itemComparer = itemComparer.Reverse();
            }

            // first, discard any existing "order by"s by going back to the producer
            var first = true;
            while (source is IOrderedDataProducer<TSource> orderedProducer)
            {
                if (first)
                {
                    // keep the top-most comparer to enforce a balanced sort
                    itemComparer = new LinkedComparer<TSource>(itemComparer, orderedProducer.Comparer);
                    first = false;
                }

                source = orderedProducer.BaseProducer;
            }

            return new OrderedDataProducer<TSource>(source, itemComparer);
        }

        private static OrderedDataProducer<TSource> ThenBy<TSource, TKey>(IOrderedDataProducer<TSource> source, Func<TSource, TKey> selector, IComparer<TKey> comparer, bool descending)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            IComparer<TSource> itemComparer = new ProjectionComparer<TSource, TKey>(selector, comparer);
            if (descending)
            {
                itemComparer = itemComparer.Reverse();
            }

            itemComparer = new LinkedComparer<TSource>(source.Comparer, itemComparer);
            return new OrderedDataProducer<TSource>(source, itemComparer);
        }
    }
}
