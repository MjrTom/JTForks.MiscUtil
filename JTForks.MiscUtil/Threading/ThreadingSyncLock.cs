// <copyright file="ThreadingSyncLock.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class used for locking, as an alternative to just locking on normal monitors.
    /// Allows for timeouts when locking, and each Lock method returns a token which
    /// must then be disposed of to release the internal monitor (i.e. to unlock).
    /// All properties and methods of this class are thread-safe.
    /// </summary>
    public class ThreadingSyncLock
    {
        /// <summary>
        /// Lock for static mutable properties.
        /// </summary>
        private static readonly object staticLock = new();

        /// <summary>
        /// Gets or sets the default timeout for new instances of this class
        /// where the default timeout isn't otherwise specified.
        /// Defaults to Timeout.Infinite.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static int DefaultDefaultTimeout
        {
            get
            {
                lock (staticLock)
                {
                    return field;
                }
            }
            set
            {
                ArgumentOutOfRangeException.ThrowIfLessThan(value, Timeout.Infinite, nameof(value));

                lock (staticLock)
                {
                    field = value;
                }
            }
        }
= Timeout.Infinite;

        /// <summary>
        /// The default timeout for the
        /// </summary>
        public int DefaultTimeout { get; }

        /// <summary>
        /// The name of this lock.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The internal monitor used for locking. While this
        /// is owned by the thread, it can be used for waiting
        /// and pulsing in the usual way. Note that manually entering/exiting
        /// this monitor could result in the lock malfunctioning.
        /// </summary>
        public object Monitor { get; } = new object();

        /// <summary>
        /// Creates a new lock with no name, and the default timeout specified by DefaultDefaultTimeout.
        /// </summary>
        public ThreadingSyncLock()
            : this(null!, DefaultDefaultTimeout)
        {
        }

        /// <summary>
        /// Creates a new lock with the specified name, and the default timeout specified by
        /// DefaultDefaultTimeout.
        /// </summary>
        /// <param name="name">The name of the new lock</param>
        public ThreadingSyncLock(string name)
            : this(name, DefaultDefaultTimeout)
        {
        }

        /// <summary>
        /// Creates a new lock with no name, and the specified default timeout
        /// </summary>
        /// <param name="defaultTimeout">Default timeout, in milliseconds</param>
        public ThreadingSyncLock(int defaultTimeout)
            : this(null!, defaultTimeout)
        {
        }

        /// <summary>
        /// Creates a new lock with the specified name, and an
        /// infinite default timeout.
        /// </summary>
        /// <param name="name">The name of the new lock</param>
        /// <param name="defaultTimeout">
        /// Default timeout, in milliseconds. Use Timeout.Infinite
        /// for an infinite timeout, or a non-negative number otherwise.
        /// </param>
        public ThreadingSyncLock(string name, int defaultTimeout)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(defaultTimeout, Timeout.Infinite, nameof(defaultTimeout));

            name ??= "Anonymous Lock";
            this.Name = name;
            this.DefaultTimeout = defaultTimeout;
        }

        /// <summary>
        /// Locks the monitor, with the default timeout.
        /// </summary>
        /// <returns>A lock token which should be disposed to release the lock</returns>
        /// <exception cref="LockTimeoutException">The operation times out.</exception>
        public LockToken Lock()
        {
            return this.Lock(this.DefaultTimeout);
        }

        /// <summary>
        /// Locks the monitor, with the specified timeout.
        /// </summary>
        /// <param name="timeout">The timeout duration. When converted to milliseconds,
        /// must be Timeout.Infinite, or non-negative.</param>
        /// <returns>A lock token which should be disposed to release the lock</returns>
        /// <exception cref="LockTimeoutException">The operation times out.</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public LockToken Lock(TimeSpan timeout)
        {
            var millis = (long)timeout.TotalMilliseconds;

            ArgumentOutOfRangeException.ThrowIfLessThan(millis, Timeout.Infinite, nameof(timeout));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(millis, int.MaxValue, nameof(timeout));

            return this.Lock((int)millis);
        }

        /// <summary>
        /// Locks the monitor, with the specified timeout. Derived classes may override
        /// this method to change the behavior; the other calls to Lock all result in
        /// a call to this method. This implementation checks the validity of the timeout,
        /// calls Monitor.TryEnter (throwing an exception if appropriate) and returns a
        /// new LockToken.
        /// </summary>
        /// <param name="timeout">The timeout, in milliseconds. Must be Timeout.Infinite,
        /// or non-negative.</param>
        /// <returns>A lock token which should be disposed to release the lock</returns>
        /// <exception cref="LockTimeoutException">The operation times out.</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public virtual LockToken Lock(int timeout)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(timeout, Timeout.Infinite, nameof(timeout));

            return !System.Threading.Monitor.TryEnter(this.Monitor, timeout)
                ? throw new LockTimeoutException("Failed to acquire lock {0}", this.Name)
                : new LockToken(this);
        }

        /// <summary>
        /// Unlocks the monitor. This method may be overridden in derived classes
        /// to change the behavior. This implementation simply calls Monitor.Exit.
        /// </summary>
        protected internal virtual void Unlock()
        {
            System.Threading.Monitor.Exit(this.Monitor);
        }
    }
}
