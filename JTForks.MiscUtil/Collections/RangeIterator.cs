// <copyright file="RangeIterator.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using MiscUtil.Collections.Extensions;
    using MiscUtil.Extensions;

    /// <summary>
    /// Iterates over a range. Despite its name, this implements IEnumerable{T} rather than
    /// IEnumerator{T} - it just sounds better, frankly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeIterator<T> : IEnumerable<T>
    {
        /// <summary>
        /// Returns the range this object iterates over
        /// </summary>
        public Range<T> Range { get; }

        /// <summary>
        /// Returns the step function used for this range
        /// </summary>
        public Func<T, T> Step { get; }

        /// <summary>
        /// Returns whether or not this iterator works up from the start point (ascending)
        /// or down from the end point (descending)
        /// </summary>
        public bool Ascending { get; }

        /// <summary>
        /// Creates an ascending iterator over the given range with the given step function
        /// </summary>
        /// <param name="range"></param>
        /// <param name="step"></param>
        public RangeIterator(Range<T> range, Func<T, T> step)
            : this(range, step, true)
        {
        }

        /// <summary>
        /// Creates an iterator over the given range with the given step function,
        /// with the specified direction.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="step"></param>
        /// <param name="ascending"></param>
        public RangeIterator(Range<T> range, Func<T, T> step, bool ascending)
        {
            step.ThrowIfNull("step");

            if ((ascending && range.Comparer.Compare(range.Start, step(range.Start)) >= 0) ||
                (!ascending && range.Comparer.Compare(range.End, step(range.End)) <= 0))
            {
                throw new ArgumentException("step does nothing, or progresses the wrong way");
            }

            this.Ascending = ascending;
            this.Range = range;
            this.Step = step;
        }

        /// <summary>
        /// Returns an IEnumerator{T} running over the range.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            // A descending range effectively has the start and end points (and inclusions)
            // reversed, and a reverse comparer.
            var includesStart = this.Ascending ? this.Range.IncludesStart : this.Range.IncludesEnd;
            var includesEnd = this.Ascending ? this.Range.IncludesEnd : this.Range.IncludesStart;
            T start = this.Ascending ? this.Range.Start : this.Range.End;
            T end = this.Ascending ? this.Range.End : this.Range.Start;
            IComparer<T> comparer = this.Ascending ? this.Range.Comparer : this.Range.Comparer.Reverse();

            // Now we can use our local version of the range variables to iterate

            T value = start;

            if (includesStart)
            {
                // Deal with possibility that start point = end point
                if (includesEnd || comparer.Compare(value, end) < 0)
                {
                    yield return value;
                }
            }

            value = this.Step(value);

            while (comparer.Compare(value, end) < 0)
            {
                yield return value;
                value = this.Step(value);
            }

            // We've already performed a step, therefore we can't
            // still be at the start point
            if (includesEnd && comparer.Compare(value, end) == 0)
            {
                yield return value;
            }
        }

        /// <summary>
        /// Returns an IEnumerator running over the range.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
