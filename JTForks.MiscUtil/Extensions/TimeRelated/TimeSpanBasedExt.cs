// <copyright file="TimeSpanBasedExt.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Extensions.TimeRelated
{
    using System;

    /// <summary>
    /// Extension methods producing TimeSpan values. Note: Ticks should really
    /// take a long, and the rest should all take doubles. It looks like extension
    /// methods don't quite work properly with implicit numeric conversions :(
    /// </summary>
    public static class TimeSpanBasedExt
    {
        /// <summary>
        /// Returns a TimeSpan representing the specified number of ticks.
        /// </summary>
        /// <param name="ticks"></param>
        public static TimeSpan Ticks(this int ticks)
        {
            return TimeSpan.FromTicks(ticks);
        }

        /// <summary>
        /// Returns a TimeSpan representing the specified number of milliseconds.
        /// </summary>
        /// <param name="milliseconds"></param>
        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }

        /// <summary>
        /// Returns a TimeSpan representing the specified number of seconds.
        /// </summary>
        /// <param name="seconds"></param>
        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Returns a TimeSpan representing the specified number of minutes.
        /// </summary>
        /// <param name="minutes"></param>
        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        /// <summary>
        /// Returns a TimeSpan representing the specified number of hours.
        /// </summary>
        /// <param name="hours"></param>
        public static TimeSpan Hours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        /// <summary>
        /// Returns a TimeSpan representing the specified number of days.
        /// </summary>
        /// <param name="days"></param>
        public static TimeSpan Days(this int days)
        {
            return TimeSpan.FromDays(days);
        }
    }
}
