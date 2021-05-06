namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Contracts;

    /// <summary>
    /// Represents a set of segments of varying (but limited) capacity that together virtualize a large list of generic objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the partition.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    interface IPartition<T>
    {
        /// <summary>
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but must remain constant in a given <see cref="IPartition{T}"/>.
        /// </summary>
        /// <returns>
        /// The maximum capacity allowed for segments.
        /// </returns>
        int MaxSegmentCapacity { get; }

        /// <summary>
        /// Gets the total number of elements the <see cref="IPartition{T}"/> can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the <see cref="IPartition{T}"/> can hold without resizing.
        /// </returns>
        long Capacity { get; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="IPartition{T}"/>.
        /// </returns>
        long Count { get; }

        /// <summary>
        /// Gets the position of an element in the <see cref="IPartition{T}"/> from its virtual index in a linear list.
        /// </summary>
        /// <param name="index">The virtual index of the element.</param>
        /// <param name="segmentIndex">Upon return, the segment index of the element.</param>
        /// <param name="elementIndex">Upon return, the element index of the element.</param>
        /// <param name="cacheIndex">Upon return, the cache index of the element.</param>
        void GetPosition(long index, out int segmentIndex, out int elementIndex, out int cacheIndex);

        /// <summary>
        /// Check that the specified position in the <see cref="IPartition{T}"/> is valid. Calling this method is reserved to debugging.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position to check.</param>
        /// <param name="elementIndex">The element index of the position to check.</param>
        /// <param name="allowEnd">True to allow the <see cref="IPartition{T}"/>.End position; False to only allow position of existing elements.</param>
        /// <returns>
        /// True if the position in the <see cref="IPartition{T}"/> specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> is valid.
        /// </returns>
        bool IsValidPosition(int segmentIndex, int elementIndex, bool allowEnd);

        /// <summary>
        /// Gets the previous position in the <see cref="IPartition{T}"/>. The returned position may be invalid if <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> specify the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this interface.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexPrevious">Upon return, the segment index of the previous position.</param>
        /// <param name="elementIndexPrevious">Upon return, the element index of the previous position.</param>
        void GetPreviousPosition(int segmentIndex, int elementIndex, out int segmentIndexPrevious, out int elementIndexPrevious);

        /// <summary>
        /// Gets the next position in the <see cref="IPartition{T}"/>. <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> must specify the position of an existing element, or the position that is before the first element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexNext">Upon return, the segment index of the next position.</param>
        /// <param name="elementIndexNext">Upon return, the element index of the next position.</param>
        void GetNextPosition(int segmentIndex, int elementIndex, out int segmentIndexNext, out int elementIndexNext);

        /// <summary>
        /// Updates a position in the <see cref="IPartition{T}"/> to the previous element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        void DecrementPosition(ref int segmentIndex, ref int elementIndex);

        /// <summary>
        /// Updates a position in the <see cref="IPartition{T}"/> to the next element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        void IncrementPosition(ref int segmentIndex, ref int elementIndex);

        /// <summary>
        /// Gets the element in the <see cref="IPartition{T}"/> at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the element.</param>
        /// <param name="elementIndex">The element index of the position of the element.</param>
        /// <returns>
        /// The element in the <see cref="IPartition{T}"/> specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        T GetItem(int segmentIndex, int elementIndex);

        /// <summary>
        /// Returns an enumerator for the <see cref="IPartition{T}"/>, starting from the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the <see cref="IPartition{T}"/>, starting from the element specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        IPartitionEnumerator<T> GetEnumerator(int segmentIndex, int elementIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the specified <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <param name="remainingCount">Upon return, the remaining number of elements that can be enumerated in the <see cref="ISegment{T}"/>.</param>
        /// <returns>
        /// An enumerator for the <see cref="ISegment{T}"/>.
        /// </returns>
        IEnumerator<T> GetSegmentEnumerator(int segmentIndex, int elementIndex, out int remainingCount);

        /// <summary>
        /// Gets the next segment in the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment used as starting point.</param>
        /// <returns>
        /// The index of the segment in the <see cref="IPartition{T}"/> that follows <paramref name="segmentIndex"/>, -1 if <paramref name="segmentIndex"/> specified the last one in the <see cref="IPartition{T}"/>.
        /// </returns>
        int NextSegmentIndex(int segmentIndex);

        /// <summary>
        /// Determines whether an element is in the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IPartition{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="IPartition{T}"/>; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the first occurrence in a linear list that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IPartition{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty partition.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the first occurrence of <paramref name="item"/> within the range of elements in a linear list that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        long IndexOf(T item, long startIndex, long count);

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the last occurrence within the range of elements in a linear list that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="IPartition{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the last occurrence of <paramref name="item"/> within the range of elements in the linear list that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        long LastIndexOf(T item, long startIndex, long count);

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="IPartition{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="IPartition{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="IPartition{T}"/>.Count.
        /// </returns>
        long BinarySearch(long index, long count, T item, IComparer<T> comparer);

        /// <summary>
        /// Removes all elements from the <see cref="IPartition{T}"/>.
        /// </summary>
        void Clear();

        /// <summary>
        /// Increases the <see cref="IPartition{T}"/>.Capacity by the given amount.
        /// </summary>
        /// <param name="extended">The number of elements added to the <see cref="IPartition{T}"/>.Capacity.</param>
        void ExtendCapacity(long extended);

        /// <summary>
        /// Decreases the <see cref="IPartition{T}"/>.Capacity by the given amount.
        /// </summary>
        /// <param name="trimed">The number of elements substracted to the <see cref="IPartition{T}"/>.Capacity.</param>
        void TrimCapacity(long trimed);

        /// <summary>
        /// Makes room for a number of elements starting at the specified position. Elements already the specified position and beyond are moved toward the end of the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="elementIndex">The element index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="cacheIndex">The cache index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        void MakeRoom(int segmentIndex, int elementIndex, int cacheIndex, long count);

        /// <summary>
        /// Replaces the element at the specified position with a new item.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the replaced element.</param>
        /// <param name="elementIndex">The element index of the position of the replaced element.</param>
        /// <param name="item">The item to set.</param>
        void SetItem(int segmentIndex, int elementIndex, T item);

        /// <summary>
        /// Replaces a range of elements at the specified position with new items from a collection.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of replaced elements.</param>
        /// <param name="elementIndex">The element index of the position of replaced elements.</param>
        /// <param name="collection">The collection containing items to set.</param>
        void SetItemRange(int segmentIndex, int elementIndex, IEnumerable<T> collection);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="IPartition{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the <see cref="IPartition{T}"/>.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes a range of elements from the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to remove.</param>
        /// <param name="elementIndex">The element index of the position of the first element to remove.</param>
        /// <param name="cacheIndex">The cache index of the position of the first element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        void RemoveRange(int segmentIndex, int elementIndex, int cacheIndex, long count);

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the <see cref="IPartition{T}"/>.
        /// </returns>
        long RemoveAll(Predicate<T> match);

        /// <summary>
        /// Reverses the order of the elements in the specified range of the <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        void Reverse(int segmentIndexBegin, int elementIndexBegin, int segmentIndexEnd, int elementIndexEnd, long count);

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="IPartition{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        void Sort(int segmentIndexBegin, int elementIndexBegin, int segmentIndexEnd, int elementIndexEnd, long count, IComparer<T> comparer);
    }
}
