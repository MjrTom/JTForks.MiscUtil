// <copyright file="ThreadPoolWorkItem.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Threading
{
    using System;
    using System.Threading;

    /// <summary>
    /// Class encapsulating an item of work to be executed in a CustomThreadPool.
    /// </summary>
    public class ThreadPoolWorkItem
    {
        /// <summary>
        /// The target delegate for the work item. This is the delegate
        /// which is run when the work item is executed.
        /// </summary>
        public Delegate Target { get; }

        /// <summary>
        /// Gets the parameters passed to the delegate. This may be null,
        /// and will definitely be null if PreserveParameters is false
        /// and the work item has started executing. The contents of
        /// the returned array should not be changed.
        /// </summary>
        public object[]? Parameters { get; private set; }

        /// <summary>
        /// Gets the priority of this work item compared with others. Note
        /// that this is entirely independent of the thread priority - it
        /// serves only to specify the order of execution of a work item.
        /// Items with a higher priority are added ahead of items with a lower
        /// priority in the queue.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Gets a value indicating whether the parameters should be preserved
        /// during and after execution. If this is true, the parameters are available in
        /// the AfterWorkItem and WorkerException events of the containing
        /// CustomThreadPool. However, this means that the contents cannot
        /// be garbage collected until after the work item has finished
        /// executing, which may be costly in some situations.
        /// </summary>
        public bool PreserveParameters { get; }

        /// <summary>
        /// Gets the ID of the work item, which may be null. This is provided
        /// by the caller when the work item is constructed, and is used
        /// for cancellation purposes.
        /// </summary>
        public object? ID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolWorkItem"/> class.
        /// </summary>
        /// <param name="id">The ID of the work item. May be null.</param>
        /// <param name="preserveParameters">
        /// Whether or not the parameter array should be preserved during the work item's
        /// execution to allow the information to be retrieved in the WorkerException and
        /// AfterWorkItem events.
        /// </param>
        /// <param name="cloneParameters">
        /// Whether or not the parameter array provided should be cloned. This should be
        /// true if the contents of the passed array will be changed by the caller afterwards,
        /// but false in the common case of creating the array solely for the purpose of
        /// constructing this work item. Note that the values within the array are not cloned
        /// - just the array itself.
        /// </param>
        /// <param name="priority">The priority of this work item.</param>
        /// <param name="target">
        /// The delegate to run when the work item is executed. Must not be null.
        /// </param>
        /// <param name="parameters">
        /// The parameters to pass to the target delegate. May be null if the delegate
        /// takes no parameters.
        /// </param>
        internal ThreadPoolWorkItem(object? id, bool preserveParameters, bool cloneParameters,
                                   int priority, Delegate target, params object[] parameters)
        {
            this.ID = id;
            this.Priority = priority;
            this.PreserveParameters = preserveParameters;
            this.Target = target ?? throw new ArgumentNullException(nameof(target));
            if (parameters != null)
            {
                this.Parameters = cloneParameters ? (object[])parameters.Clone() : parameters;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolWorkItem"/> class.
        /// Creates a new work item with the given target delegate and parameters.
        /// The parameters (if any) are cloned on construction and preserved during
        /// the work item's execution. The ID of the constructed work item is null,
        /// and the priority is 0.
        /// </summary>
        /// <param name="target">
        /// The delegate to run when the work item is executed. Must not be null.
        /// </param>
        /// <param name="parameters">
        /// The parameters to pass to the target delegate. May be null if the delegate
        /// takes no parameters.
        /// </param>
        public ThreadPoolWorkItem(Delegate target, params object[] parameters)
            : this((object?)null, true, true, 0, target, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolWorkItem"/> class.
        /// Creates a new work item with the given target delegate.
        /// The ID of the constructed work item is null, and the priority is 0.
        /// </summary>
        /// <param name="target">
        /// The delegate to run when the work item is executed. Must not be null.
        /// </param>
        public ThreadPoolWorkItem(Delegate target)
            : this(target, [])
        {
        }

        /// <summary>
        /// Invokes the work item.
        /// </summary>
        internal void Invoke()
        {
            var p = this.Parameters;
            if (!this.PreserveParameters)
            {
                this.Parameters = null!;
            }

            // Should be faster than dynamic invoke
            if (this.Target is ThreadStart)
            {
                ((ThreadStart)this.Target)();
            }
            else
            {
                this.Target.DynamicInvoke(p);
            }
        }
    }
}
