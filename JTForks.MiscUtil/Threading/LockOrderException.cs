// <copyright file="LockOrderException.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Threading
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Exception thrown when a Lock method on the SyncLock class times out.
    /// </summary>
    public class LockOrderException : Exception
    {
        /// <summary>
        /// Constructs an instance with the specified message.
        /// </summary>
        /// <param name="message">The message for the exception</param>
        internal LockOrderException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance by formatting the specified message with
        /// the given parameters.
        /// </summary>
        /// <param name="message">The message, which will be formatted with the parameters.</param>
        /// <param name="args">The parameters to use for formatting.</param>
        public LockOrderException(string message, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, message, args))
        {
        }
    }
}
