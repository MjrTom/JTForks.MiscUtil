// <copyright file="VcdiffFormatException.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Compression.Vcdiff
{
    using System;

    /// <summary>
    /// Summary description for VcdiffFormatException.
    /// </summary>
    [Serializable()]
    public class VcdiffFormatException : Exception
    {
        internal VcdiffFormatException(string message)
            : base(message)
        {
        }

        internal VcdiffFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
