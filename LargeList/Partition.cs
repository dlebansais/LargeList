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
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but must remain constant in a given Partition&lt;T&gt;.
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
        /// Gets the position of the first element in the IPartition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The position of the first element in the IPartition&lt;T&gt;.
        /// </returns>
        ElementPosition Begin { get; }

        /// <summary>
        /// Gets the position of an element in the IPartition&lt;T&gt; from its virtual index in a linear list.
        /// </summary>
        /// <param name="index">The virtual index of the element.</param>
        /// <returns>
        /// The position of the element in the IPartition&lt;T&gt;.
        /// </returns>
        ElementPosition PositionOf(long index);

        /// <summary>
        /// Returns an enumerator for the IPartition&lt;T&gt;, starting from the specified position.
        /// </summary>
        /// <param name="position">The position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the IPartition&lt;T&gt;, starting from the element specified by <paramref name="position"/>.
        /// </returns>
        IPartitionEnumerator<T> GetEnumerator(ElementPosition position);

        /// <summary>
        /// Gets the previous position in the IPartition&lt;T&gt;. The returned position may be invalid if <paramref name="position"/> is the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this interface.
        /// </summary>
        /// <param name="position">The position used as starting point.</param>
        /// <returns>
        /// The position in the IPartition&lt;T&gt; that precedes <paramref name="position"/>. The position preceding IPartition&lt;T&gt;.Begin may be returned, but must not be used in subsequent calls.
        /// </returns>
        ElementPosition PreviousPosition(ElementPosition position);

        /// <summary>
        /// Gets the next position in the IPartition&lt;T&gt;. <paramref name="position"/> must not be IPartition&lt;T&gt;.End.
        /// </summary>
        /// <param name="position">The position used as starting point.</param>
        /// <returns>
        /// The position in the IPartition&lt;T&gt; that follows <paramref name="position"/>.
        /// </returns>
        ElementPosition NextPosition(ElementPosition position);

        /// <summary>
        /// Gets the next segment in the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="segment">The segment used as starting point.</param>
        /// <returns>
        /// The segment in the IPartition&lt;T&gt; that follows <paramref name="segment"/>, null if <paramref name="segment"/> was the last one in the IPartition&lt;T&gt;.
        /// </returns>
        ISegment<T> NextSegment(ISegment<T> segment);

        /// <summary>
        /// Gets the element in the IPartition&lt;T&gt; at the specified position.
        /// </summary>
        /// <param name="position">The position of the element.</param>
        /// <returns>
        /// The element in the IPartition&lt;T&gt; specified by <paramref name="position"/>.
        /// </returns>
        T GetItem(ElementPosition position);

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
        /// Makes room for a number of elements starting at the specified position. Elements already the specified position and beyond are moved toward the end of the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="position">The position at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        void MakeRoom(ElementPosition position, long count);

        /// <summary>
        /// Replaces the element at the specified position with a new item.
        /// </summary>
        /// <param name="position">The position of the replaced element.</param>
        /// <param name="item">The item to set.</param>
        void SetItem(ElementPosition position, T item);

        /// <summary>
        /// Replaces a range of elements at the specified position with new items from a collection.
        /// </summary>
        /// <param name="position">The position of replaced elements.</param>
        /// <param name="collection">The collection containing items to set.</param>
        void SetItemRange(ElementPosition position, IEnumerable<T> collection);

        /// <summary>
        /// Removes the first occurrence of a specific object from the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the IPartition&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the IPartition&lt;T&gt;.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the IPartition&lt;T&gt;.
        /// </returns>
        long RemoveAll(Predicate<T> match);

        /// <summary>
        /// Removes a range of elements from the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="position">The position of the first element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        void RemoveRange(ElementPosition position, long count);

        /// <summary>
        /// Reverses the order of the elements in the specified range of the IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="begin">The position of the first item in the range.</param>
        /// <param name="end">The position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        void Reverse(ElementPosition begin, ElementPosition end, long count);

        /// <summary>
        /// Sorts the elements in a range of elements in IPartition&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="begin">The position of the first item in the range.</param>
        /// <param name="end">The position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        void Sort(ElementPosition begin, ElementPosition end, long count, IComparer<T> comparer);
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
        /// Gets the number of elements contained in the IPartition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the IPartition&lt;T&gt;.
        /// </returns>
        public long Count { get; private set; }

        /// <summary>
        /// Gets the position of the first element in the Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The position of the first element in the Partition&lt;T&gt;.
        /// </returns>
        public ElementPosition Begin
        {
            get
            {
                return new ElementPosition(0, 0, 0);
            }
        }
        #endregion

        #region Queries
        /// <summary>
        /// Gets the position of an element in the Partition&lt;T&gt; from its virtual index in a linear list.
        /// </summary>
        /// <param name="index">The virtual index of the element.</param>
        /// <returns>
        /// The position of the element in the Partition&lt;T&gt;.
        /// </returns>
        public ElementPosition PositionOf(long index)
        {
            Debug.Assert(index >= 0 && index <= Count);

            ElementPosition Result = PositionOfFromCache(index);

            Debug.Assert(IsValidPosition(Result, true));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Gets the element in the Partition&lt;T&gt; at the specified position.
        /// </summary>
        /// <param name="position">The position of the element.</param>
        /// <returns>
        /// The element in the Partition&lt;T&gt; specified by <paramref name="position"/>.
        /// </returns>
        public T GetItem(ElementPosition position)
        {
            Debug.Assert(IsValidPosition(position, false));

            T Result = SegmentTable[position.SegmentIndex][position.ElementIndex];

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Returns an enumerator for the Partition&lt;T&gt;, starting from the specified position.
        /// </summary>
        /// <param name="position">The position of the first element to enumerate.</param>
        /// <returns>
        /// An enumerator that can iterate through the Partition&lt;T&gt;, starting from the element specified by <paramref name="position"/>.
        /// </returns>
        public IPartitionEnumerator<T> GetEnumerator(ElementPosition position)
        {
            Debug.Assert(IsValidPosition(position, true));

            IPartitionEnumerator<T> Result = CreateEnumerator(position);

            Debug.Assert(Result != null);

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Creates an enumerator to iterate through the Partition&lt;T&gt; starting at the specified position.
        /// </summary>
        /// <param name="position">Position of the first element to enumerate.</param>
        /// <returns></returns>
        protected IPartitionEnumerator<T> CreateEnumerator(ElementPosition position)
        {
            Debug.Assert(IsValidPosition(position, true));

            if (IsValidPosition(position, false))
            {
                ISegment<T> segment = SegmentTable[position.SegmentIndex];
                int index = position.ElementIndex;

                return new PartitionEnumerator<T>(segment, index);
            }
            else
                return new PartitionEnumerator<T>();
        }

        /// <summary>
        /// Gets the previous position in the Partition&lt;T&gt;. The returned position may be invalid if <paramref name="position"/> is the first element. In that case, the caller should not use the returned position in subsequent calls to methods of this class.
        /// </summary>
        /// <param name="position">The position used as starting point.</param>
        /// <returns>
        /// The position in the Partition&lt;T&gt; that precedes <paramref name="position"/>. The position preceding Partition&lt;T&gt;.Begin may be returned, but must not be used in subsequent calls.
        /// </returns>
        public ElementPosition PreviousPosition(ElementPosition position)
        {
            Debug.Assert(IsValidPosition(position, true));

            ElementPosition Result;

            if (position.ElementIndex > 0)
                Result = new ElementPosition(position.SegmentIndex, position.ElementIndex - 1, -1);

            else
            {
                int SegmentIndex = position.SegmentIndex;
                do
                    SegmentIndex--;
                while (SegmentIndex >= 0 && SegmentTable[SegmentIndex].Count == 0);

                if (SegmentIndex >= 0)
                    Result = new ElementPosition(SegmentIndex, SegmentTable[SegmentIndex].Count - 1, -1);
                else
                    Result = new ElementPosition(SegmentIndex, 0, -1);
            }

            Debug.Assert(IsValidPosition(Result, false) || (Result.SegmentIndex == -1 && Result.ElementIndex == 0));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Gets the next position in the Partition&lt;T&gt;. <paramref name="position"/> must not be Partition&lt;T&gt;.End.
        /// </summary>
        /// <param name="position">The position used as starting point.</param>
        /// <returns>
        /// The position in the Partition&lt;T&gt; that follows <paramref name="position"/>.
        /// </returns>
        public ElementPosition NextPosition(ElementPosition position)
        {
            Debug.Assert(IsValidPosition(position, false));

            ElementPosition Result;

            if (position.ElementIndex + 1 < SegmentTable[position.SegmentIndex].Count)
                Result = new ElementPosition(position.SegmentIndex, position.ElementIndex + 1, -1);

            else
            {
                int NextSegmentIndex = position.SegmentIndex + 1;
                while (NextSegmentIndex < SegmentTable.Count && SegmentTable[NextSegmentIndex].Count == 0)
                    NextSegmentIndex++;

                if (NextSegmentIndex < SegmentTable.Count)
                    Result = new ElementPosition(NextSegmentIndex, 0, -1);
                else
                    Result = new ElementPosition(position.SegmentIndex, position.ElementIndex + 1, -1);
            }

            Debug.Assert(IsValidPosition(Result, true));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Gets the next segment in the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="segment">The segment used as starting point.</param>
        /// <returns>
        /// The segment in the Partition&lt;T&gt; that follows <paramref name="segment"/>, null if <paramref name="segment"/> was the last one in the Partition&lt;T&gt;.
        /// </returns>
        public ISegment<T> NextSegment(ISegment<T> segment)
        {
            Debug.Assert(SegmentTable.Contains(segment));

            ISegment<T> Result;

            int NextContentIndex = SegmentTable.IndexOf(segment) + 1;
            Debug.Assert(NextContentIndex >= 0 && NextContentIndex <= SegmentTable.Count);

            if (NextContentIndex < SegmentTable.Count)
                Result = SegmentTable[NextContentIndex];
            else
                Result = null;

            Debug.Assert(Result == null || SegmentTable.Contains(Result));

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

            int SegmentIndex = 0;
            long ElementStartIndex = startIndex;
            long ItemIndex = 0;

            for (;;)
            {
                Debug.Assert(ElementStartIndex >= 0);
                Debug.Assert(SegmentIndex < SegmentTable.Count);

                ISegment<T> Segment = SegmentTable[SegmentIndex];

                if (ElementStartIndex < Segment.Count)
                    break;

                if (Segment.Count > 0)
                {
                    if (SegmentIndex + 1 >= SegmentTable.Count)
                        break;

                    ElementStartIndex -= Segment.Count;
                    ItemIndex += Segment.Count;
                    SegmentIndex++;
                }
                else
                {
                    int NextSegmentIndex = SegmentIndex + 1;
                    while (NextSegmentIndex < SegmentTable.Count && SegmentTable[NextSegmentIndex].Count == 0)
                        NextSegmentIndex++;

                    if (NextSegmentIndex >= SegmentTable.Count)
                        break;

                    SegmentIndex = NextSegmentIndex;
                }
            }

            while (SegmentIndex < SegmentTable.Count && count > 0)
            {
                ISegment<T> Segment = SegmentTable[SegmentIndex];

                int CompareCount = (Segment.Count - (int)ElementStartIndex <= count) ? Segment.Count - (int)ElementStartIndex : (int)count;
                int ElementIndex = Segment.IndexOf(item, (int)ElementStartIndex, CompareCount);
                if (ElementIndex >= 0)
                {
                    Result = ItemIndex + ElementIndex;
                    break;
                }

                ElementStartIndex = 0;
                count -= CompareCount;
                ItemIndex += Segment.Count;
                SegmentIndex++;
            }

            Debug.Assert(count >= 0);
            Debug.Assert(Result == -1 || Result >= 0 && Result < Count && GetItem(PositionOf(Result)).Equals(item));

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

            ElementPosition position = PositionOf(startIndex);
            int SegmentIndex = position.SegmentIndex;
            int ElementStartIndex = position.ElementIndex;
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
                Segment = null;

                do
                    SegmentIndex--;
                while (SegmentIndex >= 0 && SegmentTable[SegmentIndex].Count == 0);

                if (SegmentIndex < 0 || count == 0)
                    break;

                Segment = SegmentTable[SegmentIndex];
                ElementStartIndex = Segment.Count - 1;
            }

            Debug.Assert(count >= 0);
            Debug.Assert(Result == -1 || (Result >= 0 && Result < Count && ((item == null && GetItem(PositionOf(Result)) == null) || (item != null && item.Equals(GetItem(PositionOf(Result)))))));

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
            long indexUpper = index + count - 1;
            ElementPosition lower = PositionOf(indexLower);
            ElementPosition upper = PositionOf(indexUpper);

            while (lower <= upper)
            {
                if (indexLower == 9 && indexUpper == 10)
                    indexUpper = 10;

                ElementPosition middle;
                long indexMiddle;
                GetMiddleOf(lower, indexLower, upper, indexUpper, out middle, out indexMiddle);

                Debug.Assert(lower <= middle);
                Debug.Assert(indexLower <= indexMiddle);
                Debug.Assert(upper >= middle);
                Debug.Assert(indexUpper >= indexMiddle);

                long comparisonResult = comparer.Compare(item, GetItem(middle));

                if (comparisonResult == 0)
                    return indexMiddle;

                else if (comparisonResult < 0)
                {
                    upper = PreviousPosition(middle);
                    indexUpper = indexMiddle - 1;
                }
                else
                {
                    lower = NextPosition(middle);
                    indexLower = indexMiddle + 1;
                }
            }

            return -(indexLower + 1);
        }

        /// <summary>
        /// Gets the position in the middle of two positions, assuming <paramref name="lower"/> is lesser than or equal to <paramref name="upper"/>.
        /// </summary>
        /// <param name="lower">The lower position.</param>
        /// <param name="indexLower">The virtual index corresponding to <paramref name="lower"/>.</param>
        /// <param name="upper">The upper position.</param>
        /// <param name="indexUpper">The virtual index corresponding to <paramref name="upper"/>.</param>
        /// <param name="middle">The result as position.</param>
        /// <param name="indexMiddle">The result as virtual index.</param>
        /// <returns>
        /// The position in the middle of <paramref name="lower"/> and <paramref name="upper"/>.
        /// </returns>
        private void GetMiddleOf(ElementPosition lower, long indexLower, ElementPosition upper, long indexUpper, out ElementPosition  middle, out long indexMiddle)
        {
            Debug.Assert(lower <= upper);
            Debug.Assert(indexLower <= indexUpper);

            while (lower.SegmentIndex < upper.SegmentIndex)
            {
                int AboveLower = SegmentTable[lower.SegmentIndex].Count - lower.ElementIndex - 1;
                int BelowUpper = upper.ElementIndex;
                int Difference = BelowUpper - AboveLower;

                if (Difference > 0)
                {
                    int SegmentIndex = lower.SegmentIndex;

                    do
                        SegmentIndex++;
                    while (SegmentIndex < upper.SegmentIndex && SegmentTable[SegmentIndex].Count == 0);

                    Debug.Assert(SegmentTable[SegmentIndex].Count > 0);

                    indexLower += AboveLower + 1;
                    indexUpper -= AboveLower + 1;
                    lower = new ElementPosition(SegmentIndex, 0, -1);
                    upper = new ElementPosition(upper.SegmentIndex, Difference - 1, -1);
                }

                else if (Difference < 0)
                {
                    int SegmentIndex = upper.SegmentIndex;

                    do
                        SegmentIndex--;
                    while (SegmentIndex > lower.SegmentIndex && SegmentTable[SegmentIndex].Count == 0);

                    Debug.Assert(SegmentTable[SegmentIndex].Count > 0);

                    indexLower += BelowUpper + 1;
                    indexUpper -= BelowUpper + 1;
                    lower = new ElementPosition(lower.SegmentIndex, lower.ElementIndex + BelowUpper + 1, -1);
                    upper = new ElementPosition(SegmentIndex, SegmentTable[SegmentIndex].Count - 1, -1);
                }

                else
                {
                    int LowerSegmentIndex = lower.SegmentIndex;
                    int UpperSegmentIndex = upper.SegmentIndex;

                    do
                        LowerSegmentIndex++;
                    while (LowerSegmentIndex < upper.SegmentIndex && SegmentTable[LowerSegmentIndex].Count == 0);

                    do
                        UpperSegmentIndex--;
                    while (UpperSegmentIndex > lower.SegmentIndex && SegmentTable[UpperSegmentIndex].Count == 0);

                    Debug.Assert(SegmentTable[LowerSegmentIndex].Count > 0);
                    Debug.Assert(SegmentTable[UpperSegmentIndex].Count > 0);

                    indexLower += BelowUpper + 1;
                    indexUpper -= BelowUpper + 1;
                    lower = new ElementPosition(LowerSegmentIndex, 0, -1);
                    upper = new ElementPosition(UpperSegmentIndex, SegmentTable[UpperSegmentIndex].Count - 1, -1);
                }
            }

            if (lower.SegmentIndex > upper.SegmentIndex)
            {
                Debug.Assert(lower.ElementIndex == 0);
                Debug.Assert(upper.SegmentIndex >= 0 && upper.SegmentIndex < SegmentTable.Count);
                Debug.Assert(upper.ElementIndex == SegmentTable[upper.SegmentIndex].Count - 1);
                Debug.Assert(indexUpper + 1 == indexLower);
                Debug.Assert(PositionOf(indexUpper) == upper);
                Debug.Assert(PositionOf(indexLower) == lower);

                middle = upper;
                indexMiddle = indexUpper;
            }
            else
            {
                Debug.Assert(lower.SegmentIndex == upper.SegmentIndex);
                Debug.Assert(lower.ElementIndex <= upper.ElementIndex);

                int middleElementIndex = lower.ElementIndex + (upper.ElementIndex - lower.ElementIndex) / 2;

                middle = new ElementPosition(lower.SegmentIndex, middleElementIndex, -1);
                indexMiddle = indexLower - lower.ElementIndex + middleElementIndex;
            }

            Debug.Assert(PositionOf(indexMiddle) == middle);
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
        /// <param name="position">The position at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        public void MakeRoom(ElementPosition position, long count)
        {
            Debug.Assert(IsValidPosition(position, true));
            Debug.Assert(count >= 0);

            int SegmentIndex = position.SegmentIndex;
            int ElementIndex = position.ElementIndex;
            long RemainingCount = count;
            int effectiveExtended;

            // First we try to make room in just one segment.
            int Extendable = SegmentTable[SegmentIndex].Extendable;
            if (Extendable >= RemainingCount)
            {
                SegmentTable[SegmentIndex].MakeRoom(ElementIndex, (int)RemainingCount, out effectiveExtended);
                Capacity += effectiveExtended;
            }

            else
            {
                // Otherwise, we're going to need to move elements upward.
                int SegmentEndCount = SegmentTable[SegmentIndex].Count - ElementIndex;
                int NextExtendable = (SegmentIndex + 1 < SegmentTable.Count) ? SegmentTable[SegmentIndex + 1].Extendable : -1;

                Debug.Assert(SegmentEndCount >= 0);
                Debug.Assert(ElementIndex + SegmentEndCount + RemainingCount > MaxSegmentCapacity);

                // If there is room for elements in the next segment, use it.
                if (SegmentEndCount <= NextExtendable)
                {
                    Debug.Assert(NextExtendable >= 0);
                    Debug.Assert(SegmentIndex + 1 < SegmentTable.Count);
                    Debug.Assert(SegmentTable[SegmentIndex + 1].Count + SegmentEndCount <= MaxSegmentCapacity);

                    SegmentTable[SegmentIndex + 1].MakeRoom(0, SegmentEndCount, out effectiveExtended);
                    Capacity += effectiveExtended;
                }

                // Otherwise, perform a split.
                else if (SegmentEndCount > 0)
                {
                    ISegment<T> NewSegment = CreateSegment(SegmentEndCount);
                    NewSegment.MakeRoom(0, SegmentEndCount, out effectiveExtended);
                    Capacity += NewSegment.Capacity;
                    SegmentTable.Insert(SegmentIndex + 1, NewSegment);
                }

                // Move the end of the current segment to the new one.
                if (SegmentEndCount > 0)
                    SegmentTable[SegmentIndex].MoveTo(SegmentTable[SegmentIndex + 1], 0, ElementIndex, SegmentEndCount);

                // Check that we didn't do all this for nothing.
                Debug.Assert(ElementIndex == SegmentTable[SegmentIndex].Count);

                // Resume making room.
                Extendable = SegmentTable[SegmentIndex].Extendable;
                if (Extendable > RemainingCount)
                    Extendable = (int)RemainingCount;
                SegmentTable[SegmentIndex].MakeRoom(ElementIndex, Extendable, out effectiveExtended);
                Capacity += effectiveExtended;
                RemainingCount -= Extendable;

                Debug.Assert(RemainingCount >= 0);

                SegmentIndex++;

                if (SegmentIndex < SegmentTable.Count)
                {
                    Extendable = SegmentTable[SegmentIndex].Extendable;
                    if (Extendable > RemainingCount)
                        Extendable = (int)RemainingCount;
                    SegmentTable[SegmentIndex].MakeRoom(0, Extendable, out effectiveExtended);
                    Capacity += effectiveExtended;
                    RemainingCount -= Extendable;

                    Debug.Assert(RemainingCount >= 0);
                }

                while (RemainingCount >= MaxSegmentCapacity)
                {
                    ISegment<T> NewSegment = CreateMaxCapacitySegment();
                    NewSegment.MakeRoom(0, MaxSegmentCapacity, out effectiveExtended);
                    Capacity += NewSegment.Capacity;

                    SegmentTable.Insert(SegmentIndex, NewSegment);
                    RemainingCount -= MaxSegmentCapacity;

                    SegmentIndex++;
                }

                if (RemainingCount > 0)
                {
                    ISegment<T> NewSegment = CreateSegment((int)RemainingCount);
                    NewSegment.MakeRoom(0, (int)RemainingCount, out effectiveExtended);
                    Capacity += NewSegment.Capacity;

                    SegmentTable.Insert(SegmentIndex, NewSegment);
                }
            }

            Count += count;
            ResizeCache();

            Debug.Assert(position.CacheIndex >= 0);
            RebuildCacheFrom(position.CacheIndex);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Replaces the element at the specified position with a new item.
        /// </summary>
        /// <param name="position">The position of the replaced element.</param>
        /// <param name="item">The item to set.</param>
        public void SetItem(ElementPosition position, T item)
        {
            Debug.Assert(IsValidPosition(position, false));

            SegmentTable[position.SegmentIndex][position.ElementIndex] = item;

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Replaces a range of elements at the specified position with new items from a collection.
        /// </summary>
        /// <param name="position">The position of replaced elements.</param>
        /// <param name="collection">The collection containing items to set.</param>
        public void SetItemRange(ElementPosition position, IEnumerable<T> collection)
        {
            Debug.Assert(IsValidPosition(position, true));

            int SegmentIndex = position.SegmentIndex;
            int ElementIndex = position.ElementIndex;

            foreach (T item in collection)
            {
                Debug.Assert(IsValidPosition(position, false));

                SegmentTable[position.SegmentIndex][position.ElementIndex] = item;
                position = NextPosition(position);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes a range of elements from the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="position">The position of the first element to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(ElementPosition position, long count)
        {
            Debug.Assert(IsValidPosition(position, true));

            int SegmentIndex = position.SegmentIndex;
            int ElementIndex = position.ElementIndex;
            long RemainingCount = count;

            while (RemainingCount > 0)
            {
                Debug.Assert(SegmentIndex >= 0 && SegmentIndex < SegmentTable.Count);

                if (SegmentTable[SegmentIndex].Count == 0)
                {
                    SegmentIndex++;
                    continue;
                }

                Debug.Assert(ElementIndex >= 0 && ElementIndex < SegmentTable[SegmentIndex].Count);

                int Removable = SegmentTable[SegmentIndex].Count - ElementIndex;
                if (Removable > RemainingCount)
                    Removable = (int)RemainingCount;

                SegmentTable[SegmentIndex].RemoveRange(ElementIndex, Removable);

                if (SegmentTable[SegmentIndex].Count > 0)
                    SegmentIndex++;
                else
                {
                    ISegment<T> Segment = SegmentTable[SegmentIndex];
                    SegmentTable.RemoveAt(SegmentIndex);
                    SegmentTable.Add(Segment);
                }

                RemainingCount -= Removable;
                ElementIndex = 0;
            }

            Debug.Assert(RemainingCount == 0);

            Count -= count;

            Debug.Assert(position.CacheIndex >= 0);
            RebuildCacheFrom(position.CacheIndex);

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
        /// <param name="begin">The position of the first item in the range.</param>
        /// <param name="end">The position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        public void Reverse(ElementPosition begin, ElementPosition end, long count)
        {
            Debug.Assert(IsValidPosition(begin, true));
            Debug.Assert(IsValidPosition(end, true));
            Debug.Assert(count >= 0);
            Debug.Assert((count == 0 && begin == end) || (count > 0 && begin < end));

            for (long l = 0; l < count / 2; l++)
            {
                end = PreviousPosition(end);

                Debug.Assert(begin < end);

                SwapItem(begin, end);
                begin = NextPosition(begin);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Sorts the elements in a range of elements in Partition&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="begin">The position of the first item in the range.</param>
        /// <param name="end">The position after the last item in the range.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        public void Sort(ElementPosition begin, ElementPosition end, long count, IComparer<T> comparer)
        {
            Debug.Assert(IsValidPosition(begin, true));
            Debug.Assert(IsValidPosition(end, true));
            Debug.Assert((count == 0 && begin == end) || (count > 0 && begin < end));
            Debug.Assert(comparer != null);

            if (count > 0)
            {
                Debug.Assert(QuickSortStack.Count == 0);

                QuickSortRange Range = new QuickSortRange() { FirstElement = begin, LastElement = PreviousPosition(end) };
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
            public ElementPosition FirstElement;
            public ElementPosition LastElement;
        }

        private void QuickSort(IComparer<T> comparer)
        {
            Debug.Assert(comparer != null);

            QuickSortRange Range = QuickSortStack.Pop();
            ElementPosition low = Range.FirstElement;
            ElementPosition high = Range.LastElement;

            Debug.Assert(IsValidPosition(low, false));
            Debug.Assert(IsValidPosition(high, false));
            Debug.Assert(low <= high);

            if (low.SegmentIndex == high.SegmentIndex)
            {
                ISegment<T> Segment = SegmentTable[low.SegmentIndex];
                Segment.Sort(low.ElementIndex, high.ElementIndex, comparer);
            }
            else if (low < high)
            {
                ElementPosition middle = SplitSortInterval(low, high, comparer);

                QuickSortRange RangeLow = new QuickSortRange() { FirstElement = low, LastElement = middle };
                QuickSortRange RangeHigh = new QuickSortRange() { FirstElement = NextPosition(middle), LastElement = high };

                QuickSortStack.Push(RangeLow);
                QuickSortStack.Push(RangeHigh);
            }
        }

        private ElementPosition SplitSortInterval(ElementPosition low, ElementPosition high, IComparer<T> comparer)
        {
            Debug.Assert(IsValidPosition(low, false));
            Debug.Assert(IsValidPosition(high, false));
            Debug.Assert(comparer != null);
            Debug.Assert(low < high);

            T pivot = SelectPivot(low, high);
            ElementPosition up = PreviousPosition(low);
            ElementPosition down = NextPosition(high);

            for (;;)
            {
                do
                    if (up.SegmentIndex < 0)
                        up = PositionOf(0);
                    else
                        up = NextPosition(up);
                while (comparer.Compare(SegmentTable[up.SegmentIndex][up.ElementIndex], pivot) < 0);

                do
                    down = PreviousPosition(down);
                while (comparer.Compare(SegmentTable[down.SegmentIndex][down.ElementIndex], pivot) > 0);

                if (up >= down)
                    return down;

                SwapItem(up, down);
            }
        }

        private T SelectPivot(ElementPosition low, ElementPosition high)
        {
            Debug.Assert(IsValidPosition(low, false));
            Debug.Assert(IsValidPosition(high, false));
            Debug.Assert(low < high);

            int PivotSegmentIndex = (low.SegmentIndex + high.SegmentIndex) / 2;
            while (PivotSegmentIndex > low.SegmentIndex && SegmentTable[PivotSegmentIndex].Count == 0)
                PivotSegmentIndex--;
            while (PivotSegmentIndex < high.SegmentIndex && SegmentTable[PivotSegmentIndex].Count == 0)
                PivotSegmentIndex++;

            Debug.Assert(PivotSegmentIndex >= low.SegmentIndex && PivotSegmentIndex <= high.SegmentIndex && SegmentTable[PivotSegmentIndex].Count > 0);

            int PivotElementIndex = (low.ElementIndex + high.ElementIndex) / 2;
            if (PivotSegmentIndex == low.SegmentIndex && PivotElementIndex < low.ElementIndex)
                PivotElementIndex = low.ElementIndex;
            if (PivotElementIndex >= SegmentTable[PivotSegmentIndex].Count)
                PivotElementIndex = SegmentTable[PivotSegmentIndex].Count - 1;

            return SegmentTable[PivotSegmentIndex][PivotElementIndex];
        }

        private void SwapItem(ElementPosition p1, ElementPosition p2)
        {
            Debug.Assert(IsValidPosition(p1, false));
            Debug.Assert(IsValidPosition(p2, false));

            T item = SegmentTable[p1.SegmentIndex][p1.ElementIndex];
            SegmentTable[p1.SegmentIndex][p1.ElementIndex] = SegmentTable[p2.SegmentIndex][p2.ElementIndex];
            SegmentTable[p2.SegmentIndex][p2.ElementIndex] = item;
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

        private ElementPosition PositionOfFromCache(long index)
        {
            int CacheIndex = (int)(index >> CacheLineExponent);

            if (CacheIndex >= CacheLineCount)
                return new ElementPosition(SegmentTable.Count - 1, SegmentTable[SegmentTable.Count - 1].Count, CacheLineCount - 1);

            Debug.Assert(CacheIndex < CacheLineCount);

            int SegmentIndex = Cache[CacheIndex].SegmentIndex;
            long ElementIndex = index - Cache[CacheIndex].Min;

            while (((ElementIndex > SegmentTable[SegmentIndex].Count) || (ElementIndex == SegmentTable[SegmentIndex].Count && index < Count)) && SegmentTable[SegmentIndex].Count > 0 && SegmentIndex + 1 < SegmentTable.Count)
            {
                ElementIndex -= SegmentTable[SegmentIndex].Count;
                SegmentIndex++;
            }

            Debug.Assert(ElementIndex <= SegmentTable[SegmentIndex].Capacity);

            if (ElementIndex == SegmentTable[SegmentIndex].Capacity && SegmentTable[SegmentIndex].Capacity > 0)
            {
                Debug.Assert(SegmentTable[SegmentIndex].Count == SegmentTable[SegmentIndex].Capacity);

                if (SegmentTable[SegmentIndex].Capacity == MaxSegmentCapacity && SegmentIndex + 1 < SegmentTable.Count)
                {
                    ElementIndex -= SegmentTable[SegmentIndex].Count;
                    SegmentIndex++;
                    Debug.Assert(ElementIndex == 0);
                }
            }

            return new ElementPosition(SegmentIndex, (int)ElementIndex, CacheIndex);
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
                ElementPosition p = PositionOfFromCache(n);

                TotalCount = 0;
                for (int s = 0; s < p.SegmentIndex; s++)
                    TotalCount += SegmentTable[s].Count;

                TotalCount += p.ElementIndex;
                Debug.Assert(TotalCount == n);

                n += CacheLineLength;
            }
        }

        private bool IsValidPosition(ElementPosition position, bool AllowEnd)
        {
            if (position.SegmentIndex < 0 || position.SegmentIndex >= SegmentTable.Count)
                return false;

            if (position.ElementIndex < SegmentTable[position.SegmentIndex].Count)
                return true;

            if (!AllowEnd)
                return false;

            int SegmentIndex = position.SegmentIndex;
            while (SegmentIndex  + 1 < SegmentTable.Count)
                if (SegmentTable[++SegmentIndex].Count != 0)
                    return false;

            if (position.ElementIndex == SegmentTable[position.SegmentIndex].Count)
                return true;

            return false;
        }
        #endregion
    }
}
