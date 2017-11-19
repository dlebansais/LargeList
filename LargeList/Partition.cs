using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
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
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but must remain constant in a given IPartition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The maximum capacity allowed for segments.
        /// </returns>
        int MaxSegmentCapacity { get; }

        /// <summary>
        /// Gets the total number of elements the IPartition&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the IPartition&lt;T&gt; can hold without resizing.
        /// </returns>
        long Capacity { get; }

        /// <summary>
        /// Gets the number of elements contained in the IPartition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the IPartition&lt;T&gt;.
        /// </returns>
        long Count { get; }

        /// <summary>
        /// Gets the position of an element in the IPartition&lt;T&gt; from its virtual index in a linear list.
        /// </summary>
        /// <param name="index">The virtual index of the element.</param>
        /// <param name="segmentIndex">Upon return, the segment index of the element.</param>
        /// <param name="elementIndex">Upon return, the element index of the element.</param>
        /// <param name="cacheIndex">Upon return, the cache index of the element.</param>
        void GetPosition(long index, out int segmentIndex, out int elementIndex, out int cacheIndex);

        /// <summary>
        /// Check that the specified position in the IPartition&lt;T&gt; is valid. Calling this method is reserved to debugging.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position to check.</param>
        /// <param name="elementIndex">The element index of the position to check.</param>
        /// <param name="allowEnd">True to allow the IPartition&lt;T&gt;.End position; False to only allow position of existing elements.</param>
        /// <returns>
        /// True if the position in the IPartition&lt;T&gt; specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> is valid.
        /// </returns>
        bool IsValidPosition(int segmentIndex, int elementIndex, bool allowEnd);

        /// <summary>
        /// Gets the previous position in the IPartition&lt;T&gt;. The returned position may be invalid if <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> specify the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this interface.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexPrevious">Upon return, the segment index of the previous position.</param>
        /// <param name="elementIndexPrevious">Upon return, the element index of the previous position.</param>
        void GetPreviousPosition(int segmentIndex, int elementIndex, out int segmentIndexPrevious, out int elementIndexPrevious);

        /// <summary>
        /// Gets the next position in the IPartition&lt;T&gt;. <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> must specify the position of an existing element, or the position that is before the first element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position used as starting point.</param>
        /// <param name="elementIndex">The element index of the position used as starting point.</param>
        /// <param name="segmentIndexNext">Upon return, the segment index of the next position.</param>
        /// <param name="elementIndexNext">Upon return, the element index of the next position.</param>
        void GetNextPosition(int segmentIndex, int elementIndex, out int segmentIndexNext, out int elementIndexNext);

        /// <summary>
        /// Updates a position in the IPartition&lt;T&gt; to the previous element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        void DecrementPosition(ref int segmentIndex, ref int elementIndex);

        /// <summary>
        /// Updates a position in the IPartition&lt;T&gt; to the next element.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position.</param>
        /// <param name="elementIndex">The element index of the position.</param>
        void IncrementPosition(ref int segmentIndex, ref int elementIndex);

        /// <summary>
        /// Gets the element in the IPartition&lt;T&gt; at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the element.</param>
        /// <param name="elementIndex">The element index of the position of the element.</param>
        /// <returns>
        /// The element in the IPartition&lt;T&gt; specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        T GetItem(int segmentIndex, int elementIndex);

        /// <summary>
        /// Returns an enumerator for the IPartition&lt;T&gt;, starting from the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the IPartition&lt;T&gt;, starting from the element specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
        /// </returns>
        IPartitionEnumerator<T> GetEnumerator(int segmentIndex, int elementIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the specified ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <param name="remainingCount">Upon return, the remaining number of elements that can be enumerated in the ISegment&lt;T&gt;.</param>
        /// <returns>
        /// An enumerator for the ISegment&lt;T&gt;.
        /// </returns>
        IEnumerator<T> GetSegmentEnumerator(int segmentIndex, int elementIndex, out int remainingCount);

        /// <summary>
        /// Gets the next segment in the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment used as starting point.</param>
        /// <returns>
        /// The index of the segment in the IPartition&lt;T&gt; that follows <paramref name="segmentIndex"/>, -1 if <paramref name="segmentIndex"/> specified the last one in the IPartition&lt;T&gt;.
        /// </returns>
        int NextSegmentIndex(int segmentIndex);

        /// <summary>
        /// Determines whether an element is in the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the IPartition&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the IPartition&lt;T&gt;; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the first occurrence in a linear list that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the IPartition&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty partition.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the first occurrence of <paramref name="item"/> within the range of elements in a linear list that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        long IndexOf(T item, long startIndex, long count);

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the last occurrence within the range of elements in a linear list that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the IPartition&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the last occurrence of <paramref name="item"/> within the range of elements in the linear list that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        long LastIndexOf(T item, long startIndex, long count);

        /// <summary>
        /// Searches a range of elements in the sorted IPartition&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted IPartition&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of IPartition&lt;T&gt;.Count.
        /// </returns>
        long BinarySearch(long index, long count, T item, IComparer<T> comparer);

        /// <summary>
        /// Removes all elements from the IPartition&lt;T&gt;.
        /// </summary>
        void Clear();

        /// <summary>
        /// Increases the IPartition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="extended">The number of elements added to the IPartition&lt;T&gt;.Capacity.</param>
        void ExtendCapacity(long extended);

        /// <summary>
        /// Decreases the IPartition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="trimed">The number of elements substracted to the IPartition&lt;T&gt;.Capacity.</param>
        void TrimCapacity(long trimed);

        /// <summary>
        /// Makes room for a number of elements starting at the specified position. Elements already the specified position and beyond are moved toward the end of the IPartition&lt;T&gt;.
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
        /// Removes the first occurrence of a specific object from the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the IPartition&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the IPartition&lt;T&gt;.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes a range of elements from the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to remove.</param>
        /// <param name="elementIndex">The element index of the position of the first element to remove.</param>
        /// <param name="cacheIndex">The cache index of the position of the first element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        void RemoveRange(int segmentIndex, int elementIndex, int cacheIndex, long count);

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the IPartition&lt;T&gt;.
        /// </returns>
        long RemoveAll(Predicate<T> match);

        /// <summary>
        /// Reverses the order of the elements in the specified range of the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        void Reverse(int segmentIndexBegin, int elementIndexBegin, int segmentIndexEnd, int elementIndexEnd, long count);

        /// <summary>
        /// Sorts the elements in a range of elements in IPartition&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
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
    class Partition<T> : IPartition<T>
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the Partition&lt;T&gt; class with the specified initial number of uninitialized elements and capacity.
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

#if DEBUG
            AssertInvariant();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but remains constant in a given Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The maximum capacity allowed for segments.
        /// </returns>
        public int MaxSegmentCapacity { get; private set; }

        /// <summary>
        /// Gets the total number of elements the Partition&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the Partition&lt;T&gt; can hold without resizing.
        /// </returns>
        public long Capacity { get; private set; }

        /// <summary>
        /// Gets the number of elements contained in the Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the Partition&lt;T&gt;.
        /// </returns>
        public long Count { get; private set; }
        #endregion

        #region Queries
        /// <summary>
        /// Gets the position of an element in the Partition&lt;T&gt; from its virtual index in a linear list.
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
        /// Check that the specified position in the Partition&lt;T&gt; is valid. Calling this method is reserved to debugging.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position to check.</param>
        /// <param name="elementIndex">The element index of the position to check.</param>
        /// <param name="allowEnd">True to allow the Partition&lt;T&gt;.End position; False to only allow position of existing elements.</param>
        /// <returns>
        /// True if the position in the Partition&lt;T&gt; specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> is valid.
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
        /// Gets the previous position in the Partition&lt;T&gt;. The returned position may be invalid if <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> specify the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this interface.
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
        /// Gets the next position in the Partition&lt;T&gt;. <paramref name="segmentIndex"/> and <paramref name="elementIndex"/> must specify the position of an existing element, or the position that is before the first element.
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
        /// Updates a position in the Partition&lt;T&gt; to the previous element.
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
        /// Updates a position in the Partition&lt;T&gt; to the next element.
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
        /// Gets the element in the Partition&lt;T&gt; at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the element.</param>
        /// <param name="elementIndex">The element index of the position of the element.</param>
        /// <returns>
        /// The element in the Partition&lt;T&gt; specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
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
        /// Returns an enumerator for the Partition&lt;T&gt;, starting from the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the Partition&lt;T&gt;, starting from the element specified by <paramref name="segmentIndex"/> and <paramref name="elementIndex"/>.
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
        /// Returns an enumerator that iterates through the specified ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <param name="remainingCount">Upon return, the remaining number of elements that can be enumerated in the ISegment&lt;T&gt;.</param>
        /// <returns>
        /// An enumerator for the ISegment&lt;T&gt;.
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
        /// Gets the next segment in the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment used as starting point.</param>
        /// <returns>
        /// The index of the segment in the Partition&lt;T&gt; that follows <paramref name="segmentIndex"/>, -1 if <paramref name="segmentIndex"/> specified the last one in the Partition&lt;T&gt;.
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

        /// <summary>
        /// Determines whether an element is in the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the Partition&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the Partition&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            bool Result = false;

            foreach (ISegment<T> Segment in SegmentTable)
                if (Segment.Contains(item))
                {
                    Result = true;
                    break;
                }

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the first occurrence in a linear list that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the Partition&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty partition.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the first occurrence of <paramref name="item"/> within the range of elements in a linear list that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        public long IndexOf(T item, long startIndex, long count)
        {
            Debug.Assert(startIndex >= 0 && startIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + count <= Count);

            long Result = -1;

            int SegmentIndex;
            int ElementStartIndex;
            int CacheIndex;
            GetPosition(startIndex, out SegmentIndex, out ElementStartIndex, out CacheIndex);

            long ItemIndex = startIndex - ElementStartIndex;
            long RemainingCount = count;

            while (SegmentIndex < SegmentTable.Count && RemainingCount > 0)
            {
                ISegment<T> Segment = SegmentTable[SegmentIndex];
                if (Segment.Count == 0)
                    break;

                int CompareCount = (Segment.Count - ElementStartIndex <= RemainingCount) ? Segment.Count - ElementStartIndex : (int)RemainingCount;
                int ElementIndex = Segment.IndexOf(item, ElementStartIndex, CompareCount);
                if (ElementIndex >= 0)
                {
                    Result = ItemIndex + ElementIndex;
                    break;
                }

                ElementStartIndex = 0;
                RemainingCount -= CompareCount;
                ItemIndex += Segment.Count;
                SegmentIndex++;
            }

            Debug.Assert(RemainingCount >= 0);
            Debug.Assert(Result == -1 || (Result >= startIndex && Result < startIndex + count && IsItemEqual(Result, item)));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based virtual index of the last occurrence within the range of elements in a linear list that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the Partition&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based virtual index of the last occurrence of <paramref name="item"/> within the range of elements in the linear list that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        public long LastIndexOf(T item, long startIndex, long count)
        {
            Debug.Assert(startIndex >= 0 && startIndex < Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + 1 >= count);

            long Result = -1;

            int SegmentIndex;
            int ElementStartIndex;
            int CacheIndex;
            GetPosition(startIndex, out SegmentIndex, out ElementStartIndex, out CacheIndex);

            long ItemIndex = startIndex;
            ISegment<T> Segment = SegmentTable[SegmentIndex];

            for(;;)
            {
                Debug.Assert(SegmentIndex >= 0 && SegmentIndex < SegmentTable.Count && Segment == SegmentTable[SegmentIndex]);
                Debug.Assert(ElementStartIndex >= 0 && ElementStartIndex < Segment.Count);
                Debug.Assert(count >= 0);

                int CompareCount = (ElementStartIndex < count) ? ElementStartIndex + 1 : (int)count;
                int ElementIndex = Segment.LastIndexOf(item, ElementStartIndex, CompareCount);
                if (ElementIndex >= 0)
                {
                    Result = ItemIndex - ElementStartIndex + ElementIndex;
                    break;
                }

                count -= CompareCount;
                ItemIndex -= CompareCount;
                SegmentIndex--;

                Debug.Assert(SegmentIndex < 0 || SegmentTable[SegmentIndex].Count > 0);

                if (SegmentIndex < 0 || count == 0)
                    break;

                Segment = SegmentTable[SegmentIndex];
                ElementStartIndex = Segment.Count - 1;
            }

            Debug.Assert(count >= 0);
            Debug.Assert(Result == -1 || (Result >= 0 && Result < Count && ((item == null && IsItemNull(Result)) || (item != null && IsItemEqual(Result, item)))));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Searches a range of elements in the sorted Partition&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted Partition&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of Partition&lt;T&gt;.Count.
        /// </returns>
        public long BinarySearch(long index, long count, T item, IComparer<T> comparer)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(comparer != null);
            Debug.Assert(index + count <= Count);

            if (count == 0)
                return -(index + 1);

            long indexLower = index;
            int SegmentIndexLower;
            int ElementIndexLower;
            int CacheIndexLower;
            GetPosition(indexLower, out SegmentIndexLower, out ElementIndexLower, out CacheIndexLower);

            long indexUpper = index + count - 1;
            int SegmentIndexUpper;
            int ElementIndexUpper;
            int CacheIndexUpper;
            GetPosition(indexUpper, out SegmentIndexUpper, out ElementIndexUpper, out CacheIndexUpper);

            while (SegmentIndexLower < SegmentIndexUpper || (SegmentIndexLower == SegmentIndexUpper && ElementIndexLower <= ElementIndexUpper))
            {
                long indexMiddle;
                int SegmentIndexMiddle;
                int ElementIndexMiddle;
                GetMiddleOf(SegmentIndexLower, ElementIndexLower, indexLower, SegmentIndexUpper, ElementIndexUpper, indexUpper, out SegmentIndexMiddle, out ElementIndexMiddle, out indexMiddle);

                Debug.Assert(SegmentIndexLower < SegmentIndexMiddle || (SegmentIndexLower == SegmentIndexMiddle && ElementIndexLower <= ElementIndexMiddle));
                Debug.Assert(indexLower <= indexMiddle);
                Debug.Assert(SegmentIndexUpper > SegmentIndexMiddle || (SegmentIndexUpper == SegmentIndexMiddle && ElementIndexUpper >= ElementIndexMiddle));
                Debug.Assert(indexUpper >= indexMiddle);

                long comparisonResult = comparer.Compare(item, SegmentTable[SegmentIndexMiddle][ElementIndexMiddle]);

                if (comparisonResult == 0)
                    return indexMiddle;

                else if (comparisonResult < 0)
                {
                    GetPreviousPosition(SegmentIndexMiddle, ElementIndexMiddle, out SegmentIndexUpper, out ElementIndexUpper);
                    indexUpper = indexMiddle - 1;
                }
                else
                {
                    GetNextPosition(SegmentIndexMiddle, ElementIndexMiddle, out SegmentIndexLower, out ElementIndexLower);
                    indexLower = indexMiddle + 1;
                }
            }

            return -(indexLower + 1);
        }

        /// <summary>
        /// Gets the middle of two positions.
        /// </summary>
        /// <param name="segmentIndexLower">Segment index of the lower position.</param>
        /// <param name="elementIndexLower">Element index of the lower position.</param>
        /// <param name="indexLower">The virtual index corresponding to the lower position.</param>
        /// <param name="segmentIndexUpper">Segment index of the upper position.</param>
        /// <param name="elementIndexUpper">Element index of the upper position.</param>
        /// <param name="indexUpper">The virtual index corresponding to the upper position.</param>
        /// <param name="segmentIndexMiddle">Upon return, the segment index of the middle position.</param>
        /// <param name="elementIndexMiddle">Upon return, the element index of the middle position.</param>
        /// <param name="indexMiddle">Upon return, he resulting middle position as virtual index.</param>
        private void GetMiddleOf(int segmentIndexLower, int elementIndexLower, long indexLower, int segmentIndexUpper, int elementIndexUpper, long indexUpper, out int segmentIndexMiddle, out int elementIndexMiddle, out long indexMiddle)
        {
            Debug.Assert(segmentIndexLower < segmentIndexUpper || (segmentIndexLower == segmentIndexUpper && elementIndexLower <= elementIndexUpper));
            Debug.Assert(indexLower <= indexUpper);

            while (segmentIndexLower < segmentIndexUpper)
            {
                int AboveLower = SegmentTable[segmentIndexLower].Count - elementIndexLower - 1;
                int BelowUpper = elementIndexUpper;
                int Difference = BelowUpper - AboveLower;

                if (Difference > 0)
                {
                    Debug.Assert(SegmentTable[segmentIndexLower + 1].Count > 0);

                    indexLower += AboveLower + 1;
                    indexUpper -= AboveLower + 1;
                    segmentIndexLower++;
                    elementIndexLower = 0;
                    elementIndexUpper = Difference - 1;
                }

                else if (Difference < 0)
                {
                    Debug.Assert(SegmentTable[segmentIndexUpper - 1].Count > 0);

                    indexLower += BelowUpper + 1;
                    indexUpper -= BelowUpper + 1;
                    elementIndexLower += BelowUpper + 1;
                    segmentIndexUpper--;
                    elementIndexUpper = SegmentTable[segmentIndexUpper].Count - 1;
                }

                else
                {
                    int LowerSegmentIndex = segmentIndexLower + 1;
                    int UpperSegmentIndex = segmentIndexUpper - 1;

                    Debug.Assert(SegmentTable[segmentIndexLower + 1].Count > 0);
                    Debug.Assert(SegmentTable[segmentIndexUpper - 1].Count > 0);

                    indexLower += BelowUpper + 1;
                    indexUpper -= BelowUpper + 1;
                    segmentIndexLower++;
                    elementIndexLower = 0;
                    segmentIndexUpper--;
                    elementIndexUpper = SegmentTable[segmentIndexUpper].Count - 1;
                }
            }

            if (segmentIndexLower > segmentIndexUpper)
            {
                Debug.Assert(elementIndexLower == 0);
                Debug.Assert(segmentIndexUpper >= 0 && segmentIndexUpper < SegmentTable.Count);
                Debug.Assert(elementIndexUpper == SegmentTable[segmentIndexUpper].Count - 1);
                Debug.Assert(indexUpper + 1 == indexLower);
                Debug.Assert(IsPositionEqual(indexUpper, segmentIndexUpper, elementIndexUpper));
                Debug.Assert(IsPositionEqual(indexLower, segmentIndexLower, elementIndexLower));

                segmentIndexMiddle = segmentIndexUpper;
                elementIndexMiddle = elementIndexUpper;
                indexMiddle = indexUpper;
            }
            else
            {
                Debug.Assert(segmentIndexLower == segmentIndexUpper);
                Debug.Assert(elementIndexLower <= elementIndexUpper);

                int Middle = elementIndexLower + (elementIndexUpper - elementIndexLower) / 2;

                segmentIndexMiddle = segmentIndexLower;
                elementIndexMiddle = Middle;
                indexMiddle = indexLower - elementIndexLower + Middle;
            }

            Debug.Assert(IsPositionEqual(indexMiddle, segmentIndexMiddle, elementIndexMiddle));
        }
#endregion

        #region Commands
        /// <summary>
        /// Removes all elements from the Partition&lt;T&gt;.
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
        /// Increases the Partition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="extended">The number of elements added to the Partition&lt;T&gt;.Capacity.</param>
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
        /// Decreases the Partition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="trimed">The number of elements substracted to the Partition&lt;T&gt;.Capacity.</param>
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
        /// Makes room for a number of elements starting at the specified position. Elements already the specified position and beyond are moved toward the end of the Partition&lt;T&gt;.
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
        /// Removes the first occurrence of a specific object from the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the Partition&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the Partition&lt;T&gt;.
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
        /// Removes a range of elements from the Partition&lt;T&gt;.
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
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the Partition&lt;T&gt;.
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
        /// Reverses the order of the elements in the specified range of the Partition&lt;T&gt;.
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
            Debug.Assert((count == 0 && segmentIndexBegin == segmentIndexEnd && elementIndexBegin == elementIndexEnd) || (count > 0 && ((segmentIndexBegin < segmentIndexEnd) || (segmentIndexBegin == segmentIndexEnd && elementIndexBegin < elementIndexEnd))));

            for (long l = 0; l < count / 2; l++)
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

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Sorts the elements in a range of elements in Partition&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="segmentIndexBegin">The segment index of the position of the first item in the range.</param>
        /// <param name="elementIndexBegin">The element index of the position of the first item in the range.</param>
        /// <param name="segmentIndexEnd">The segment index of the position after the last item in the range.</param>
        /// <param name="elementIndexEnd">The element index of the position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
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

        #region Sorting
        private struct QuickSortRange
        {
            public int SegmentIndexFirst;
            public int ElementIndexFirst;
            public int SegmentIndexLast;
            public int ElementIndexLast;
        }

        private void QuickSort(IComparer<T> comparer)
        {
            Debug.Assert(comparer != null);

            QuickSortRange Range = QuickSortStack.Pop();
            int SegmentIndexLow = Range.SegmentIndexFirst;
            int ElementIndexLow = Range.ElementIndexFirst;
            int SegmentIndexHigh = Range.SegmentIndexLast;
            int ElementIndexHigh = Range.ElementIndexLast;

            Debug.Assert(IsValidPosition(SegmentIndexLow, ElementIndexLow, false));
            Debug.Assert(IsValidPosition(SegmentIndexHigh, ElementIndexHigh, false));
            Debug.Assert(SegmentIndexLow < SegmentIndexHigh || (SegmentIndexLow == SegmentIndexHigh && ElementIndexLow <= ElementIndexHigh));

            if (SegmentIndexLow == SegmentIndexHigh)
            {
                ISegment<T> Segment = SegmentTable[SegmentIndexLow];
                Segment.Sort(ElementIndexLow, ElementIndexHigh, comparer);
            }
            else if (SegmentIndexLow < SegmentIndexHigh || (SegmentIndexLow == SegmentIndexHigh && ElementIndexLow < ElementIndexHigh))
            {
                int SegmentIndexMiddle;
                int ElementIndexMiddle;
                SplitSortInterval(SegmentIndexLow, ElementIndexLow, SegmentIndexHigh, ElementIndexHigh, comparer, out SegmentIndexMiddle, out ElementIndexMiddle);

                QuickSortRange RangeLow = new QuickSortRange() { SegmentIndexFirst = SegmentIndexLow, ElementIndexFirst = ElementIndexLow, SegmentIndexLast = SegmentIndexMiddle, ElementIndexLast = ElementIndexMiddle };

                if (ElementIndexMiddle + 1 < SegmentTable[SegmentIndexMiddle].Count)
                    ElementIndexMiddle++;

                else
                {
                    SegmentIndexMiddle++;
                    Debug.Assert(SegmentIndexMiddle < SegmentTable.Count && SegmentTable[SegmentIndexMiddle].Count > 0);
                    ElementIndexMiddle = 0;
                }

                QuickSortRange RangeHigh = new QuickSortRange() { SegmentIndexFirst = SegmentIndexMiddle, ElementIndexFirst = ElementIndexMiddle, SegmentIndexLast = SegmentIndexHigh, ElementIndexLast = ElementIndexHigh };

                QuickSortStack.Push(RangeLow);
                QuickSortStack.Push(RangeHigh);
            }
        }

        private void SplitSortInterval(int segmentIndexLow, int elementIndexLow, int segmentIndexHigh, int elementIndexHigh, IComparer<T> comparer, out int segmentIndexMiddle, out int elementIndexMiddle)
        {
            Debug.Assert(IsValidPosition(segmentIndexLow, elementIndexLow, false));
            Debug.Assert(IsValidPosition(segmentIndexHigh, elementIndexHigh, false));
            Debug.Assert(comparer != null);
            Debug.Assert(segmentIndexLow < segmentIndexHigh || (segmentIndexLow == segmentIndexHigh && elementIndexLow < elementIndexHigh));

            T pivot = SelectPivot(segmentIndexLow, elementIndexLow, segmentIndexHigh, elementIndexHigh);

            int SegmentIndexUp;
            int ElementIndexUp;
            int SegmentIndexDown;
            int ElementIndexDown;

            GetPreviousPosition(segmentIndexLow, elementIndexLow, out SegmentIndexUp, out ElementIndexUp);
            GetNextPosition(segmentIndexHigh, elementIndexHigh, out SegmentIndexDown, out ElementIndexDown);

            for (;;)
            {
                do
                    if (SegmentIndexUp < 0)
                    {
                        SegmentIndexUp = 0;
                        ElementIndexUp = 0;
                    }
                    else
                    {
                        if (ElementIndexUp + 1 < SegmentTable[SegmentIndexUp].Count)
                            ElementIndexUp++;

                        else
                        {
                            SegmentIndexUp++;
                            Debug.Assert(SegmentIndexUp < SegmentTable.Count && SegmentTable[SegmentIndexUp].Count > 0);
                            ElementIndexUp = 0;
                        }
                    }
                while (comparer.Compare(SegmentTable[SegmentIndexUp][ElementIndexUp], pivot) < 0);

                do
                {
                    if (ElementIndexDown > 0)
                        ElementIndexDown--;

                    else
                    {
                        SegmentIndexDown--;
                        Debug.Assert(SegmentIndexDown >= 0 && SegmentTable[SegmentIndexDown].Count > 0);
                        ElementIndexDown = SegmentTable[SegmentIndexDown].Count -  1;
                    }
                }
                while (comparer.Compare(SegmentTable[SegmentIndexDown][ElementIndexDown], pivot) > 0);

                if (SegmentIndexUp > SegmentIndexDown || (SegmentIndexUp == SegmentIndexDown && ElementIndexUp >= ElementIndexDown))
                {
                    segmentIndexMiddle = SegmentIndexDown;
                    elementIndexMiddle = ElementIndexDown;
                    return;
                }

                T item = SegmentTable[SegmentIndexUp][ElementIndexUp];
                SegmentTable[SegmentIndexUp][ElementIndexUp] = SegmentTable[SegmentIndexDown][ElementIndexDown];
                SegmentTable[SegmentIndexDown][ElementIndexDown] = item;
            }
        }

        private T SelectPivot(int segmentIndexLow, int elementIndexLow, int segmentIndexHigh, int elementIndexHigh)
        {
            Debug.Assert(IsValidPosition(segmentIndexLow, elementIndexLow, false));
            Debug.Assert(IsValidPosition(segmentIndexHigh, elementIndexHigh, false));
            Debug.Assert(segmentIndexLow < segmentIndexHigh || (segmentIndexLow == segmentIndexHigh && elementIndexLow < elementIndexHigh));

            int PivotSegmentIndex = (segmentIndexLow + segmentIndexHigh) / 2;
            Debug.Assert(PivotSegmentIndex >= segmentIndexLow && PivotSegmentIndex <= segmentIndexHigh && SegmentTable[PivotSegmentIndex].Count > 0);

            int PivotElementIndex;

            if (PivotSegmentIndex == segmentIndexLow && PivotSegmentIndex < segmentIndexHigh)
                PivotElementIndex = SegmentTable[PivotSegmentIndex].Count - 1;
            else if (PivotSegmentIndex == segmentIndexHigh && PivotSegmentIndex > segmentIndexLow)
                PivotElementIndex = 0;
            else
            {
                Debug.Assert((PivotSegmentIndex > segmentIndexLow && PivotSegmentIndex < segmentIndexHigh) || (segmentIndexLow == segmentIndexHigh));
                PivotElementIndex = SegmentTable[PivotSegmentIndex].Count / 2;
            }

            return SegmentTable[PivotSegmentIndex][PivotElementIndex];
        }
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
        protected ISegment<T> CreateMaxCapacitySegment()
        {
            return new Segment<T>(MaxSegmentCapacity);
        }

        /// <summary>
        /// Creates a new segment with the specified initial capacity.
        /// </summary>
        /// <returns>
        /// The created segment.
        /// </returns>
        protected ISegment<T> CreateSegment(int initialCapacity)
        {
            return new Segment<T>(initialCapacity, MaxSegmentCapacity);
        }

        /// <summary>
        /// Creates an enumerator to iterate through the Partition&lt;T&gt; starting at the specified position.
        /// </summary>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        /// <returns></returns>
        protected IPartitionEnumerator<T> CreateEnumerator(int segmentIndex, int elementIndex)
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
        /// Removes the specified segment from the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="segment">The segment to remove.</param>
        protected void RemoveSegment(ISegment<T> segment)
        {
            Debug.Assert(SegmentTable.Contains(segment));

            RemoveSegmentAt(SegmentTable.IndexOf(segment));
        }

        /// <summary>
        /// Removes the segment at the specified index from the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the segment to remove.</param>
        protected void RemoveSegmentAt(int index)
        {
            Debug.Assert(index >= 0 && index < SegmentTable.Count);

            SegmentTable.RemoveAt(index);
        }

        /// <summary>
        /// Removes segments starting from the specified index in the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the first segment to remove.</param>
        /// <param name="count">The number of segments to remove.</param>
        protected void RemoveSegmentRange(int index, int count)
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

            RebuildCacheFrom(0);
        }

        private void ResizeCache()
        {
            CacheLineCount = (int)(Count / CacheLineLength) + 1;

            if (Cache == null || CacheLineCount > Cache.Length)
                Array.Resize(ref Cache, CacheLineCount);
        }

        private void RebuildCacheFrom(int CacheLine)
        {
            Debug.Assert(CacheLine < CacheLineCount);

            int SegmentIndex = Cache[CacheLine].SegmentIndex;
            long Min = Cache[CacheLine].Min;

            Debug.Assert(SegmentIndex < SegmentTable.Count);

            while (CacheLine + 1 < CacheLineCount)
            {
                CacheLine++;

                long CacheIndex = CacheLine * CacheLineLength;

                while (Min + SegmentTable[SegmentIndex].Count < CacheIndex)
                {
                    if (SegmentTable[SegmentIndex].Count == 0 || SegmentIndex + 1 == SegmentTable.Count)
                        break;

                    Min += SegmentTable[SegmentIndex].Count;
                    SegmentIndex++;
                }

                Debug.Assert(SegmentIndex < SegmentTable.Count);

                Cache[CacheLine].SegmentIndex = SegmentIndex;
                Cache[CacheLine].Min = Min;
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

            return GetItem(SegmentIndex, ElementIndex).Equals(item);
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
