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
        /// Gets the total number of elements the IPartition&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the IPartition&lt;T&gt; can hold without resizing.
        /// </returns>
        long Capacity { get; }

        /// <summary>
        /// For debug purpose only.
        /// </summary>
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
        /// Gets a position one step beyond the last element in the IPartition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// A position one step beyond the last element in the IPartition&lt;T&gt;.
        /// </returns>
        ElementPosition End { get; }

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of Partition<T>.Initialize and Partition<T>.MaxSegmentCapacity")]
        public Partition(long capacity, long count)
        {
            Debug.Assert(capacity >= 0);
            Debug.Assert(count >= 0);
            Debug.Assert(count <= capacity);

            Initialize();

            SegmentTable = new List<ISegment<T>>();
            ISegment<T> NewSegment;
            long InitialCount = count;

            while (capacity > MaxSegmentCapacity)
            {
                long SegmentCount = count;
                if (SegmentCount > MaxSegmentCapacity)
                    SegmentCount = MaxSegmentCapacity;

                NewSegment = CreateMaxCapacitySegment();
                NewSegment.MakeRoom(0, (int)SegmentCount);
                SegmentTable.Add(NewSegment);

                capacity -= MaxSegmentCapacity;
                count -= SegmentCount;
            }

            NewSegment = CreateSegment((int)capacity);
            NewSegment.MakeRoom(0, (int)count);
            SegmentTable.Add(NewSegment);

            Debug.Assert(Capacity >= capacity);
            long MaxCapacity = (long)SegmentTable.Count * (long)MaxSegmentCapacity;
            Debug.Assert(Capacity <= MaxCapacity); // Tolerate a higher capacity than requested.
            Debug.Assert(Count == InitialCount);
            AssertInvariant();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the maximum capacity allowed for segments. This number can vary from partition to partition but must remain constant in a given Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The maximum capacity allowed for segments.
        /// </returns>
        protected virtual int MaxSegmentCapacity { get { return 10; } }

        /// <summary>
        /// Gets the total number of elements the Partition&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The total number of elements the Partition&lt;T&gt; can hold without resizing.
        /// </returns>
        public long Capacity
        {
            get
            {
                long Result = 0;
                foreach (ISegment<T> Segment in SegmentTable)
                    Result += Segment.Capacity;

                return Result;
            }
        }

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
                int SegmentIndex = 0;

                while (SegmentIndex < SegmentTable.Count && SegmentTable[SegmentIndex].Count == 0)
                    SegmentIndex++;

                if (SegmentIndex < SegmentTable.Count)
                    return new ElementPosition(SegmentIndex, 0);
                else
                    return new ElementPosition(0, 0);
            }
        }

        /// <summary>
        /// Gets a position immediately after the last element in the Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The position immediately after the last element in the Partition&lt;T&gt;.
        /// </returns>
        public ElementPosition End
        {
            get { return new ElementPosition(SegmentTable.Count - 1, SegmentTable[SegmentTable.Count - 1].Count); }
        }

        /// <summary>
        /// For debug purpose only.
        /// </summary>
        public long Count
        {
            get
            {
                long Result = 0;
                foreach (ISegment<T> Segment in SegmentTable)
                    Result += Segment.Count;

                return Result;
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

            int SegmentIndex = 0;
            for(;;)
            {
                Debug.Assert(index >= 0);
                Debug.Assert(SegmentIndex < SegmentTable.Count);

                ISegment<T> Segment = SegmentTable[SegmentIndex];

                if (index < Segment.Count || (SegmentIndex + 1 == SegmentTable.Count && index == Segment.Count))
                    break;

                index -= Segment.Count;
                SegmentIndex++;
            }

            ElementPosition Result = new ElementPosition(SegmentIndex, (int)index);

            Debug.Assert(IsValidPosition(Result, true));
            AssertInvariant();

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

            AssertInvariant();

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
            AssertInvariant();

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

            if (position == End)
                return new PartitionEnumerator<T>();
            else
            {
                ISegment<T> segment = SegmentTable[position.SegmentIndex];
                int index = position.ElementIndex;

                return new PartitionEnumerator<T>(segment, index);
            }
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
                Result = new ElementPosition(position.SegmentIndex, position.ElementIndex - 1);

            else
            {
                int SegmentIndex = position.SegmentIndex;
                do
                    SegmentIndex--;
                while (SegmentIndex >= 0 && SegmentTable[SegmentIndex].Count == 0);

                if (SegmentIndex >= 0)
                    Result = new ElementPosition(SegmentIndex, SegmentTable[SegmentIndex].Count - 1);
                else
                    Result = new ElementPosition(SegmentIndex, 0);
            }

            Debug.Assert(IsValidPosition(Result, false) || (Result.SegmentIndex == -1 && Result.ElementIndex == 0));
            AssertInvariant();

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
                Result = new ElementPosition(position.SegmentIndex, position.ElementIndex + 1);

            else
            {
                int SegmentIndex = position.SegmentIndex;
                do
                    SegmentIndex++;
                while (SegmentIndex < SegmentTable.Count && SegmentTable[SegmentIndex].Count == 0);

                if (SegmentIndex < SegmentTable.Count)
                    Result = new ElementPosition(SegmentIndex, 0);
                else
                    Result = End;
            }

            Debug.Assert(IsValidPosition(Result, true));
            AssertInvariant();

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
            AssertInvariant();

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

            AssertInvariant();

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
            long ItemIndex = 0;
            while (SegmentIndex < SegmentTable.Count)
            {
                Debug.Assert(startIndex >= 0);

                ISegment<T> Segment = SegmentTable[SegmentIndex];
                if (startIndex < Segment.Count)
                    break;

                startIndex -= Segment.Count;
                ItemIndex += Segment.Count;
                SegmentIndex++;
            }

            Debug.Assert((SegmentIndex < SegmentTable.Count && startIndex < SegmentTable[SegmentIndex].Count) || (SegmentIndex == SegmentTable.Count && startIndex == 0));

            int ElementStartIndex = (int)startIndex;

            while (SegmentIndex < SegmentTable.Count && count > 0)
            {
                ISegment<T> Segment = SegmentTable[SegmentIndex];

                int CompareCount = (Segment.Count - ElementStartIndex <= count) ? Segment.Count - ElementStartIndex : (int)count;
                int ElementIndex = Segment.IndexOf(item, ElementStartIndex, CompareCount);
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
            AssertInvariant();

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
            Debug.Assert(Result == -1 || Result >= 0 && Result < Count && GetItem(PositionOf(Result)).Equals(item));
            AssertInvariant();

            return Result;
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

            AssertInvariant();
        }

        /// <summary>
        /// Increases the Partition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="extended">The number of elements added to the Partition&lt;T&gt;.Capacity.</param>
        public void ExtendCapacity(long extended)
        {
            Debug.Assert(extended >= 0);

            // We first extend the capacity of the last segment in the partition.
            if (SegmentTable[SegmentTable.Count - 1].Capacity < MaxSegmentCapacity)
            {
                int Extendable = SegmentTable[SegmentTable.Count - 1].Extendable;
                if (Extendable > extended)
                    Extendable = (int)extended;

                SegmentTable[SegmentTable.Count - 1].Extend(Extendable);
                extended -= Extendable;
            }

            Debug.Assert(extended >= 0);

            // Then we add as many empty segments as necessary to increase the partition capacity.
            while (extended > MaxSegmentCapacity)
            {
                SegmentTable.Add(CreateMaxCapacitySegment());
                extended -= MaxSegmentCapacity;
            }

            Debug.Assert(extended >= 0 && extended <= MaxSegmentCapacity);

            if (extended > 0)
                SegmentTable.Add(CreateSegment((int)extended));

            AssertInvariant();
        }

        /// <summary>
        /// Decreases the Partition&lt;T&gt;.Capacity by the given amount.
        /// </summary>
        /// <param name="trimed">The number of elements substracted to the Partition&lt;T&gt;.Capacity.</param>
        public void TrimCapacity(long trimed)
        {
            Debug.Assert(trimed >= 0);

            // Starting from the end of the partition, we trim all segments until Capacity reaches the target.
            for (int i = SegmentTable.Count; i > 0 && trimed > 0; i--)
            {
                ISegment<T> Segment = SegmentTable[i - 1];
                int Trimmable = Segment.Trimmable;
                if (Trimmable == 0)
                    continue;

                if (Trimmable > trimed)
                    Trimmable = (int)trimed;

                Segment.Trim(Trimmable);
                trimed -= Trimmable;
            }

            Debug.Assert(trimed == 0);

            // Remove all unused segments
            int SegmentIndex = 0;
            while (SegmentIndex < SegmentTable.Count && SegmentTable.Count > 1)
                if (SegmentTable[SegmentIndex].Capacity == 0)
                    RemoveSegmentAt(SegmentIndex);
                else
                    SegmentIndex++;

            AssertInvariant();
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

            // First we try to make room in just one segment.
            int Extendable = SegmentTable[SegmentIndex].Extendable;
            if (Extendable >= count)
                SegmentTable[SegmentIndex].MakeRoom(ElementIndex, (int)count);

            else
            {
                // Otherwise, we're going to need to move elements upward.
                int SegmentEndCount = SegmentTable[SegmentIndex].Count - ElementIndex;
                int NextExtendable = (SegmentIndex + 1 < SegmentTable.Count) ? SegmentTable[SegmentIndex + 1].Extendable : -1;

                Debug.Assert(SegmentEndCount >= 0);
                Debug.Assert(ElementIndex + SegmentEndCount + count > MaxSegmentCapacity);

                // If there is room for elements in the next segment, use it.
                if (SegmentEndCount <= NextExtendable)
                {
                    Debug.Assert(NextExtendable >= 0);
                    Debug.Assert(SegmentIndex + 1 < SegmentTable.Count);
                    Debug.Assert(SegmentTable[SegmentIndex + 1].Count + SegmentEndCount <= MaxSegmentCapacity);

                    SegmentTable[SegmentIndex + 1].MakeRoom(0, SegmentEndCount);
                }

                // Otherwise, perform a split.
                else if (SegmentEndCount > 0)
                {
                    ISegment<T> NewSegment = CreateSegment(SegmentEndCount);
                    NewSegment.MakeRoom(0, SegmentEndCount);
                    SegmentTable.Insert(SegmentIndex + 1, NewSegment);
                }

                // Move the end of the current segment to the new one.
                if (SegmentEndCount > 0)
                    SegmentTable[SegmentIndex].MoveTo(SegmentTable[SegmentIndex + 1], 0, ElementIndex, SegmentEndCount);

                // Check that we didn't do all this for nothing.
                Debug.Assert(ElementIndex == SegmentTable[SegmentIndex].Count);

                // Resume making room.
                Extendable = SegmentTable[SegmentIndex].Extendable;
                if (Extendable > count)
                    Extendable = (int)count;
                SegmentTable[SegmentIndex].MakeRoom(ElementIndex, Extendable);
                count -= Extendable;

                Debug.Assert(count >= 0);

                SegmentIndex++;

                if (SegmentIndex < SegmentTable.Count)
                {
                    Extendable = SegmentTable[SegmentIndex].Extendable;
                    if (Extendable > count)
                        Extendable = (int)count;
                    SegmentTable[SegmentIndex].MakeRoom(0, Extendable);
                    count -= Extendable;

                    Debug.Assert(count >= 0);
                }

                while (count >= MaxSegmentCapacity)
                {
                    ISegment<T> NewSegment = CreateMaxCapacitySegment();
                    NewSegment.MakeRoom(0, MaxSegmentCapacity);

                    SegmentTable.Insert(SegmentIndex, NewSegment);
                    count -= MaxSegmentCapacity;
                }

                if (count > 0)
                {
                    ISegment<T> NewSegment = CreateSegment((int)count);
                    NewSegment.MakeRoom(0, (int)count);

                    SegmentTable.Insert(SegmentIndex, NewSegment);
                }
            }

            AssertInvariant();
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

            AssertInvariant();
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

            AssertInvariant();
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

            while (count > 0)
            {
                Debug.Assert(SegmentIndex >= 0 && SegmentIndex < SegmentTable.Count);

                if (SegmentTable[SegmentIndex].Count == 0)
                {
                    SegmentIndex++;
                    continue;
                }

                Debug.Assert(ElementIndex >= 0 && ElementIndex < SegmentTable[SegmentIndex].Count);

                int Removable = SegmentTable[SegmentIndex].Count - ElementIndex;
                if (Removable > count)
                    Removable = (int)count;

                SegmentTable[SegmentIndex++].RemoveRange(ElementIndex, Removable);
                
                count -= Removable;
                ElementIndex = 0;
            }

            Debug.Assert(count == 0);

            AssertInvariant();
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
                    break;
                }

            AssertInvariant();

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

            foreach (ISegment<T> Segment in SegmentTable)
                RemovedCount += Segment.RemoveAll(match);

            AssertInvariant();

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

            AssertInvariant();
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
                QuickSort(begin, PreviousPosition(end), comparer);

            AssertInvariant();
        }

        private void QuickSort(ElementPosition low, ElementPosition high, IComparer<T> comparer)
        {
            Debug.Assert(IsValidPosition(low, false));
            Debug.Assert(IsValidPosition(high, false));
            Debug.Assert(comparer != null);

            if (low < high)
            {
                ElementPosition middle = SplitSortInterval(low, high, comparer);
                QuickSort(low, middle, comparer);
                QuickSort(NextPosition(middle), high, comparer);
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
                        up = Begin;
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
            RemoveSegmentAt(SegmentTable.IndexOf(segment));
        }

        /// <summary>
        /// Removes the segment at the specified index from the Partition&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the segment to remove.</param>
        protected void RemoveSegmentAt(int index)
        {
            SegmentTable.RemoveAt(index);
        }
        #endregion

        private List<ISegment<T>> SegmentTable;

        #region Contracts
        private void AssertInvariant()
        {
            Debug.Assert(SegmentTable.Count > 0);
            Debug.Assert(Count <= Capacity);

            foreach (ISegment<T> segment in SegmentTable)
                Debug.Assert(segment.Count <= segment.Capacity);
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
