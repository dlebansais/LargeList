namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

#pragma warning disable CS1710
    /// <summary>
    /// Represents a set of segments of varying (but limited) capacity that together virtualize a large list of generic objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the partition.</typeparam>
#pragma warning restore CS1710
#if STRICT
    internal
#else
    public
#endif
    partial class Partition<T> : IPartition<T>
    {
        #region Queries
        /// <summary>
        /// Determines whether an element is in the <see cref="Partition{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="Partition{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="Partition{T}"/>; otherwise, false.
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
        /// <param name="item">The object to locate in the <see cref="Partition{T}"/>. The value can be null for reference types.</param>
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
        /// <param name="item">The object to locate in the <see cref="Partition{T}"/>. The value can be null for reference types.</param>
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

            for (; ;)
            {
                Debug.Assert(SegmentIndex >= 0 && SegmentIndex < SegmentTable.Count && Segment == SegmentTable[SegmentIndex]);
                Debug.Assert(ElementStartIndex >= 0 && ElementStartIndex < Segment.Count);
                Debug.Assert(count >= 0);

                if (LastIndexOfLoop(item, ref count, ref Segment, ref SegmentIndex, ref ElementStartIndex, ref ItemIndex, ref Result))
                    break;
            }

            Debug.Assert(count >= 0);
            Debug.Assert(Result == -1 || (Result >= 0 && Result < Count && ((item == null && IsItemNull(Result)) || (item != null && IsItemEqual(Result, item)))));

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        private bool LastIndexOfLoop(T item, ref long count, ref ISegment<T> segment, ref int segmentIndex, ref int elementStartIndex, ref long itemIndex, ref long result)
        {
            int CompareCount = (elementStartIndex < count) ? elementStartIndex + 1 : (int)count;
            int ElementIndex = segment.LastIndexOf(item, elementStartIndex, CompareCount);
            if (ElementIndex >= 0)
            {
                result = itemIndex - elementStartIndex + ElementIndex;
                return true;
            }

            count -= CompareCount;
            itemIndex -= CompareCount;
            segmentIndex--;

            Debug.Assert(segmentIndex < 0 || SegmentTable[segmentIndex].Count > 0);

            if (segmentIndex < 0 || count == 0)
                return true;

            segment = SegmentTable[segmentIndex];
            elementStartIndex = segment.Count - 1;

            return false;
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="Partition{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="Partition{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="Partition{T}"/>.Count.
        /// </returns>
        public long BinarySearch(long index, long count, T item, IComparer<T> comparer)
        {
            Contracts.Contract.RequireNotNull(comparer, out IComparer<T> Comparer);

            Debug.Assert(index >= 0);
            Debug.Assert(count >= 0);
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

                long comparisonResult = Comparer.Compare(item, SegmentTable[SegmentIndexMiddle][ElementIndexMiddle]);

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

                int Middle = elementIndexLower + ((elementIndexUpper - elementIndexLower) / 2);

                segmentIndexMiddle = segmentIndexLower;
                elementIndexMiddle = Middle;
                indexMiddle = indexLower - elementIndexLower + Middle;
            }

            Debug.Assert(IsPositionEqual(indexMiddle, segmentIndexMiddle, elementIndexMiddle));
        }
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
            Contracts.Contract.RequireNotNull(comparer, out IComparer<T> Comparer);

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
                Segment.Sort(ElementIndexLow, ElementIndexHigh, Comparer);
            }
            else if (SegmentIndexLow < SegmentIndexHigh || (SegmentIndexLow == SegmentIndexHigh && ElementIndexLow < ElementIndexHigh))
            {
                int SegmentIndexMiddle;
                int ElementIndexMiddle;
                SplitSortInterval(SegmentIndexLow, ElementIndexLow, SegmentIndexHigh, ElementIndexHigh, Comparer, out SegmentIndexMiddle, out ElementIndexMiddle);

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
            Contracts.Contract.RequireNotNull(comparer, out IComparer<T> Comparer);

            Debug.Assert(IsValidPosition(segmentIndexLow, elementIndexLow, false));
            Debug.Assert(IsValidPosition(segmentIndexHigh, elementIndexHigh, false));
            Debug.Assert(segmentIndexLow < segmentIndexHigh || (segmentIndexLow == segmentIndexHigh && elementIndexLow < elementIndexHigh));

            T pivot = SelectPivot(segmentIndexLow, elementIndexLow, segmentIndexHigh, elementIndexHigh);

            int SegmentIndexUp;
            int ElementIndexUp;
            int SegmentIndexDown;
            int ElementIndexDown;

            GetPreviousPosition(segmentIndexLow, elementIndexLow, out SegmentIndexUp, out ElementIndexUp);
            GetNextPosition(segmentIndexHigh, elementIndexHigh, out SegmentIndexDown, out ElementIndexDown);

            for (; ;)
            {
                SplitSortLoop1(Comparer, pivot, ref SegmentIndexUp, ref ElementIndexUp);
                SplitSortLoop2(Comparer, pivot, ref SegmentIndexDown, ref ElementIndexDown);

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

        private void SplitSortLoop1(IComparer<T> comparer, T pivot, ref int segmentIndexUp, ref int elementIndexUp)
        {
            do
            {
                if (segmentIndexUp < 0)
                {
                    segmentIndexUp = 0;
                    elementIndexUp = 0;
                }
                else
                {
                    if (elementIndexUp + 1 < SegmentTable[segmentIndexUp].Count)
                        elementIndexUp++;
                    else
                    {
                        segmentIndexUp++;
                        Debug.Assert(segmentIndexUp < SegmentTable.Count && SegmentTable[segmentIndexUp].Count > 0);
                        elementIndexUp = 0;
                    }
                }
            }
            while (comparer.Compare(SegmentTable[segmentIndexUp][elementIndexUp], pivot) < 0);
        }

        private void SplitSortLoop2(IComparer<T> comparer, T pivot, ref int segmentIndexDown, ref int elementIndexDown)
        {
            do
            {
                if (elementIndexDown > 0)
                    elementIndexDown--;
                else
                {
                    segmentIndexDown--;
                    Debug.Assert(segmentIndexDown >= 0 && SegmentTable[segmentIndexDown].Count > 0);
                    elementIndexDown = SegmentTable[segmentIndexDown].Count - 1;
                }
            }
            while (comparer.Compare(SegmentTable[segmentIndexDown][elementIndexDown], pivot) > 0);
        }
        #endregion
    }
}
