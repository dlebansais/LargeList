namespace LargeCollections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using Contracts;

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
            Debug.Assert(SegmentTable[0].ToString() is not null); // For code coverage.

#if DEBUG
            AssertInvariant();
#endif
        }
        #endregion

        #region Properties
        /// <inheritdoc cref="IPartition{T}.MaxSegmentCapacity" />
        public int MaxSegmentCapacity { get; private set; }

        /// <inheritdoc cref="IPartition{T}.Capacity" />
        public long Capacity { get; private set; }

        /// <inheritdoc cref="IPartition{T}.Count" />
        public long Count { get; private set; }
        #endregion

        #region Queries
        /// <inheritdoc cref="IPartition{T}.GetPosition(long, out int, out int, out int)" />
        public void GetPosition(long index, out int segmentIndex, out int elementIndex, out int cacheIndex)
        {
            Debug.Assert(index >= 0 && index <= Count);

            PositionFromCache(index, out segmentIndex, out elementIndex, out cacheIndex);

            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <inheritdoc cref="IPartition{T}.IsValidPosition(int, int, bool)" />
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

        /// <inheritdoc cref="IPartition{T}.GetPreviousPosition(int, int, out int, out int)" />
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

        /// <inheritdoc cref="IPartition{T}.GetNextPosition(int, int, out int, out int)" />
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

        /// <inheritdoc cref="IPartition{T}.DecrementPosition(ref int, ref int)" />
        public void DecrementPosition(ref int segmentIndex, ref int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            if (elementIndex > 0)
            {
                elementIndex--;
            }
            else
            {
                segmentIndex--;

                if (segmentIndex >= 0)
                {
                    Debug.Assert(SegmentTable[segmentIndex].Count > 0);
                    elementIndex = SegmentTable[segmentIndex].Count - 1;
                }
                else
                {
                    elementIndex = 0;
                }
            }

            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false) || (segmentIndex == -1 && elementIndex == 0));

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <inheritdoc cref="IPartition{T}.IncrementPosition(ref int, ref int)" />
        public void IncrementPosition(ref int segmentIndex, ref int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            if (elementIndex + 1 < SegmentTable[segmentIndex].Count || segmentIndex + 1 >= SegmentTable.Count || SegmentTable[segmentIndex + 1].Count == 0)
            {
                elementIndex++;
            }
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

        /// <inheritdoc cref="IPartition{T}.GetItem(int, int)" />
        public T GetItem(int segmentIndex, int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            T Result = SegmentTable[segmentIndex][elementIndex];

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <inheritdoc cref="IPartition{T}.GetEnumerator(int, int)" />
        public IPartitionEnumerator<T> GetEnumerator(int segmentIndex, int elementIndex)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, true));

            Contract.RequireNotNull(CreateEnumerator(segmentIndex, elementIndex), out IPartitionEnumerator<T> Result);

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <inheritdoc cref="IPartition{T}.GetSegmentEnumerator(int, int, out int)" />
        public IEnumerator<T> GetSegmentEnumerator(int segmentIndex, int elementIndex, out int remainingCount)
        {
            Debug.Assert(IsValidPosition(segmentIndex, elementIndex, false));

            ISegment<T> Segment = SegmentTable[segmentIndex];

            remainingCount = Segment.Count - elementIndex;
            Debug.Assert(remainingCount > 0);

            return Segment.GetEnumerator(elementIndex);
        }

        /// <inheritdoc cref="IPartition{T}.NextSegmentIndex(int)" />
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
            {
                return new PartitionEnumerator<T>();
            }
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
                return SegmentIndex.ToString(CultureInfo.InvariantCulture) + "," + Min;
            }
        }

        private void InitCache()
        {
            RebuildCache();
        }

        private void RebuildCache()
        {
            Debug.Assert(SegmentTable.Count > 0);

            Debug.Assert(MaxSegmentCapacity > 0);
            CacheLineExponent = HighestExponentAbove(MaxSegmentCapacity) - 1;

            Debug.Assert(CacheLineExponent >= 0);
            CacheLineLength = 1 << CacheLineExponent;

            Debug.Assert(CacheLineLength > 0);

            ResizeCache();

            Cache[0].SegmentIndex = 0;
            Cache[0].Min = 0;
            Debug.Assert(Cache[0].ToString() is not null); // For code coverage.

            RebuildCacheFrom(0);
        }

        private void ResizeCache()
        {
            CacheLineCount = (int)(Count / CacheLineLength) + 1;

            if (Cache is null || CacheLineCount > Cache.Length)
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

                long CacheIndex = (long)cacheLine * CacheLineLength;

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

        private static int HighestExponentAbove(long n)
        {
            Debug.Assert(n >= 0);

            int Exponent = 0;
            long i = n;

            while (i > 0)
            {
                Exponent++;
                i >>= 1;
            }

            Debug.Assert((1L << Exponent) >= n);

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

        private CacheLine[] Cache = Array.Empty<CacheLine>();
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
                {
                    IsPreviousSegmentVoid = true;
                }
                else
                {
                    Debug.Assert(!IsPreviousSegmentVoid);
                    TotalCapacity += Segment.Capacity;
                }

                if (Segment.Count == 0)
                {
                    IsPreviousSegmentEmpty = true;
                }
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

            return GetItem(SegmentIndex, ElementIndex) is null;
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

        private static void BreakIfNotDebugging()
        {
#if DEBUG
#else
            Debugger.Break(); // This method should not be called in release mode.
#endif
        }
        #endregion
    }
}
