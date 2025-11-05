// <copyright file="OrderedDataProducer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Linq
{
    using System;
    using System.Collections.Generic;
    using MiscUtil.Extensions;

    /// <summary>
    /// A DataProducer with ordering capabilities.
    /// </summary>
    /// <remarks>Note that this may cause data to be buffered.</remarks>
    /// <typeparam name="T"></typeparam>
    internal class OrderedDataProducer<T> : IOrderedDataProducer<T>
    {
        private bool dataHasEnded;
        private List<T>? buffer;

        public IDataProducer<T> BaseProducer { get; }

        public IComparer<T> Comparer { get; }

        public event Action<T>? DataProduced;
        public event Action? EndOfData;

        /// <summary>
        /// Create a new OrderedDataProducer.
        /// </summary>
        /// <param name="baseProducer">The base source which will supply data.</param>
        /// <param name="comparer">The comparer to use when sorting the data (once complete).</param>
        public OrderedDataProducer(
            IDataProducer<T> baseProducer,
            IComparer<T> comparer)
        {
            baseProducer.ThrowIfNull("baseProducer");

            this.BaseProducer = baseProducer;
            this.Comparer = comparer ?? Comparer<T>.Default;

            baseProducer.DataProduced += new Action<T>(this.OriginalDataProduced);
            baseProducer.EndOfData += new Action(this.EndOfOriginalData);
        }

        private void OriginalDataProduced(T item)
        {
            if (this.dataHasEnded)
            {
                throw new InvalidOperationException("EndOfData already occurred");
            }

            if (this.DataProduced != null)
            { // only get excited if somebody is listening
                this.buffer ??= [];

                this.buffer.Add(item);
            }
        }

        private void EndOfOriginalData()
        {
            if (this.dataHasEnded)
            {
                throw new InvalidOperationException("EndOfData already occurred");
            }

            this.dataHasEnded = true;
            // only do the sort if somebody is still listening
            if (this.DataProduced != null && this.buffer != null)
            {
                this.buffer.Sort(this.Comparer);
                foreach (T item in this.buffer)
                {
                    this.OnDataProduced(item);
                }
            }

            this.buffer = null!;
            this.OnEndOfData();
        }

        private void OnEndOfData()
        {
            this.EndOfData?.Invoke();
        }

        private void OnDataProduced(T item)
        {
            this.DataProduced?.Invoke(item);
        }
    }
}
