namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    public partial class LargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than <see cref="LargeList{T}.Count"/>.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                int SegmentIndex;
                int ElementIndex;
                int CacheIndex;
                Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);
                return Partition.GetItem(SegmentIndex, ElementIndex);
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                int SegmentIndex;
                int ElementIndex;
                int CacheIndex;
                Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);
                Partition.SetItem(SegmentIndex, ElementIndex, value);
            }
        }

#pragma warning disable SA1600
        object ILargeList.this[long index]
        {
            get { return this[index]!; }
            set { this[index] = (T)value; }
        }
#pragma warning restore SA1600

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="LargeList{T}"/> can contain before resizing is required.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><see cref="LargeList{T}"/>.Capacity is set to a value that is less than <see cref="LargeList{T}.Count"/>.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public long Capacity
        {
            get { return Partition.Capacity; }
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");

                if (value > Capacity)
                    Partition.ExtendCapacity(value - Capacity);
                else if (value < Capacity)
                    Partition.TrimCapacity(Capacity - value);

#if DEBUG
                AssertInvariant();
#endif
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="LargeList{T}"/>.
        /// </returns>
        public long Count { get { return Partition.Count; } }
    }
}
