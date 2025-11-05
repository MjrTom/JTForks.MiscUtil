// <copyright file="AddressCache.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Compression.Vcdiff
{
    using System;
    using System.IO;

    /// <summary>
    /// Cache used for encoding/decoding addresses.
    /// </summary>
    internal sealed class AddressCache : IDisposable
    {
        private const byte SelfMode = 0;
        private const byte HereMode = 1;

        private readonly int nearSize;
        private readonly int sameSize;
        private readonly int[] near;
        private int nextNearSlot;
        private readonly int[] same;

        private Stream addressStream;

        internal AddressCache(int nearSize, int sameSize)
        {
            this.nearSize = nearSize;
            this.sameSize = sameSize;
            this.near = new int[nearSize];
            this.same = new int[sameSize * 256];
            this.addressStream = Stream.Null;
        }

        internal void Reset(byte[] addresses)
        {
            this.nextNearSlot = 0;
            Array.Clear(this.near, 0, this.near.Length);
            Array.Clear(this.same, 0, this.same.Length);

            this.addressStream = new MemoryStream(addresses, false);
        }

        internal int DecodeAddress(int here, byte mode)
        {
            int ret;
            if (mode == SelfMode)
            {
                ret = IOHelper.ReadBigEndian7BitEncodedInt(this.addressStream);
            }
            else if (mode == HereMode)
            {
                ret = here - IOHelper.ReadBigEndian7BitEncodedInt(this.addressStream);
            }
            else if (mode - 2 < this.nearSize) // Near cache
            {
                ret = this.near[mode - 2] + IOHelper.ReadBigEndian7BitEncodedInt(this.addressStream);
            }
            else // Same cache
            {
                var m = mode - (2 + this.nearSize);
                ret = this.same[(m * 256) + IOHelper.CheckedReadByte(this.addressStream)];
            }

            this.Update(ret);
            return ret;
        }

        private void Update(int address)
        {
            if (this.nearSize > 0)
            {
                this.near[this.nextNearSlot] = address;
                this.nextNearSlot = (this.nextNearSlot + 1) % this.nearSize;
            }

            if (this.sameSize > 0)
            {
                this.same[address % (this.sameSize * 256)] = address;
            }
        }

        public void Dispose()
        {
            this.addressStream.Dispose();
        }
    }
}
