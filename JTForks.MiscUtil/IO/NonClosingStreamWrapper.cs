// <copyright file="NonClosingStreamWrapper.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.IO
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Wraps a stream for all operations except Close and Dispose, which
    /// merely flush the stream and prevent further operations from being
    /// carried out using this wrapper.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of the class, wrapping the specified stream.
    /// </remarks>
    /// <param name="stream">The stream to wrap. Must not be null.</param>
    /// <exception cref="ArgumentNullException">stream is null</exception>
    public sealed class NonClosingWrapperStream(Stream stream) : Stream
    {
        /// <summary>
        /// Gets stream wrapped by this wrapper
        /// </summary>
        public Stream BaseStream { get; } = stream ?? throw new ArgumentNullException("stream");

        /// <summary>
        /// Whether this stream has been closed or not
        /// </summary>
        private bool closed;

        /// <summary>
        /// Throws an InvalidOperationException if the wrapper is closed.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void CheckClosed()
        {
            if (this.closed)
            {
                throw new InvalidOperationException("Wrapper has been closed or disposed");
            }
        }

        /// <summary>
        /// Indicates whether or not the underlying stream can be read from.
        /// </summary>
        public override bool CanRead => !this.closed && this.BaseStream.CanRead;

        /// <summary>
        /// Gets a value indicating whether the underlying stream supports seeking.
        /// </summary>
        public override bool CanSeek => !this.closed && this.BaseStream.CanSeek;

        /// <summary>
        /// Gets a value indicating whether the underlying stream can be written to.
        /// </summary>
        public override bool CanWrite => !this.closed && this.BaseStream.CanWrite;

        /// <summary>
        /// This method is not proxied to the underlying stream; instead, the wrapper
        /// is marked as unusable for other (non-close/Dispose) operations. The underlying
        /// stream is flushed if the wrapper wasn't closed before this call.
        /// </summary>
        public override void Close()
        {
            if (!this.closed)
            {
                this.BaseStream.Flush();
            }

            this.closed = true;
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size and cancellation token.
        /// </summary>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            this.CheckClosed();
            return this.BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        /// <summary>
        /// Asynchronously clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            this.CheckClosed();
            return this.BaseStream.FlushAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            this.CheckClosed();
            return this.BaseStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            this.CheckClosed();
            return this.BaseStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            this.CheckClosed();
            return this.BaseStream.ReadAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            this.CheckClosed();
            return this.BaseStream.WriteAsync(buffer, cancellationToken);
        }

        /// <summary>
        /// Flushes the underlying stream.
        /// </summary>
        public override void Flush()
        {
            this.CheckClosed();
            this.BaseStream.Flush();
        }

        /// <summary>
        /// Returns the length of the underlying stream.
        /// </summary>
        public override long Length
        {
            get
            {
                this.CheckClosed();
                return this.BaseStream.Length;
            }
        }

        /// <summary>
        /// Gets or sets the current position in the underlying stream.
        /// </summary>
        public override long Position
        {
            get
            {
                this.CheckClosed();
                return this.BaseStream.Position;
            }
            set
            {
                this.CheckClosed();
                this.BaseStream.Position = value;
            }
        }

        /// <summary>
        /// Reads a sequence of bytes from the underlying stream and advances the
        /// position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. When this method returns, the buffer contains
        /// the specified byte array with the values between offset and
        /// (offset + count- 1) replaced by the bytes read from the underlying source.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at which to begin storing the data
        /// read from the underlying stream.
        /// </param>
        /// <param name="count">
        /// The maximum number of bytes to be read from the
        /// underlying stream.
        /// </param>
        /// <returns>The total number of bytes read into the buffer.
        /// This can be less than the number of bytes requested if that many
        /// bytes are not currently available, or zero (0) if the end of the
        /// stream has been reached.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckClosed();
            return this.BaseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the
        /// stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            this.CheckClosed();
            return this.BaseStream.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">
        /// A value of type SeekOrigin indicating the reference
        /// point used to obtain the new position.
        /// </param>
        /// <returns>The new position within the underlying stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckClosed();
            return this.BaseStream.Seek(offset, origin);
        }

        /// <summary>
        /// Sets the length of the underlying stream.
        /// </summary>
        /// <param name="value">The desired length of the underlying stream in bytes.</param>
        public override void SetLength(long value)
        {
            this.CheckClosed();
            this.BaseStream.SetLength(value);
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream and advances
        /// the current position within the stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">
        /// An array of bytes. This method copies count bytes
        /// from buffer to the underlying stream.
        /// </param>
        /// <param name="offset">
        /// The zero-based byte offset in buffer at
        /// which to begin copying bytes to the underlying stream.
        /// </param>
        /// <param name="count">The number of bytes to be written to the underlying stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckClosed();
            this.BaseStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and
        /// advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream. </param>
        public override void WriteByte(byte value)
        {
            this.CheckClosed();
            this.BaseStream.WriteByte(value);
        }
    }
}

