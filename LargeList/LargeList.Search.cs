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
        /// Searches the entire sorted <see cref="LargeList{T}"/> for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="LargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="LargeList{T}.Count"/>.
        /// </returns>
        public long BinarySearch(T item)
        {
            return BinarySearchItem(0, Count, item, Comparer<T>.Default);
        }

        /// <summary>
        /// Searches the entire sorted <see cref="LargeList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="LargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="LargeList{T}.Count"/>.
        /// </returns>
        public long BinarySearch(T item, IComparer<T> comparer)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;

            return BinarySearchItem(0, Count, item, comparer);
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="LargeList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="LargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="LargeList{T}.Count"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the <see cref="LargeList{T}"/>.</exception>
        public long BinarySearch(long index, long count, T item, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            if (comparer == null)
                comparer = Comparer<T>.Default;

            return BinarySearchItem(index, count, item, comparer);
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="LargeList{T}"/>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return Partition.Contains(item);
        }

#pragma warning disable SA1600
        bool ILargeList.Contains(object item)
        {
            return Contains((T)item);
        }
#pragma warning restore SA1600

        /// <summary>
        /// Determines whether the <see cref="LargeList{T}"/> contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// true if the <see cref="LargeList{T}"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool Exists(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                if (match(Partition.GetItem(SegmentIndex, ElementIndex)))
                    return true;

                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return false;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                T item = Partition.GetItem(SegmentIndex, ElementIndex);
                if (match(item))
                    return item;

                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return default(T)!;
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A <see cref="LargeList{T}"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="LargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public LargeList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            LargeList<T> Result = new LargeList<T>();

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                T item = Partition.GetItem(SegmentIndex, ElementIndex);
                if (match(item))
                    Result.Add(item);

                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return Result;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            return FindIndex(0, Count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long FindIndex(long startIndex, Predicate<T> match)
        {
            if (startIndex < 0 || startIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            return FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long FindIndex(long startIndex, long count, Predicate<T> match)
        {
            if (startIndex < 0 || startIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");

            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(startIndex, out SegmentIndex, out ElementIndex, out CacheIndex);

            for (long l = 0; l < count; l++)
            {
                if (match(Partition.GetItem(SegmentIndex, ElementIndex)))
                    return l + startIndex;

                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T FindLast(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(Count, out SegmentIndex, out ElementIndex, out CacheIndex);

            for (long l = 0; l < Count; l++)
            {
                Partition.DecrementPosition(ref SegmentIndex, ref ElementIndex);
                T item = Partition.GetItem(SegmentIndex, ElementIndex);
                if (match(item))
                    return item;
            }

            return default(T)!;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindLastIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (Count > 0)
                return FindLastIndex(Count - 1, Count, match);
            else
                return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long FindLastIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (startIndex < -1 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long FindLastIndex(long startIndex, long count, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (startIndex < -1 || (Count == 0 && startIndex < 0) || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");

            if (startIndex + 1 < count)
            {
                Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.", InnerException);
            }

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(startIndex, out SegmentIndex, out ElementIndex, out CacheIndex);

            for (long l = 0; l < count; l++)
            {
                if (match(Partition.GetItem(SegmentIndex, ElementIndex)))
                    return startIndex - l;

                Partition.DecrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return -1;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="LargeList{T}"/>, if found; otherwise, –1.
        /// </returns>
        public long IndexOf(T item)
        {
            return Partition.IndexOf(item, 0, Count);
        }

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long IndexOf(T item, long index)
        {
#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
#endif

            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.IndexOf(item, index, Count - index);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that extends from <paramref name="startIndex"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long IndexOf(T item, long startIndex)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.IndexOf(item, startIndex, Count - startIndex);
        }
#endif

#if STRICT
            /// <summary>
            /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that starts at the specified index and contains the specified number of elements.
            /// </summary>
            /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
            /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
            /// <param name="count">The number of elements in the section to search.</param>
            /// <returns>
            /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long IndexOf(T item, long index, long count)
        {
#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
#endif

            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return Partition.IndexOf(item, index, count);
        }
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item, 0, Count);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="LargeList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long IndexOf(T item, long startIndex, long count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (startIndex + count > Count)
            {
                Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.", InnerException);
            }

            return Partition.IndexOf(item, startIndex, count);
        }

#pragma warning disable SA1600
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item, 0, Count);
        }
#pragma warning restore SA1600
#endif

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the entire the <see cref="LargeList{T}"/>, if found; otherwise, –1.
        /// </returns>
        public long LastIndexOf(T item)
        {
            if (Count > 0)
                return Partition.LastIndexOf(item, Count - 1, Count);
            else
                return -1;
        }

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that extends from the first element to <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long LastIndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.LastIndexOf(item, index, index + 1);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that extends from the first element to <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</exception>
        public long LastIndexOf(T item, long startIndex)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.LastIndexOf(item, startIndex, startIndex + 1);
        }
#endif

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long LastIndexOf(T item, long index, long count)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + 1 < count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return Partition.LastIndexOf(item, index, count);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="LargeList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="LargeList{T}"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="LargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="LargeList{T}"/>.</para></exception>
        public long LastIndexOf(T item, long startIndex, long count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (startIndex + 1 < count)
            {
                Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from startIndex to the end of the source collection.", InnerException);
            }

            return Partition.LastIndexOf(item, startIndex, count);
        }
#endif

        /// <summary>
        /// Determines whether every element in the <see cref="LargeList{T}"/> matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// true if every element in the <see cref="LargeList{T}"/> matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                if (!match(Partition.GetItem(SegmentIndex, ElementIndex)))
                    return false;

                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

            return true;
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="LargeList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="LargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="LargeList{T}.Count"/>.
        /// </returns>
        private long BinarySearchItem(long index, long count, T item, IComparer<T> comparer)
        {
            return Partition.BinarySearch(index, count, item, comparer);
        }
    }
}
