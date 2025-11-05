// <copyright file="RandomAccessCollection.cs" company="MjrTom">
// Copyright (c) Joseph Bridgewater. All rights reserved.
// </copyright>

namespace MiscUtil.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A class with a similar function to System.Collections.Queue,
    /// but allowing random access to the contents of the queue as well
    /// as the usual enqueuing at the end and dequeuing at the start.
    /// This implementation is not synchronized at all - clients should
    /// provide their own synchronization. A SyncRoot is provided for
    /// this purpose, although any other common reference may also be used.
    /// In order to provide an efficient implementation of both random access
    /// and the removal of items from the start of the queue, a circular
    /// buffer is used and resized when necessary. The buffer never shrinks
    /// unless TrimToSize is called.
    /// </summary>
    /// <typeparam name="T">The type of item stored in the queue.</typeparam>
    public sealed class RandomAccessCollection<T> : ICollection<T>, ICollection, ICloneable
    {
        /// <summary>
        /// Default (and minimum) capacity for the buffer containing the elements in the queue.
        /// </summary>
        public const int DefaultCapacity = 16;

        /// <summary>
        /// The circular buffer containing the items in the queue.
        /// </summary>
        private T[] buffer;

        /// <summary>
        /// The "physical" index of item with logical index 0.
        /// </summary>
        private int start;

        /// <summary>
        /// Version information for the queue - this is incremented every time
        /// the contents of the queue is changed, so that enumerators can detect
        /// the change.
        /// </summary>
        private int version;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessCollection{T}"/> class which is empty
        /// and has the default capacity.
        /// </summary>
        public RandomAccessCollection()
            : this(DefaultCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessCollection{T}"/> class which is empty.
        /// and has the specified capacity (or the default capacity if that is higher).
        /// </summary>
        /// <param name="capacity">The initial capacity of the queue.</param>
        public RandomAccessCollection(int capacity)
        {
            this.buffer = new T[Math.Max(capacity, DefaultCapacity)];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomAccessCollection{T}"/> class.
        /// Private constructor used in cloning
        /// </summary>
        /// <param name="buffer">The buffer to clone for use in this queue.</param>
        /// <param name="count">The number of "valid" elements in the buffer.</param>
        /// <param name="start">The first valid element in the queue.</param>
        private RandomAccessCollection(T[] buffer, int count, int start)
        {
            this.buffer = (T[])buffer.Clone();
            this.Count = count;
            this.start = start;
        }

        /// <summary>
        /// Gets current capacity of the queue - the size of the buffer.
        /// </summary>
        public int Capacity => this.buffer.Length;

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this queue is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets an object reference to synchronize on when using the queue
        /// from multiple threads. This reference isn't used anywhere
        /// in the class itself. The same reference will always be returned
        /// for the same queue, and this will never be the same as the reference
        /// returned for a different queue, even a clone.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Gets a value indicating whether this queue is synchronized.
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Indexer for the class, allowing items to be retrieved by
        /// index and replaced.
        /// </summary>
        /// <param name="index">The index of the item to retrieve or replace.</param>
        public T this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Count, nameof(index));
                ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
                return this.buffer[(this.start + index) % this.Capacity];
            }

            set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Count, nameof(index));
                ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));
                this.version++;
                this.buffer[(this.start + index) % this.Capacity] = value;
            }
        }

        /// <summary>
        /// Clears the queue without resizing the buffer.
        /// </summary>
        public void Clear()
        {
            this.start = 0;
            this.Count = 0;
            ((IList)this.buffer).Clear();
        }

        /// <summary>
        /// Resizes the buffer to just fit the current number of items in the queue.
        /// The buffer size is never set to less than the default capacity, however.
        /// </summary>
        public void TrimToSize()
        {
            var newCapacity = Math.Max(this.Count, DefaultCapacity);
            if (this.Capacity == newCapacity)
            {
                return;
            }

            this.Resize(newCapacity, -1);
        }

        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="value">The item to add to the queue. The value can be a null reference.</param>
        public void Enqueue(T value)
        {
            this.Enqueue(value, this.Count);
        }

        /// <summary>
        /// Adds an object at the specified index.
        /// </summary>
        /// <param name="value">The item to add to the queue. The value can be a null reference.</param>
        /// <param name="index">The index of the newly added item.</param>
        public void Enqueue(T value, int index)
        {
            if (this.Count == this.Capacity)
            {
                this.Resize(this.Count * 2, index);
                this.Count++;
            }
            else
            {
                this.Count++;
                // TODO: Make this vaguely efficient :)
                for (var i = this.Count - 2; i >= index; i--)
                {
                    this[i + 1] = this[i];
                }
            }

            this[index] = value;
        }

        /// <summary>
        /// Removes an T from the start of the queue, returning it.
        /// </summary>
        /// <returns>The item at the head of the queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
        public T Dequeue()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("Dequeue called on an empty queue.");
            }

            T ret = this[0];
            Array.Clear(this.buffer, this.start, 1);
            this.start++;
            if (this.start == this.Capacity)
            {
                this.start = 0;
            }

            this.Count--;
            return ret;
        }

        /// <summary>
        /// Removes an item at the given index and returns it.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <returns>The item which has been removed from the queue.</returns>
        public T RemoveAt(int index)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, this.Count, nameof(index));
            ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));

            // Special case: head of queue
            if (index == 0)
            {
                return this.Dequeue();
            }

            T ret = this[index];

            // Special case: end of queue
            if (index == this.Count - 1)
            {
                Array.Clear(this.buffer, (this.start + index) % this.Capacity, 1);
                this.Count--;
                return ret;
            }

            _ = this[index];

            // Everything else involves shuffling things one way or the other.
            // Shuffle things in whichever way involves only a single array copy
            // (either towards the end or towards the start - with a circular buffer
            // it doesn't matter which)
            // TODO: Benchmark this to find out whether one way is faster than the other
            // and possibly put code in to copy the shorter amount, even if it involves two copies
            if (this.start + index >= this.Capacity)
            {
                // Move everything later than index down 1
                Array.Copy(
                    this.buffer,
                    this.start + index - this.Capacity + 1,
                    this.buffer,
                    this.start + index - this.Capacity,
                    this.Count - index - 1);
                Array.Clear(this.buffer, this.start + this.Count - 1 - this.Capacity, 1);
            }
            else
            {
                // Move everything earlier than index up one
                Array.Copy(this.buffer, this.start, this.buffer, this.start + 1, index);
                Array.Clear(this.buffer, this.start, 1);
                this.start++;
            }

            this.Count--;
            this.version++;
            return ret;
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            ArgumentNullException.ThrowIfNull(array);

            if (array is not T[] strongDest)
            {
                throw new ArgumentException(
                    string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Cannot copy elements of type {0} to an array of type {1}",
                        typeof(T).Name,
                        array.GetType().GetElementType()!.Name),
                    nameof(array));
            }

            this.CopyTo(strongDest, index);
        }

        /// <summary>
        /// Performs a binary search using IComparable. If the value occurs multiple times,
        /// there is no guarantee as to which index will be returned. If the value does
        /// not occur at all, the bitwise complement of the first index containing a larger
        /// value is returned (or the bitwise complement of the size of the queue if the value
        /// is larger than any value in the queue). This is the location at which the value should
        /// be inserted to preserve sort order. If the list is not sorted according to
        /// the appropriate IComparable implementation before this method is calling, the result
        /// is not guaranteed. The value passed in must implement IComparable, unless it is null.
        /// The IComparable.CompareTo method will be called on the value passed in, with the
        /// values in the queue as parameters, rather than the other way round. No test is made
        /// to make sure that the types of item are the same - it is up to the implementation of
        /// IComparable to throw an exception if incomparable types are presented.
        /// A null reference is treated as being less than any item, (so passing in null will always
        /// return 0 or -1). The implementation of IComparable is never asked to compare to null.
        /// </summary>
        /// <param name="obj">The item to search for.</param>
        /// <returns>
        /// A location in the queue containing the item, or the bitwise complement of the
        /// first index containing a larger value.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when obj does not implement IComparable.</exception>
        public int BinarySearch(T? obj)
        {
            if (obj == null)
            {
                return this.Count == 0 || this.buffer[this.start] != null ? ~0 : 0;
            }

            if (obj is not IComparable comp)
            {
                throw new ArgumentException("obj does not implement IComparable");
            }

            if (this.Count == 0)
            {
                return ~0;
            }

            var min = 0;
            var max = this.Count - 1;

            while (min <= max)
            {
                var test = (min + max) / 2;
                T element = this[test];
                var result = element == null ? 1 : comp.CompareTo(element);
                if (result == 0)
                {
                    return test;
                }

                if (result < 0)
                {
                    max = test - 1;
                }

                if (result > 0)
                {
                    min = test + 1;
                }
            }

            return ~min;
        }

        /// <summary>
        /// Performs a binary search using the specified IComparer. If the value occurs multiple times,
        /// there is no guarantee as to which index will be returned. If the value does
        /// not occur at all, the bitwise complement of the first index containing a larger
        /// value is returned (or the bitwise complement of the size of the queue if the value
        /// is larger than any value in the queue). This is the location at which the value should
        /// be inserted to preserve sort order. If the list is not sorted according to
        /// the appropriate IComparer implementation before this method is calling, the result
        /// is not guaranteed. The CompareTo method will be called on the comparer passed in, with the
        /// specified value as the first parameter, and values in the queue as the second parameter,
        /// rather than the other way round.
        /// While a null reference should be treated as being less than any object in most
        /// implementations of IComparer, this is not required by this method. Any null references
        /// (whether in the queue or the specified value itself) are passed directly to the CompareTo
        /// method. This allow for IComparers to reverse the usual order, if required.
        /// </summary>
        /// <param name="obj">The object to search for.</param>
        /// <param name="comparer">The comparator to use for searching. Must not be null.</param>
        /// <returns>
        /// A location in the queue containing the object, or the bitwise complement of the
        /// first index containing a larger value.
        /// </returns>
        public int BinarySearch(T? obj, IComparer<T> comparer)
        {
            ArgumentNullException.ThrowIfNull(comparer);
            if (this.Count == 0)
            {
                return ~0;
            }

            var min = 0;
            var max = this.Count - 1;

            while (min <= max)
            {
                var test = (min + max) / 2;
                var result = comparer.Compare(obj, this[test]);
                if (result == 0)
                {
                    return test;
                }

                if (result < 0)
                {
                    max = test - 1;
                }

                if (result > 0)
                {
                    min = test + 1;
                }
            }

            return ~min;
        }

        /// <summary>
        /// Performs a binary search using the specified Comparison. If the value occurs multiple times,
        /// there is no guarantee as to which index will be returned. If the value does
        /// not occur at all, the bitwise complement of the first index containing a larger
        /// value is returned (or the bitwise complement of the size of the queue if the value
        /// is larger than any value in the queue). This is the location at which the value should
        /// be inserted to preserve sort order. If the list is not sorted according to
        /// the appropriate IComparer implementation before this method is calling, the result
        /// is not guaranteed. The CompareTo method will be called on the comparer passed in, with the
        /// specified value as the first parameter, and values in the queue as the second parameter,
        /// rather than the other way round.
        /// While a null reference should be treated as being less than any object in most
        /// implementations of IComparer, this is not required by this method. Any null references
        /// (whether in the queue or the specified value itself) are passed directly to the CompareTo
        /// method. This allow for Comparisons to reverse the usual order, if required.
        /// </summary>
        /// <param name="obj">The object to search for.</param>
        /// <param name="comparison">The comparison to use for searching. Must not be null.</param>
        /// <returns>
        /// A location in the queue containing the object, or the bitwise complement of the
        /// first index containing a larger value.
        /// </returns>
        public int BinarySearch(T? obj, Comparison<T> comparison)
        {
            ArgumentNullException.ThrowIfNull(comparison);
            return this.BinarySearch(
                obj,
                new ComparisonComparer<T>(
                    (x,
                    y) => x is null ? y is null ? 0 : -1 : y is null ? 1 : comparison(x, y)
                )
            );
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the queue.
        /// Note that due to the way C# 2.0 iterators work, we cannot spot changes
        /// to the queue after the enumerator was fetched but before MoveNext() is first
        /// called.
        /// </summary>
        /// <returns>Returns an enumerator for the entire queue.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the collection is modified after the enumerator was created.</exception>
        public IEnumerator<T> GetEnumerator()
        {
            var originalVersion = this.version;

            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
                if (this.version != originalVersion)
                {
                    throw new InvalidOperationException(
                        "Collection was modified after the enumerator was created");
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the queue.
        /// </summary>
        /// <returns>Returns an enumerator for the entire queue.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Creates a new queue with the same contents as this queue.
        /// The queues are separate, however - adding an item to the returned
        /// queue doesn't affect the current queue or vice versa.
        /// A new sync root is also supplied.
        /// </summary>
        /// <returns>A clone of the current queue.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Strongly typed version of ICloneable.Clone. Creates a new queue
        /// with the same contents as this queue.
        /// The queues are separate, however - adding an item to the returned
        /// queue doesn't affect the current queue or vice versa.
        /// A new sync root is also supplied.
        /// </summary>
        /// <returns>A clone of the current queue.</returns>
        public RandomAccessCollection<T> Clone()
        {
            return new RandomAccessCollection<T>(this.buffer, this.Count, this.start);
        }

        /// <summary>
        /// Adds an item to the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            this.Enqueue(item);
        }

        /// <summary>
        /// Returns whether or not the queue contains the given item,
        /// using the default EqualityComparer if the item to find is
        /// non-null.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <returns>True if the queue contains the item, false otherwise.</returns>
        public bool Contains(T? item)
        {
            if (item == null)
            {
                for (var i = 0; i < this.Count; i++)
                {
                    if (this[i] == null)
                    {
                        return true;
                    }
                }

                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < this.Count; i++)
            {
                if (comparer.Equals(this[i], item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the queue to the given array, beginning at
        /// the specified index in the array.
        /// </summary>
        /// <param name="array">The array to copy the contents of the queue into.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException">Thrown if array is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if arrayIndex is negative.</exception>
        /// <exception cref="ArgumentException">Thrown if array is not large enough to hold the contents of the queue.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(array);
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex, nameof(arrayIndex));
            if (array.Length < arrayIndex + this.Count)
            {
                throw new ArgumentException("Not enough space in array for contents of queue", nameof(array));
            }

            Span<T> span = array.AsSpan(arrayIndex, this.Count);
            for (var i = 0; i < this.Count; i++)
            {
                span[i] = this[i];
            }
        }

        /// <summary>
        /// Removes the given item from the queue, if it is present. The first
        /// equal value is removed.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed, false otherwise.</returns>
        public bool Remove(T item)
        {
            if (item is null)
            {
                for (var i = 0; i < this.Count; i++)
                {
                    if (this[i] is null)
                    {
                        this.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (var i = 0; i < this.Count; i++)
            {
                if (comparer.Equals(this[i], item))
                {
                    this.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resizes the queue to a new capacity, optionally leaving a gap at
        /// a specified logical index so that a new item can be slotted in
        /// without further copying
        /// </summary>
        /// <param name="newCapacity">The new capacity</param>
        /// <param name="gapIndex">The logical index at which to insert a gap,
        /// or -1 for no gap.</param>
        private void Resize(int newCapacity, int gapIndex)
        {
            var newBuffer = new T[newCapacity];
            if (gapIndex == -1)
            {
                int firstChunkSize;
                int secondChunkSize;

                // If we don't wrap round, it's easy
                if (this.buffer.Length - this.start >= this.Count)
                {
                    firstChunkSize = this.Count;
                    secondChunkSize = 0;
                }
                else
                {
                    firstChunkSize = this.buffer.Length - this.start;
                    secondChunkSize = this.Count - firstChunkSize;
                }

                Array.Copy(this.buffer, this.start, newBuffer, 0, firstChunkSize);
                Array.Copy(this.buffer, 0, newBuffer, firstChunkSize, secondChunkSize);
            }
            else
            {
                // Aargh. The logic's too difficult to do prettily here. Do it simply instead...
                var outIndex = 0;
                var inIndex = this.start;
                for (var i = 0; i < this.Count; i++)
                {
                    if (i == gapIndex)
                    {
                        outIndex++;
                    }

                    newBuffer[outIndex] = this.buffer[inIndex];
                    outIndex++;
                    inIndex++;
                    if (inIndex == this.buffer.Length)
                    {
                        inIndex = 0;
                    }
                }
            }

            this.buffer = newBuffer;
            this.start = 0;
        }
    }
}
