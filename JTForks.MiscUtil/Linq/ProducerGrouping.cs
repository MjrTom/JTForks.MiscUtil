// <copyright file="ProducerGrouping.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System;

    /// <summary>
    /// Simple implementation of IProducerGrouping which proxies to an existing
    /// IDataProducer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <param name="key"></param>
    /// <param name="source"></param>
    /// <remarks>
    /// Constructs a new grouping with the given key
    /// </remarks>
    public class ProducerGrouping<TKey, TElement>(TKey key, IDataProducer<TElement> source) : IProducerGrouping<TKey, TElement>
    {
        private readonly IDataProducer<TElement> source = source;

        /// <summary>
        /// Event which is raised when an item of data is produced.
        /// This will not be raised after EndOfData has been raised.
        /// The parameter for the event is the 
        /// </summary>
        /// <seealso cref="MiscUtil.Linq.IDataProducer&lt;T&gt;.DataProduced"/>
        public event Action<TElement> DataProduced
        {
            add { this.source.DataProduced += value; }
            remove { this.source.DataProduced -= value; }
        }

        /// <summary>
        /// Event which is raised when the sequence has finished being
        /// produced. This will be raised exactly once, and after all
        /// DataProduced events (if any) have been raised.
        /// </summary>
        /// <seealso cref="MiscUtil.Linq.IDataProducer&lt;T&gt;.EndOfData"/>
        public event Action EndOfData
        {
            add { this.source.EndOfData += value; }
            remove { this.source.EndOfData -= value; }
        }

        /// <summary>
        /// The key for this grouping.
        /// </summary>
        public TKey Key { get; } = key;
    }
}
