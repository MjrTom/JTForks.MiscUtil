// <copyright file="CachedBuffer.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil
{
    using System;

    /// <summary>
    /// Type of buffer returned by CachingBufferManager.
    /// </summary>
    internal class CachedBuffer : IBuffer
    {
        private volatile bool available;
        private readonly bool clearOnDispose;

        internal CachedBuffer(int size, bool clearOnDispose)
        {
            this.Bytes = new byte[size];
            this.clearOnDispose = clearOnDispose;
        }

        internal bool Available
        {
            get { return this.available; }
            set { this.available = value; }
        }

        public byte[] Bytes { get; }

        public void Dispose()
        {
            if (this.clearOnDispose)
            {
                Array.Clear(this.Bytes, 0, this.Bytes.Length);
            }

            this.available = true;
        }
    }
}
