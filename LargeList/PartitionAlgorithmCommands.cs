namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Contracts;

    /// <summary>
    /// Represents a set of segments of varying (but limited) capacity that together virtualize a large list of <typeparamref name="T"/> objects.
    /// </summary>
#if STRICT
    internal
#else
    public
#endif
#pragma warning disable SA1619 // Generic type parameters should be documented
    partial class Partition<T> : IPartition<T>
#pragma warning restore SA1619 // Generic type parameters should be documented
    {
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
            Contract.RequireNotNull(comparer, out IComparer<T> Comparer);

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
                    QuickSort(Comparer);

                Debug.Assert(QuickSortStack.Count == 0);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        private Stack<QuickSortRange> QuickSortStack = new Stack<QuickSortRange>();
        #endregion
    }
}
