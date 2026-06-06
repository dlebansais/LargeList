namespace LargeCollections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using Contracts;

    /// <summary>
    /// Represents a linear storage of objects of type <typeparamref name="T"/> and fast operations to copy or move elements from a <see cref="Segment{T}"/> object to another.
    /// </summary>
    /// <typeparam name="T">The type of stored elements.</typeparam>
#if STRICT
    internal sealed
#else
    public
#endif
    class Segment<T> : ISegment<T>
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Segment{T}"/> class.
        /// Creates an empty instance, has no pre-allocated capacity and has the specified maximum capacity.
        /// </summary>
        /// <param name="maxCapacity">The maximum number of elements that the new <see cref="Segment{T}"/> can ever store.</param>
        public Segment(int maxCapacity)
        {
            Debug.Assert(maxCapacity > 0);

            Content = new T[maxCapacity];
            MaxCapacity = maxCapacity;

            AssertInvariant();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment{T}"/> class.
        /// Creates an empty instance that has the specified initial and maximum capacities.
        /// </summary>
        /// <param name="initialCapacity">The maximum number of elements that the new <see cref="Segment{T}"/> can initially store.</param>
        /// <param name="maxCapacity">The maximum number of elements that the new <see cref="Segment{T}"/> can ever store.</param>
        public Segment(int initialCapacity, int maxCapacity)
        {
            Debug.Assert(initialCapacity >= 0);
            Debug.Assert(maxCapacity > 0);
            Debug.Assert(initialCapacity <= maxCapacity);

            Content = new T[initialCapacity];
            MaxCapacity = maxCapacity;

            AssertInvariant();
        }
        #endregion

        #region Properties
        /// <inheritdoc cref="ISegment{T}.Capacity" />
        public int Capacity => Content.Length;

        /// <inheritdoc cref="ISegment{T}.Count" />
        public int Count { get; set; }

        /// <inheritdoc cref="ISegment{T}.this[int]" />
        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Count);

                return Content[index];
            }
            set
            {
                Debug.Assert(index >= 0 && index < Count);

                Content[index] = value;
            }
        }

        /// <inheritdoc cref="ISegment{T}.Trimmable" />
        public int Trimmable => Capacity - Count;

        /// <inheritdoc cref="ISegment{T}.Extendable" />
        public int Extendable => MaxCapacity - Count;
        #endregion

        #region Queries
        /// <inheritdoc cref="ISegment{T}.Contains(T)" />
        public bool Contains(T item)
        {
            bool Result = false;

            for (int l = 0; l < Count; l++)
                if (Equals(Content[l], item))
                {
                    Result = true;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <inheritdoc cref="ISegment{T}.IndexOf(T, int, int)" />
        public int IndexOf(T item, int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0 && startIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + count <= Count);

            int Result = -1;

            for (int l = startIndex; l < startIndex + count; l++)
                if (Equals(Content[l], item))
                {
                    Result = l;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <inheritdoc cref="ISegment{T}.LastIndexOf(T, int, int)" />
        public int LastIndexOf(T item, int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0 && startIndex < Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + 1 >= count);

            int Result = -1;

            for (int l = 0; l < count; l++)
                if (Equals(item, Content[startIndex - l]))
                {
                    Result = startIndex - l;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <inheritdoc cref="ISegment{T}.GetEnumerator(long)" />
        public IEnumerator<T> GetEnumerator(long index)
        {
            Debug.Assert(index >= 0 && index <= Count);

            IEnumerator<T> enumerator = ((IEnumerable<T>)Content).GetEnumerator();

            for (int i = 0; i < index; i++)
                enumerator.MoveNext();

            return enumerator;
        }
        #endregion

        #region Commands
        /// <inheritdoc cref="ISegment{T}.Clear" />
        public void Clear()
        {
            for (int i = 0; i < Capacity; i++)
            {
                // ! We don't want T to be nullable, but we want to clear the content of the segment, so we set it to default anyway.
                Content[i] = default!;
            }

            Count = 0;

            AssertInvariant();
        }

        /// <inheritdoc cref="ISegment{T}.Extend(int, out int)" />
        public void Extend(int extended, out int effectiveExtended)
        {
            Debug.Assert(extended >= 0);
            Debug.Assert(Count + extended <= MaxCapacity);

            if (Count + extended > Capacity)
            {
                effectiveExtended = Count + extended - Capacity;
                Array.Resize(ref Content, Count + extended);
            }
            else
            {
                effectiveExtended = 0;
            }

            AssertInvariant();
        }

        /// <inheritdoc cref="ISegment{T}.Trim(int)" />
        public void Trim(int trimmed)
        {
            Debug.Assert(trimmed >= 0);
            Debug.Assert(Count + trimmed <= Capacity);

            Array.Resize(ref Content, Capacity - trimmed);

            AssertInvariant();
        }

        /// <inheritdoc cref="ISegment{T}.MakeRoom(int, int, out int)" />
        public void MakeRoom(int index, int count, out int effectiveExtended)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(index + count <= MaxCapacity);

            Extend(count, out effectiveExtended);
            Count += count;

            for (long l = Count; l > index + count; l--)
                Content[l - 1] = Content[l - 1 - count];
        }

        /// <inheritdoc cref="ISegment{T}.MoveTo(ISegment{T}, int, int, int)" />
        public void MoveTo(ISegment<T> destination, int toIndex, int fromIndex, int count)
        {
            Contract.RequireNotNull(destination, out Segment<T> Destination);

            Debug.Assert(toIndex >= 0 && toIndex <= Destination.Count);
            Debug.Assert(fromIndex >= 0 && fromIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(toIndex + count <= Destination.Count);
            Debug.Assert(fromIndex + count <= Count);

            for (int i = 0; i < count; i++)
                Destination.Content[toIndex + i] = Content[fromIndex + i];

            for (int i = fromIndex; i < Count - count; i++)
                Content[i] = Content[i + count];

            for (int i = Count - count; i < Count; i++)
            {
                // ! We don't want T to be nullable, but we want to clear the content of the segment, so we set it to default anyway.
                Content[i] = default!;
            }

            Count -= count;

            AssertInvariant();
            Destination.AssertInvariant();
        }

        /// <inheritdoc cref="ISegment{T}.Remove(T)" />
        public bool Remove(T item)
        {
            bool Result = false;

            for (int l = 0; l < Count; l++)
                if (Equals(Content[l], item))
                {
                    for (; l + 1 < Count; l++)
                        Content[l] = Content[l + 1];

                    // ! We don't want T to be nullable, but we want to clear the content of the segment, so we set it to default anyway.
                    Content[Count - 1] = default!;
                    Count--;
                    Result = true;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <inheritdoc cref="ISegment{T}.RemoveRange(int, int)" />
        public void RemoveRange(int index, int count)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(index + count <= Count);

            long l;

            for (l = index; l + count < Count; l++)
                Content[l] = Content[l + count];

            for (; l < Count; l++)
            {
                // ! We don't want T to be nullable, but we want to clear the content of the segment, so we set it to default anyway.
                Content[l] = default!;
            }

            Count -= count;
        }

        /// <inheritdoc cref="ISegment{T}.RemoveAll(Predicate{T})" />
        public int RemoveAll(Predicate<T> match)
        {
            Contract.RequireNotNull(match, out Predicate<T> Match);

            int RemovedCount = 0;

            for (int l = 0; l < Count; l++)
                if (Match(Content[l]))
                {
                    for (int n = l; n + 1 < Count; n++)
                        Content[n] = Content[n + 1];

                    // ! We don't want T to be nullable, but we want to clear the content of the segment, so we set it to default anyway.
                    Content[Count - 1] = default!;
                    Count--;

                    l--;
                    RemovedCount++;
                }

            AssertInvariant();

            return RemovedCount;
        }

        /// <inheritdoc cref="ISegment{T}.Sort(int, int, IComparer{T})" />
        public void Sort(int low, int high, IComparer<T> comparer)
        {
            Debug.Assert(low >= 0 && low < Content.Length);
            Debug.Assert(high >= 0 && high < Content.Length);
            Debug.Assert(low <= high);

            Array.Sort(Content, low, high - low + 1, comparer);
        }
        #endregion

        private T[] Content;
        private int MaxCapacity;

        #region Debugging
        /// <inheritdoc />
        public override string ToString()
        {
            return Count.ToString(CultureInfo.InvariantCulture) + " / " + Capacity.ToString(CultureInfo.InvariantCulture);
        }
        #endregion

        #region Contracts
        private void AssertInvariant()
        {
            Debug.Assert(Capacity <= MaxCapacity);
            Debug.Assert(Count <= Capacity);
        }
        #endregion
    }
}
