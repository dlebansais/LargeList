namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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

    /// <summary>
    /// Represents a set of segments of varying (but limited) capacity that together virtualize a large list of generic objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the partition.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    partial class Partition<T> : IPartition<T>
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the <see cref="Partition{T}"/> class.
        /// With the specified initial number of uninitialized elements and capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new partition can initially store.</param>
        /// <param name="count">The number of uninitialized elements that the new partition contains.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of Partition<T>.Initialize and Partition<T>.MaxSegmentCapacity")]
        public Partition(long capacity, long count, int maxSegmentCapacity)
        {
            Debug.Assert(capacity >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(maxSegmentCapacity > 0);
            Debug.Assert(count <= capacity);

            Initialize();

            Capacity = capacity;
            Count = count;
            MaxSegmentCapacity = maxSegmentCapacity;
            SegmentTable = new List<ISegment<T>>();

            ISegment<T> NewSegment;
            long RemainingCapacity = capacity;
            long RemainingCount = count;
            int effectiveExtended;

            while (RemainingCapacity > MaxSegmentCapacity)
            {
                long SegmentCount = RemainingCount;
                if (SegmentCount > MaxSegmentCapacity)
                    SegmentCount = MaxSegmentCapacity;

                NewSegment = CreateMaxCapacitySegment();
                NewSegment.MakeRoom(0, (int)SegmentCount, out effectiveExtended);
                SegmentTable.Add(NewSegment);

                RemainingCapacity -= MaxSegmentCapacity;
                RemainingCount -= SegmentCount;
            }

            NewSegment = CreateSegment((int)RemainingCapacity);
            NewSegment.MakeRoom(0, (int)RemainingCount, out effectiveExtended);
            SegmentTable.Add(NewSegment);

            InitCache();

            Debug.Assert(Capacity == capacity);
            Debug.Assert(Count == count);
            Debug.Assert(!IsValidPosition(0, 0, false) || IsValidPosition(0, 0, true));
            Debug.Assert(!IsValidPosition(0, SegmentTable[0].Count + 1, true));
            Debug.Assert(SegmentTable.Count > 0);
            Debug.Assert(SegmentTable[0].ToString() != null); // For code coverage.

#if DEBUG
            AssertInvariant();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but remains constant in a given <see cref="Partition{T}"/>.
        /// </summary>
        /// <returns>
        /// The maximum capacity allowed for segments.
        /// </returns>
        public int MaxSegmentCapacity { get; private set; }

        /// <summary>
        /// Gets the total number of elements the <see cref="Partition{T}"/> can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the <see cref="Partition{T}"/> can hold without resizing.
        /// </returns>
        public long Capacity { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="Partition{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="Partition{T}"/>.
        /// </returns>
        public long Count { get; private set; }
        #endregion

        #region Queries
        /// <summary>
        /// Gets the position of an element in the <see cref="Partition{T}"/> from its virtual index in a linear list.
        /// </summary>
        /// <param name="index">The virtual index of the element.</param>
        /// <param name="segmentIndex">Upon return, the segment index of the element.</param>
        /// <param name="elementIndex">Upon return, the element index of the element.</param>
        /// <param name="cacheIndex">Upon return, the cache index of the element.</param>
        public void GetPosition(long index, out int segmentIndex, out int elementIndex, out int cacheIndex)
        {
            Debug.Assert(index >= 0 && index <= Count);

            PositionFromCache(index, out segmentIndex, out elementIndex, out cacheIndex);

            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Check that the specified position in the <see cref="Partition{T}"/> is valid. Calling this method is reserved to debugging.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position to check.</param>
        /// <param name="elementIndex">The element index of the position to check.</param>
        /// <param name="allowEnd">True to allow the <see cref="Partition{T}"/>.End position; False to only allow position of existing elements.</param>
        /// <returns>
        /// True if the position in the <see cref="Partition{T}"/> specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> is valid.
        /// </returns>
        public bool IsValidPosition(int segmentIndex, int elementIndex, bool allowEnd)
        {
            BreakIfNotDebugging();

            if (segmentIndex < 0 || segmentIndex >= SegmentTable.Count)
                return false;

            if (elementIndex < SegmentTable[segmentIndex].Count)
                return true;

            if (!allowEnd)
                return false;

            if (elementIndex == SegmentTable[segmentIndex].Count)
                return true;

            return false;
        }

        /// <summary>
        /// Gets the previous position in the <see cref="Partition{T}"/>. The returned position may be invalid if <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> specify the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this interface.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexPrevious">Upon return, the segment index of the previous position.</param>
        /// <param name="elementIndexPrevious">Upon return, the element index of the previous position.</param>
        public void GetPreviousPosition(int segmentIndex, int elementIndex, out int segmentIndexPrevious, out int elementIndexPrevious)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            if (elementIndex > 0)
            {
                segmentIndexPrevious = segmentIndex;
                elementIndexPrevious = elementIndex - 1;
            }

            else
            {
                segmentIndex--;

                if (segmentIndex >= 0)
                {
                    Debug.Assert(SegmentTable[segmentIndex].Count > 0);
                    segmentIndexPrevious = segmentIndex;
                    elementIndexPrevious = SegmentTable[segmentIndex].Count - 1;
                }
                else
                {
                    segmentIndexPrevious = segmentIndex;
                    elementIndexPrevious = 0;
                }
            }

            Debug.Assert(IsValidPosition(segmentIndexPrevious, elementIndexPrevious, false) || (segmentIndexPrevious == -1 && elementIndexPrevious == 0));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Gets the next position in the <see cref="Partition{T}"/>. <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> must specify the position of an existing element, or the position that is before the first element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexNext">Upon return, the segment index of the next position.</param>
        /// <param name="elementIndexNext">Upon return, the element index of the next position.</param>
        public void GetNextPosition(int segmentIndex, int elementIndex, out int segmentIndexNext, out int elementIndexNext)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            if (elementIndex + 1 < SegmentTable[segmentIndex].Count || segmentIndex + 1 >= SegmentTable.Count || SegmentTable[segmentIndex + 1].Count == 0)
            {
                segmentIndexNext = segmentIndex;
                elementIndexNext = elementIndex + 1;
            }

            else
            {
                segmentIndexNext = segmentIndex + 1;
                elementIndexNext = 0;
            }

            Debug.Assert(IsValidPosition(segmentIndexNext, elementIndexNext, true));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Updates a position in the <see cref="Partition{T}"/> to the previous element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        public void DecrementPosition(ref int segmentIndex, ref int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            if (elementIndex > 0)
                elementIndex--;

            else
            {
                segmentIndex--;

                if (segmentIndex >= 0)
                {
                    Debug.Assert(SegmentTable[segmentIndex].Count > 0);
                    elementIndex = SegmentTable[segmentIndex].Count - 1;
                }
                else
                    elementIndex = 0;
            }

            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false) || (segmentIndex == -1 && elementIndex == 0));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Updates a position in the <see cref="Partition{T}"/> to the next element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        public void IncrementPosition(ref int segmentIndex, ref int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            if (elementIndex + 1 < SegmentTable[segmentIndex].Count || segmentIndex + 1 >= SegmentTable.Count || SegmentTable[segmentIndex + 1].Count == 0)
                elementIndex++;

            else
            {
                segmentIndex++;
                elementIndex = 0;
            }

            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Gets the element in the <see cref="Partition{T}"/> at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the element.</param>
        /// <param name="elementIndex">The element index of the position of the element.</param>
        /// <returns>
        /// The element in the <see cref="Partition{T}"/> specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        public T GetItem(int segmentIndex, int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            T Result = SegmentTable[segmentIndex][elementIndex];

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Returns an enumerator for the <see cref="Partition{T}"/>, starting from the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the <see cref="Partition{T}"/>, starting from the element specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        public IPartitionEnumerator<T> GetEnumerator(int segmentIndex, int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            IPartitionEnumerator<T> Result = CreateEnumerator(segmentIndex, elementIndex);

            Debug.Assert(Result != null);

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the specified <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <param name="remainingCount">Upon return, the remaining number of elements that can be enumerated in the <see cref="ISegment{T}"/>.</param>
        /// <returns>
        /// An enumerator for the <see cref="ISegment{T}"/>.
        /// </returns>
        public IEnumerator<T> GetSegmentEnumerator(int segmentIndex, int elementIndex, out int remainingCount)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            ISegment<T> Segment = SegmentTable[segmentIndex];

            remainingCount = Segment.Count - elementIndex;
            Debug.Assert(remainingCount > 0);

            return Segment.GetEnumerator(elementIndex);
        }

        /// <summary>
        /// Gets the next segment in the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment used as starting point.</param>
        /// <returns>
        /// The index of the segment in the <see cref="Partition{T}"/> that follows <paramref name="segmentIndex"/>, -1 if <paramref name="segmentIndex"/> specified the last one in the <see cref="Partition{T}"/>.
        /// </returns>
        public int NextSegmentIndex(int segmentIndex)
        {
            Debug.Assert(segmentIndex >= 0 && segmentIndex < SegmentTable.Count);

            int Result;
            if (segmentIndex + 1 < SegmentTable.Count)
                Result = segmentIndex + 1;
            else
                Result = -1;

            Debug.Assert(Result == -1 || (Result >= 0 && Result < SegmentTable.Count));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Removes all elements from the <see cref="Partition{T}"/>.
        /// </summary>
        public void Clear()
        {
            foreach (ISegment<T> Segment in SegmentTable)
                Segment.Clear();

            Count = 0;
            RebuildCache();

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Increases the <see cref="Partition{T}"/>.Capacity by the given amount.
        /// </summary>
        /// <param name="extended">The number of elements added to the <see cref="Partition{T}"/>.Capacity.</param>
        public void ExtendCapacity(long extended)
        {
            Debug.Assert(extended >= 0);

            long RemainingCapacity = extended;

            // We first extend the capacity of the last segment in the partition.
            if (SegmentTable[SegmentTable.Count - 1].Capacity < MaxSegmentCapacity)
            {
                int Extendable = SegmentTable[SegmentTable.Count - 1].Extendable;
                if (Extendable > RemainingCapacity)
                    Extendable = (int)RemainingCapacity;

                int effectiveExtended;
                SegmentTable[SegmentTable.Count - 1].Extend(Extendable, out effectiveExtended);
                RemainingCapacity -= effectiveExtended;
            }

            Debug.Assert(RemainingCapacity >= 0);

            // Then we add as many empty segments as necessary to increase the partition capacity.
            while (RemainingCapacity > MaxSegmentCapacity)
            {
                SegmentTable.Add(CreateMaxCapacitySegment());
                RemainingCapacity -= MaxSegmentCapacity;
            }

            Debug.Assert(RemainingCapacity >= 0 && RemainingCapacity <= MaxSegmentCapacity);

            if (RemainingCapacity > 0)
                SegmentTable.Add(CreateSegment((int)RemainingCapacity));

            Capacity += extended;

            RebuildCache();

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Decreases the <see cref="Partition{T}"/>.Capacity by the given amount.
        /// </summary>
        /// <param name="trimed">The number of elements substracted to the <see cref="Partition{T}"/>.Capacity.</param>
        public void TrimCapacity(long trimed)
        {
            Debug.Assert(trimed >= 0);

            long RemainingTrim = trimed;
            int RemoveIndex = SegmentTable.Count;
            bool EmptySegment = true;

            // Starting from the end of the partition, we trim all segments until Capacity reaches the target.
            for (int i = SegmentTable.Count; i > 0 && RemainingTrim > 0; i--)
            {
                ISegment<T> Segment = SegmentTable[i - 1];
                int Trimmable = Segment.Trimmable;
                if (Trimmable == 0)
                    continue;

                if (Trimmable > RemainingTrim)
                    Trimmable = (int)RemainingTrim;

                if (Segment.Count > 0)
                {
                    EmptySegment = false;
                    Segment.Trim(Trimmable);
                }
                else
                {
                    Debug.Assert(EmptySegment);

                    if (i > 1 && Trimmable == Segment.Capacity)
                        RemoveIndex = i - 1;
                    else
                    {
                        Debug.Assert(RemainingTrim == Trimmable);
                        Segment.Trim(Trimmable);
                    }
                }

                RemainingTrim -= Trimmable;
            }

            Debug.Assert(RemainingTrim == 0);

            // Remove all unused segments
            RemoveSegmentRange(RemoveIndex, SegmentTable.Count - RemoveIndex);

            Capacity -= trimed;

            RebuildCache();

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Makes room for a number of elements starting at the specified position. Elements already the specified position and beyond are moved toward the end of the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="elementIndex">The element index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="cacheIndex">The cache index of the position at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        public void MakeRoom(int segmentIndex, int elementIndex, int cacheIndex, long count)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));
            Debug.Assert(cacheIndex >= 0);
            Debug.Assert(count >= 0);

            long RemainingCount = count;
            int effectiveExtended;

            // First we try to make room in just one segment.
            int Extendable = SegmentTable[segmentIndex].Extendable;
            if (Extendable >= RemainingCount)
            {
                SegmentTable[segmentIndex].MakeRoom(elementIndex, (int)RemainingCount, out effectiveExtended);
                Capacity += effectiveExtended;
            }

            else
            {
                // Otherwise, we're going to need to move elements upward.
                int SegmentEndCount = SegmentTable[segmentIndex].Count - elementIndex;
                int NextExtendable = (segmentIndex + 1 < SegmentTable.Count) ? SegmentTable[segmentIndex + 1].Extendable : -1;

                Debug.Assert(SegmentEndCount >= 0);
                Debug.Assert(elementIndex + SegmentEndCount + RemainingCount > MaxSegmentCapacity);

                // If there is room for elements in the next segment, use it.
                if (SegmentEndCount <= NextExtendable)
                {
                    Debug.Assert(NextExtendable >= 0);
                    Debug.Assert(segmentIndex + 1 < SegmentTable.Count);
                    Debug.Assert(SegmentTable[segmentIndex + 1].Count + SegmentEndCount <= MaxSegmentCapacity);

                    SegmentTable[segmentIndex + 1].MakeRoom(0, SegmentEndCount, out effectiveExtended);
                    Capacity += effectiveExtended;
                }

                // Otherwise, perform a split.
                else if (SegmentEndCount > 0)
                {
                    ISegment<T> NewSegment = CreateSegment(SegmentEndCount);
                    NewSegment.MakeRoom(0, SegmentEndCount, out effectiveExtended);
                    Capacity += NewSegment.Capacity;
                    SegmentTable.Insert(segmentIndex + 1, NewSegment);
                }

                // Move the end of the current segment to the new one.
                if (SegmentEndCount > 0)
                    SegmentTable[segmentIndex].MoveTo(SegmentTable[segmentIndex + 1], 0, elementIndex, SegmentEndCount);

                // Check that we didn't do all this for nothing.
                Debug.Assert(elementIndex == SegmentTable[segmentIndex].Count);

                // Resume making room.
                Extendable = SegmentTable[segmentIndex].Extendable;
                if (Extendable > RemainingCount)
                    Extendable = (int)RemainingCount;
                SegmentTable[segmentIndex].MakeRoom(elementIndex, Extendable, out effectiveExtended);
                Capacity += effectiveExtended;
                RemainingCount -= Extendable;

                Debug.Assert(RemainingCount >= 0);

                segmentIndex++;

                if (segmentIndex < SegmentTable.Count)
                {
                    Extendable = SegmentTable[segmentIndex].Extendable;
                    if (Extendable > RemainingCount)
                        Extendable = (int)RemainingCount;
                    SegmentTable[segmentIndex].MakeRoom(0, Extendable, out effectiveExtended);
                    Capacity += effectiveExtended;
                    RemainingCount -= Extendable;

                    Debug.Assert(RemainingCount >= 0);
                }

                while (RemainingCount >= MaxSegmentCapacity)
                {
                    ISegment<T> NewSegment = CreateMaxCapacitySegment();
                    NewSegment.MakeRoom(0, MaxSegmentCapacity, out effectiveExtended);
                    Capacity += NewSegment.Capacity;

                    SegmentTable.Insert(segmentIndex, NewSegment);
                    RemainingCount -= MaxSegmentCapacity;

                    segmentIndex++;
                }

                if (RemainingCount > 0)
                {
                    ISegment<T> NewSegment = CreateSegment((int)RemainingCount);
                    NewSegment.MakeRoom(0, (int)RemainingCount, out effectiveExtended);
                    Capacity += NewSegment.Capacity;

                    SegmentTable.Insert(segmentIndex, NewSegment);
                }
            }

            Count += count;
            ResizeCache();

            RebuildCacheFrom(cacheIndex);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Replaces the element at the specified position with a new item.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the replaced element.</param>
        /// <param name="elementIndex">The element index of the position of the replaced element.</param>
        /// <param name="item">The item to set.</param>
        public void SetItem(int segmentIndex, int elementIndex, T item)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            SegmentTable[segmentIndex][elementIndex] = item;

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Replaces a range of elements at the specified position with new items from a collection.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of replaced elements.</param>
        /// <param name="elementIndex">The element index of the position of replaced elements.</param>
        /// <param name="collection">The collection containing items to set.</param>
        public void SetItemRange(int segmentIndex, int elementIndex, IEnumerable<T> collection)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            foreach (T item in collection)
            {
                Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

                SegmentTable[segmentIndex][elementIndex] = item;

                if (elementIndex + 1 < SegmentTable[segmentIndex].Count || segmentIndex + 1 >= SegmentTable.Count || SegmentTable[segmentIndex + 1].Count == 0)
                    elementIndex++;
                else
                {
                    segmentIndex++;
                    elementIndex = 0;
                }
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="Partition{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the <see cref="Partition{T}"/>.
        /// </returns>
        public bool Remove(T item)
        {
            bool Result = false;

            foreach (ISegment<T> Segment in SegmentTable)
                if (Segment.Remove(item))
                {
                    Result = true;
                    Count--;

                    if (Segment.Count == 0)
                    {
                        SegmentTable.Remove(Segment);
                        SegmentTable.Add(Segment);
                    }
                    break;
                }

            RebuildCache();

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to remove.</param>
        /// <param name="elementIndex">The element index of the position of the first element to remove.</param>
        /// <param name="cacheIndex">The cache index of the position of the first element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(int segmentIndex, int elementIndex, int cacheIndex, long count)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));
            Debug.Assert(cacheIndex >= 0);

            long RemainingCount = count;

            while (RemainingCount > 0)
            {
                Debug.Assert(segmentIndex >= 0 && segmentIndex < SegmentTable.Count);
                Debug.Assert(elementIndex >= 0 && elementIndex < SegmentTable[segmentIndex].Count);

                int Removable = SegmentTable[segmentIndex].Count - elementIndex;
                if (Removable > RemainingCount)
                    Removable = (int)RemainingCount;

                SegmentTable[segmentIndex].RemoveRange(elementIndex, Removable);

                if (SegmentTable[segmentIndex].Count > 0)
                    segmentIndex++;
                else
                {
                    ISegment<T> Segment = SegmentTable[segmentIndex];
                    SegmentTable.RemoveAt(segmentIndex);
                    SegmentTable.Add(Segment);
                }

                RemainingCount -= Removable;
                elementIndex = 0;
            }

            Debug.Assert(RemainingCount == 0);

            Count -= count;

            RebuildCacheFrom(cacheIndex);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the <see cref="Partition{T}"/>.
        /// </returns>
        public long RemoveAll(Predicate<T> match)
        {
            long RemovedCount = 0;

            int SegmentCount = SegmentTable.Count;
            int SegmentIndex = 0;
            while (SegmentIndex < SegmentCount)
            {
                ISegment<T> Segment = SegmentTable[SegmentIndex];

                RemovedCount += Segment.RemoveAll(match);

                if (Segment.Count == 0)
                {
                    SegmentTable.RemoveAt(SegmentIndex);
                    SegmentTable.Add(Segment);
                    SegmentCount--;
                }
                else
                    SegmentIndex++;
            }

            Count -= RemovedCount;
            RebuildCache();

#if DEBUG
            AssertInvariant();
#endif

            return RemovedCount;
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range of the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        public void Reverse(int segmentIndexBegin, int elementIndexBegin, int segmentIndexEnd, int elementIndexEnd, long count)
        {
            Debug.Assert(IsValidPosition(segmentIndexBegin, elementIndexBegin, true));
            Debug.Assert(IsValidPosition(segmentIndexEnd, elementIndexEnd, true));
            Debug.Assert(count >= 0);

            bool IsPartitionEmpty = count == 0 && segmentIndexBegin == segmentIndexEnd && elementIndexBegin == elementIndexEnd;
            bool IsSomeSegmentsFilled = segmentIndexBegin < segmentIndexEnd;
            bool IsAllSegmentsFilled = segmentIndexBegin == segmentIndexEnd && elementIndexBegin < elementIndexEnd;
            bool IsPartitionNotEmpty = count > 0 && (IsSomeSegmentsFilled || IsAllSegmentsFilled);
            bool IsPartitionValid = IsPartitionEmpty || IsPartitionNotEmpty;

            Debug.Assert(IsPartitionValid);

            for (long l = 0; l < count / 2; l++)
                ReverseLoop(ref segmentIndexBegin, ref elementIndexBegin, ref segmentIndexEnd, ref elementIndexEnd);

#if DEBUG
            AssertInvariant();
#endif
        }

        private void ReverseLoop(ref int segmentIndexBegin, ref int elementIndexBegin, ref int segmentIndexEnd, ref int elementIndexEnd)
        {
            if (elementIndexEnd > 0)
                elementIndexEnd--;
            else
            {
                segmentIndexEnd--;
                Debug.Assert(segmentIndexEnd >= 0 && SegmentTable[segmentIndexEnd].Count > 0);
                elementIndexEnd = SegmentTable[segmentIndexEnd].Count - 1;
            }

            Debug.Assert(segmentIndexBegin < segmentIndexEnd || (segmentIndexBegin == segmentIndexEnd && elementIndexBegin < elementIndexEnd));

            T item = SegmentTable[segmentIndexBegin][elementIndexBegin];
            SegmentTable[segmentIndexBegin][elementIndexBegin] = SegmentTable[segmentIndexEnd][elementIndexEnd];
            SegmentTable[segmentIndexEnd][elementIndexEnd] = item;

            if (elementIndexBegin + 1 < SegmentTable[segmentIndexBegin].Count)
                elementIndexBegin++;
            else
            {
                segmentIndexBegin++;
                Debug.Assert(segmentIndexBegin < SegmentTable.Count && SegmentTable[segmentIndexBegin].Count > 0);
                elementIndexBegin = 0;
            }
        }

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="Partition{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        public void Sort(int segmentIndexBegin, int elementIndexBegin, int segmentIndexEnd, int elementIndexEnd, long count, IComparer<T> comparer)
        {
            Debug.Assert(IsValidPosition(segmentIndexBegin, elementIndexBegin, true));
            Debug.Assert(IsValidPosition(segmentIndexEnd, elementIndexEnd, true));
            Debug.Assert((count == 0 && segmentIndexBegin == segmentIndexEnd && elementIndexBegin == elementIndexEnd) || (count > 0 && ((segmentIndexBegin < segmentIndexEnd) || (segmentIndexBegin == segmentIndexEnd && elementIndexBegin < elementIndexEnd))));
            Debug.Assert(comparer != null);

            if (count > 0)
            {
                Debug.Assert(QuickSortStack.Count == 0);

                if (elementIndexEnd > 0)
                    elementIndexEnd--;
                else
                {
                    segmentIndexEnd--;
                    Debug.Assert(segmentIndexEnd >= 0 && SegmentTable[segmentIndexEnd].Count > 0);
                    elementIndexEnd = SegmentTable[segmentIndexEnd].Count - 1;
                }

                QuickSortRange Range = new QuickSortRange() { SegmentIndexFirst = segmentIndexBegin, ElementIndexFirst = elementIndexBegin, SegmentIndexLast = segmentIndexEnd, ElementIndexLast = elementIndexEnd };
                QuickSortStack.Push(Range);

                while (QuickSortStack.Count > 0)
                    QuickSort(comparer);

                Debug.Assert(QuickSortStack.Count == 0);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        private Stack<QuickSortRange> QuickSortStack = new Stack<QuickSortRange>();
        #endregion

        #region Descendants Interface
        /// <summary>
        /// <para>Initializes the partition.</para>
        /// <para>This method is called once, from constructors, at the begining, after parameter validation.</para>
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Creates a new segment with initial capacity equal to the maximum capacity.
        /// </summary>
        /// <returns>
        /// The created segment.
        /// </returns>
        protected virtual ISegment<T> CreateMaxCapacitySegment()
        {
            return new Segment<T>(MaxSegmentCapacity);
        }

        /// <summary>
        /// Creates a new segment with the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        /// <returns>
        /// The created segment.
        /// </returns>
        protected virtual ISegment<T> CreateSegment(int initialCapacity)
        {
            return new Segment<T>(initialCapacity, MaxSegmentCapacity);
        }

        /// <summary>
        /// Creates an enumerator to iterate through the <see cref="Partition{T}"/> starting at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns>The enumerator.</returns>
        protected virtual IPartitionEnumerator<T> CreateEnumerator(int segmentIndex, int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            if (elementIndex >= SegmentTable[segmentIndex].Count)
                return new PartitionEnumerator<T>();
            else
            {
                Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));
                return new PartitionEnumerator<T>(this, segmentIndex, elementIndex);
            }
        }

        /// <summary>
        /// Removes segments starting from the specified index in the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the first segment to remove.</param>
        /// <param name="count">The number of segments to remove.</param>
        protected virtual void RemoveSegmentRange(int index, int count)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(index + count <= SegmentTable.Count);

            SegmentTable.RemoveRange(index, count);
        }
        #endregion

        #region Cache
        private struct CacheLine
        {
            public int SegmentIndex;
            public long Min;

            public override string ToString()
            {
                return SegmentIndex.ToString() + "," + Min;
            }
        }

        private void InitCache()
        {
            RebuildCache();
        }

        private void RebuildCache()
        {
            Debug.Assert(SegmentTable.Count > 0);

            CacheLineExponent = HighestExponentAbove(MaxSegmentCapacity) - 1;
            CacheLineLength = 1 << CacheLineExponent;

            ResizeCache();

            Cache[0].SegmentIndex = 0;
            Cache[0].Min = 0;
            Debug.Assert(Cache[0].ToString() != null); // For code coverage.

            RebuildCacheFrom(0);
        }

        private void ResizeCache()
        {
            CacheLineCount = (int)(Count / CacheLineLength) + 1;

            if (Cache == null || CacheLineCount > Cache.Length)
                Array.Resize(ref Cache, CacheLineCount);
        }

        private void RebuildCacheFrom(int cacheLine)
        {
            Debug.Assert(cacheLine < CacheLineCount);

            int SegmentIndex = Cache[cacheLine].SegmentIndex;
            long Min = Cache[cacheLine].Min;

            Debug.Assert(SegmentIndex < SegmentTable.Count);

            while (cacheLine + 1 < CacheLineCount)
            {
                cacheLine++;

                long CacheIndex = cacheLine * CacheLineLength;

                while (Min + SegmentTable[SegmentIndex].Count < CacheIndex)
                {
                    if (SegmentTable[SegmentIndex].Count == 0 || SegmentIndex + 1 == SegmentTable.Count)
                        break;

                    Min += SegmentTable[SegmentIndex].Count;
                    SegmentIndex++;
                }

                Debug.Assert(SegmentIndex < SegmentTable.Count);

                Cache[cacheLine].SegmentIndex = SegmentIndex;
                Cache[cacheLine].Min = Min;
            }
        }

        private int HighestExponentAbove(long n)
        {
            Debug.Assert(n >= 0);

            int Exponent = 0;
            long i = n;

            while (i > 0)
            {
                Exponent++;
                i >>= 1;
            }

            Debug.Assert((1 << Exponent) >= n);

            return Exponent;
        }

        private void PositionFromCache(long index, out int segmentIndex, out int elementIndex, out int cacheIndex)
        {
            cacheIndex = (int)(index >> CacheLineExponent);

            if (cacheIndex >= CacheLineCount)
            {
                segmentIndex = SegmentTable.Count - 1;
                elementIndex = SegmentTable[SegmentTable.Count - 1].Count;
                cacheIndex = CacheLineCount - 1;
                return;
            }

            Debug.Assert(cacheIndex < CacheLineCount);

            segmentIndex = Cache[cacheIndex].SegmentIndex;
            long LongElementIndex = index - Cache[cacheIndex].Min;

            while (((LongElementIndex > SegmentTable[segmentIndex].Count) || (LongElementIndex == SegmentTable[segmentIndex].Count && index < Count)) && SegmentTable[segmentIndex].Count > 0 && segmentIndex + 1 < SegmentTable.Count)
            {
                LongElementIndex -= SegmentTable[segmentIndex].Count;
                segmentIndex++;
            }

            Debug.Assert(LongElementIndex <= SegmentTable[segmentIndex].Capacity);

            if (LongElementIndex == SegmentTable[segmentIndex].Capacity && SegmentTable[segmentIndex].Capacity > 0)
            {
                Debug.Assert(SegmentTable[segmentIndex].Count == SegmentTable[segmentIndex].Capacity);

                if (SegmentTable[segmentIndex].Capacity == MaxSegmentCapacity && segmentIndex + 1 < SegmentTable.Count)
                {
                    LongElementIndex -= SegmentTable[segmentIndex].Count;
                    segmentIndex++;
                    Debug.Assert(LongElementIndex == 0);
                }
            }

            elementIndex = (int)LongElementIndex;
        }

        private CacheLine[] Cache;
        private int CacheLineExponent;
        private int CacheLineLength;
        private int CacheLineCount;
        #endregion

        private List<ISegment<T>> SegmentTable;

        #region Contracts
        private void AssertInvariant()
        {
            Debug.Assert(SegmentTable.Count > 0);
            Debug.Assert(Capacity <= SegmentTable.Count * (long)MaxSegmentCapacity);
            Debug.Assert(Count <= Capacity);

            BreakIfNotDebugging();

            long TotalCapacity = 0;
            long TotalCount = 0;
            bool IsPreviousSegmentVoid = false;
            bool IsPreviousSegmentEmpty = false;
            foreach (ISegment<T> Segment in SegmentTable)
            {
                if (Segment.Capacity == 0)
                    IsPreviousSegmentVoid = true;
                else
                {
                    Debug.Assert(!IsPreviousSegmentVoid);
                    TotalCapacity += Segment.Capacity;
                }

                if (Segment.Count == 0)
                    IsPreviousSegmentEmpty = true;
                else
                {
                    Debug.Assert(!IsPreviousSegmentVoid);
                    Debug.Assert(!IsPreviousSegmentEmpty);
                    TotalCount += Segment.Count;
                }
            }

            Debug.Assert(Capacity == TotalCapacity);
            Debug.Assert(Count == TotalCount);

            Debug.Assert(Cache[0].SegmentIndex == 0);
            Debug.Assert(Cache[0].Min == 0);

            int n = 0;
            while (n < Count)
            {
                int SegmentIndex;
                int ElementIndex;
                int CacheIndex;
                PositionFromCache(n, out SegmentIndex, out ElementIndex, out CacheIndex);

                TotalCount = 0;
                for (int s = 0; s < SegmentIndex; s++)
                    TotalCount += SegmentTable[s].Count;

                TotalCount += ElementIndex;
                Debug.Assert(TotalCount == n);

                n += CacheLineLength;
            }
        }

        private bool IsItemEqual(long index, T item)
        {
            BreakIfNotDebugging();

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            return Equals(GetItem(SegmentIndex, ElementIndex), item);
        }

        private bool IsItemNull(long index)
        {
            BreakIfNotDebugging();

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            return GetItem(SegmentIndex, ElementIndex) == null;
        }

        private bool IsPositionEqual(long index, int segmentIndex, int elementIndex)
        {
            BreakIfNotDebugging();

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            return SegmentIndex == segmentIndex && ElementIndex == elementIndex;
        }

        private void BreakIfNotDebugging()
        {
#if DEBUG
#else
            Debugger.Break(); // This method should not be called in release mode.
#endif
        }
        #endregion
    }
}
