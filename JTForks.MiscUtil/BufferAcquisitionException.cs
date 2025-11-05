// <copyright file="BufferAcquisitionException.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    using System;

    /// <summary>
    /// Exception thrown to indicate that a buffer of the
    /// desired size cannot be acquired.
    /// </summary>
    public class BufferAcquisitionException : Exception
    {
        /// <summary>
        /// Creates an instance of this class with the given message.
        /// </summary>
        /// <param name="message"></param>
        public BufferAcquisitionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an instance of this class with the given message and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public BufferAcquisitionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates an instance of this class with default values.
        /// </summary>
        public BufferAcquisitionException()
        {
        }
    }
}
