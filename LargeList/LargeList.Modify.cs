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
        /// Removes all elements from the <see cref="LargeList{T}"/>.
        /// </summary>
        public void Clear()
        {
            Partition.Clear();

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Adds an object to the end of the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Add(T item)
        {
            long Index = Count;

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(Index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.MakeRoom(SegmentIndex, ElementIndex, CacheIndex, 1);

            Partition.GetPosition(Index, out SegmentIndex, out ElementIndex, out CacheIndex);
            Partition.SetItem(SegmentIndex, ElementIndex, item);

#if DEBUG
            AssertInvariant();
#endif
        }

#pragma warning disable SA1600
        long ILargeList.Add(object value)
        {
            long Index = Count;
            Add((T)value);
            return Index;
        }
#pragma warning restore SA1600

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="LargeList{T}"/>. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Value cannot be null.");

            long CollectionCount = GetCollectionCount(collection);
            long Index = Count;

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(Index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.MakeRoom(SegmentIndex, ElementIndex, CacheIndex, CollectionCount);

            Partition.GetPosition(Index, out SegmentIndex, out ElementIndex, out CacheIndex);
            Partition.SetItemRange(SegmentIndex, ElementIndex, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Inserts an element into the <see cref="LargeList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is greater than <see cref="LargeList{T}.Count"/>.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Insert(long index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.MakeRoom(SegmentIndex, ElementIndex, CacheIndex, 1);

            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);
            Partition.SetItem(SegmentIndex, ElementIndex, item);

#if DEBUG
            AssertInvariant();
#endif
        }

#pragma warning disable SA1600
        void ILargeList.Insert(long index, object item)
        {
            Insert(index, (T)item);
        }
#pragma warning restore SA1600

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="LargeList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the <see cref="LargeList{T}"/>. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is greater than <see cref="LargeList{T}.Count"/>.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void InsertRange(long index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Value cannot be null.");

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            long CollectionCount = GetCollectionCount(collection);

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.MakeRoom(SegmentIndex, ElementIndex, CacheIndex, CollectionCount);

            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);
            Partition.SetItemRange(SegmentIndex, ElementIndex, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the <see cref="LargeList{T}"/>.
        /// </returns>
        public bool Remove(T item)
        {
            bool Result = false;

            if (Partition.Remove(item))
                Result = true;

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

#pragma warning disable SA1600
        void ILargeList.Remove(object item)
        {
            Remove((T)item);
        }
#pragma warning restore SA1600

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the <see cref="LargeList{T}"/> .
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            long RemovedCount = Partition.RemoveAll(match);

#if DEBUG
            AssertInvariant();
#endif

            return RemovedCount;
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than <see cref="LargeList{T}.Count"/>.</para></exception>
        public void RemoveAt(long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.RemoveRange(SegmentIndex, ElementIndex, CacheIndex, 1);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="LargeList{T}"/>.</exception>
        public void RemoveRange(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            Partition.RemoveRange(SegmentIndex, ElementIndex, CacheIndex, count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="LargeList{T}"/>.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, Count);
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="LargeList{T}"/>.</exception>
        public void Reverse(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            int SegmentIndexBegin;
            int ElementIndexBegin;
            int CacheIndexBegin;
            Partition.GetPosition(index, out SegmentIndexBegin, out ElementIndexBegin, out CacheIndexBegin);

            int SegmentIndexEnd;
            int ElementIndexEnd;
            int CacheIndexEnd;
            Partition.GetPosition(index + count, out SegmentIndexEnd, out ElementIndexEnd, out CacheIndexEnd);

            Partition.Reverse(SegmentIndexBegin, ElementIndexBegin, SegmentIndexEnd, ElementIndexEnd, count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="LargeList{T}"/> using the default comparer.
        /// </summary>
        public void Sort()
        {
            SortItems(0, Count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="LargeList{T}"/> using the specified <see cref="System.Comparison{T}"/>.
        /// </summary>
        /// <param name="comparison">The <see cref="System.Comparison{T}"/> to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort. For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
        public void Sort(Comparison<T> comparison)
        {
#if STRICT
            if (comparison == null)
                throw new ArgumentNullException("match", "Value cannot be null.");
#else
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison), "Value cannot be null.");
#endif

            SortItems(0, Count, Comparer<T>.Create(comparison));
        }

        /// <summary>
        /// Sorts the elements in the entire <see cref="LargeList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            SortItems(0, Count, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="LargeList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid range in the <see cref="LargeList{T}"/>.</para><para>-or-</para><para>The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</para></exception>
        public void Sort(long index, long count, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            if (comparer == null)
                comparer = Comparer<T>.Default;

            SortItems(index, count, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="LargeList{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        private void SortItems(long index, long count, IComparer<T> comparer)
        {
            int SegmentIndexBegin;
            int ElementIndexBegin;
            int CacheIndexBegin;
            Partition.GetPosition(index, out SegmentIndexBegin, out ElementIndexBegin, out CacheIndexBegin);

            int SegmentIndexEnd;
            int ElementIndexEnd;
            int CacheIndexEnd;
            Partition.GetPosition(index + count, out SegmentIndexEnd, out ElementIndexEnd, out CacheIndexEnd);

            Partition.Sort(SegmentIndexBegin, ElementIndexBegin, SegmentIndexEnd, ElementIndexEnd, count, comparer);

#if DEBUG
            AssertInvariant();
#endif
        }
    }
}
