// <copyright file="Range.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections.Generic;
    using MiscUtil.Extensions;

    /// <summary>
    /// Represents a range of values. An IComparer{T} is used to compare specific
    /// values with a start and end point. A range may be include or exclude each end
    /// individually.
    /// 
    /// A range which is half-open but has the same start and end point is deemed to be empty,
    /// e.g. [3,3) doesn't include 3. To create a range with a single value, use an inclusive
    /// range, e.g. [3,3].
    /// 
    /// Ranges are always immutable - calls such as IncludeEnd() and ExcludeEnd() return a new
    /// range without modifying this one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Range<T>
    {
        /// <summary>
        /// The start of the range.
        /// </summary>
        public T Start { get; }

        /// <summary>
        /// The end of the range.
        /// </summary>
        public T End { get; }

        /// <summary>
        /// Comparer to use for comparisons
        /// </summary>
        public IComparer<T> Comparer { get; }

        /// <summary>
        /// Whether or not this range includes the start point
        /// </summary>
        public bool IncludesStart { get; }

        /// <summary>
        /// Whether or not this range includes the end point
        /// </summary>
        public bool IncludesEnd { get; }

        /// <summary>
        /// Constructs a new inclusive range using the default comparer
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Range(T start, T end)
            : this(start, end, Comparer<T>.Default, true, true)
        {
        }

        /// <summary>
        /// Constructs a new range including both ends using the specified comparer
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="comparer"></param>
        public Range(T start, T end, IComparer<T> comparer)
            : this(start, end, comparer, true, true)
        {
        }

        /// <summary>
        /// Constructs a new range, including or excluding each end as specified,
        /// with the given comparer.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="comparer"></param>
        /// <param name="includeStart"></param>
        /// <param name="includeEnd"></param>
        public Range(T start, T end, IComparer<T> comparer, bool includeStart, bool includeEnd)
        {
            if (comparer.Compare(start, end) > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(end), "start must be lower than end according to comparer");
            }

            this.Start = start;
            this.End = end;
            this.Comparer = comparer;
            this.IncludesStart = includeStart;
            this.IncludesEnd = includeEnd;
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but excluding the end point.
        /// When called on a range already excluding the end point, the original range is returned.
        /// </summary>
        public Range<T> ExcludeEnd()
        {
            return !this.IncludesEnd ? this : new Range<T>(this.Start, this.End, this.Comparer, this.IncludesStart, false);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but excluding the start point.
        /// When called on a range already excluding the start point, the original range is returned.
        /// </summary>
        public Range<T> ExcludeStart()
        {
            return !this.IncludesStart ? this : new Range<T>(this.Start, this.End, this.Comparer, false, this.IncludesEnd);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but including the end point.
        /// When called on a range already including the end point, the original range is returned.
        /// </summary>
        public Range<T> IncludeEnd()
        {
            return this.IncludesEnd ? this : new Range<T>(this.Start, this.End, this.Comparer, this.IncludesStart, true);
        }

        /// <summary>
        /// Returns a range with the same boundaries as this, but including the start point.
        /// When called on a range already including the start point, the original range is returned.
        /// </summary>
        public Range<T> IncludeStart()
        {
            return this.IncludesStart ? this : new Range<T>(this.Start, this.End, this.Comparer, true, this.IncludesEnd);
        }

        /// <summary>
        /// Returns whether or not the range contains the given value
        /// </summary>
        /// <param name="value"></param>
        public bool Contains(T value)
        {
            var lowerBound = this.Comparer.Compare(value, this.Start);
            if (lowerBound < 0 || (lowerBound == 0 && !this.IncludesStart))
            {
                return false;
            }

            var upperBound = this.Comparer.Compare(value, this.End);
            return upperBound < 0 || (upperBound == 0 && this.IncludesEnd);
        }

        /// <summary>
        /// Returns an iterator which begins at the start of this range,
        /// applying the given step delegate on each iteration until the 
        /// end is reached or passed. The start and end points are included
        /// or excluded according to this range.
        /// </summary>
        /// <param name="step">Delegate to apply to the "current value" on each iteration</param>
        public RangeIterator<T> FromStart(Func<T, T> step)
        {
            return new RangeIterator<T>(this, step);
        }

        /// <summary>
        /// Returns an iterator which begins at the end of this range,
        /// applying the given step delegate on each iteration until the 
        /// start is reached or passed. The start and end points are included
        /// or excluded according to this range.
        /// </summary>
        /// <param name="step">Delegate to apply to the "current value" on each iteration</param>
        public RangeIterator<T> FromEnd(Func<T, T> step)
        {
            return new RangeIterator<T>(this, step, false);
        }

        /// <summary>
        /// Returns an iterator which steps through the range, applying the specified
        /// step delegate on each iteration. The method determines whether to begin 
        /// at the start or end of the range based on whether the step delegate appears to go
        /// "up" or "down". The step delegate is applied to the start point. If the result is 
        /// more than the start point, the returned iterator begins at the start point; otherwise
        /// it begins at the end point.
        /// </summary>
        /// <param name="step">Delegate to apply to the "current value" on each iteration</param>
        public RangeIterator<T> Step(Func<T, T> step)
        {
            step.ThrowIfNull("step");
            var ascending = this.Comparer.Compare(this.Start, step(this.Start)) < 0;

            return ascending ? this.FromStart(step) : this.FromEnd(step);
        }
    }
}
