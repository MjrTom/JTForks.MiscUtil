// <copyright file="ThreadController.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class designed to control a worker thread (co-operatively).
    /// </summary>
    /// <remarks>
    /// Creates a new controller.
    /// </remarks>
    /// <param name="starter">The delegate to invoke when the thread is started.
    /// Must not be null.</param>
    /// <param name="state">The state to pass to the delegate. May be null.</param>
    public class ThreadController(ControlledThreadStart starter, object? state)
    {
        /// <summary>
        /// Lock used throughout for all state management.
        /// (This is unrelated to the "state" variable.)
        /// </summary>
        private readonly object stateLock = new();

        /// <summary>
        /// The delegate to be invoked when the thread is started.
        /// </summary>
        private readonly ControlledThreadStart starter = starter ?? throw new ArgumentNullException("starter");

        /// <summary>
        /// State to pass to the "starter" delegate when the thread is started.
        /// This reference is discarded when the new thread is started, so
        /// it won't prevent garbage collection.
        /// </summary>
        private object? state = state;
        private bool started;
        /// <summary>
        /// Whether the thread has been started. A thread can only
        /// be started once.
        /// </summary>
        public bool Started
        {
            get
            {
                lock (this.stateLock)
                {
                    return this.started;
                }
            }
        }

        private Thread? thread;
        /// <summary>
        /// Thread being controlled. May be null if it hasn't
        /// been created yet.
        /// </summary>
        public Thread? Thread
        {
            get
            {
                lock (this.stateLock)
                {
                    return this.thread;
                }
            }
        }

        /// <summary>
        /// Whether or not the thread is stopping. This may be used
        /// by the thread itself to test whether or not to stop, as
        /// well as by clients checking status. To see whether the
        /// thread has actually finished or not, use the IsAlive
        /// property of the thread itself.
        /// </summary>
        public bool Stopping
        {
            get
            {
                lock (this.stateLock)
                {
                    return field;
                }
            }

            private set;
        }

        private ExceptionHandler? exceptionDelegate;
        /// <summary>
        /// Event raised if the controlled thread throws an unhandled exception.
        /// The exception is not propagated beyond the controller by default, however
        /// by adding an ExceptionHandler which simply rethrows the exception,
        /// it will propagate. Note that in this case any further ExceptionHandlers
        /// added after the propagating one will not be executed. This event is
        /// raised in the worker thread.
        /// </summary>
        public event ExceptionHandler Exception
        {
            add
            {
                lock (this.stateLock)
                {
                    this.exceptionDelegate += value;
                }
            }
            remove
            {
                lock (this.stateLock)
                {
                    this.exceptionDelegate -= value;
                }
            }
        }

        private ThreadProgress? finishedDelegate;
        /// <summary>
        /// Event raised when the thread has finished and all exception handlers
        /// have executed (if an exception was raised). Note that this event is
        /// raised even if one of the exception handlers propagates the exception
        /// up to the top level. This event is raised in the worker thread.
        /// </summary>
        public event ThreadProgress Finished
        {
            add
            {
                lock (this.stateLock)
                {
                    this.finishedDelegate += value;
                }
            }
            remove
            {
                lock (this.stateLock)
                {
                    this.finishedDelegate -= value;
                }
            }
        }

        private ThreadProgress? stopRequestedDelegate;
        /// <summary>
        /// Event raised when a stop is requested. Worker threads
        /// may register for this event to allow them to respond to
        /// stop requests in a timely manner. The event is raised
        /// in the thread which calls the Stop method.
        /// </summary>
        public event ThreadProgress StopRequested
        {
            add
            {
                lock (this.stateLock)
                {
                    this.stopRequestedDelegate += value;
                }
            }
            remove
            {
                lock (this.stateLock)
                {
                    this.stopRequestedDelegate -= value;
                }
            }
        }

        /// <summary>
        /// Creates a new controller without specifying a state object to
        /// pass when the delegate is invoked.
        /// </summary>
        /// <param name="starter">The delegate to invoke when the thread is started.</param>
        public ThreadController(ControlledThreadStart starter)
            : this(starter, null)
        {
        }

        /// <summary>
        /// Creates the thread to later be started. This enables
        /// properties of the thread to be manipulated before the thread
        /// is started.
        /// </summary>
        /// <exception cref="InvalidOperationException">The thread has already been created.</exception>
        public void CreateThread()
        {
            lock (this.stateLock)
            {
                if (this.thread != null)
                {
                    throw new InvalidOperationException("Thread has already been created");
                }

                this.thread = new Thread(new ThreadStart(this.RunTask));
            }
        }

        /// <summary>
        /// Starts the task in a separate thread, creating it if it hasn't already been
        /// created with the CreateThread method.
        /// </summary>
        /// <exception cref="InvalidOperationException">The thread has already been started.</exception>
        public void Start()
        {
            lock (this.stateLock)
            {
                if (this.started)
                {
                    throw new InvalidOperationException("Thread has already been created");
                }

                this.thread ??= new Thread(new ThreadStart(this.RunTask));
                this.thread.Start();
                this.started = true;
            }
        }

        /// <summary>
        /// Tell the thread being controlled by this controller to stop. 
        /// This call does not throw an exception if the thread hasn't been 
        /// created, or has already been told to stop - it is therefore safe 
        /// to call at any time, regardless of other information about the 
        /// state of the controller. Depending on the way in which the controlled
        /// thread is running, it may not take notice of the request to stop
        /// for some time.
        /// </summary>
        public void Stop()
        {
            lock (this.stateLock)
            {
                this.Stopping = true;
            }

            ThreadProgress? handler;
            lock (this.stateLock)
            {
                handler = this.stopRequestedDelegate;
            }

            handler?.Invoke(this);
        }

        /// <summary>
        /// Runs the task specified by starter, catching exceptions and propagating them
        /// to the Exception event.
        /// </summary>
        private void RunTask()
        {
            try
            {
                // Allow state to be garbage collected during execution
                var stateTmp = this.state;
                this.state = null;
                this.starter(this, stateTmp);
            }
            catch (Exception e)
            {
                ExceptionHandler? handler;
                lock (this.stateLock)
                {
                    handler = this.exceptionDelegate;
                }

                handler?.Invoke(this, e);
            }
            finally
            {
                ThreadProgress? handler;
                lock (this.stateLock)
                {
                    handler = this.finishedDelegate;
                }

                handler?.Invoke(this);
            }
        }
    }
}
