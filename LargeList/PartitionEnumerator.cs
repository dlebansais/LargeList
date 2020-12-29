namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Supports a simple iteration over a <see cref="IPartition{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    interface IPartitionEnumerator<T> : IDisposable
    {
        /// <summary>
        /// Gets the element in the <see cref="IPartition{T}"/> at the current position of the enumerator.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="IPartition{T}"/>. If there are no more elements, does nothing.
        /// </summary>
        /// <param name="partition">The <see cref="IPartition{T}"/> object over which this enumerator is iterating.</param>
        void MoveNext(IPartition<T> partition);
    }

    /// <summary>
    /// Enumerates the elements of a <see cref="IPartition{T}"/> object.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    class PartitionEnumerator<T> : IPartitionEnumerator<T>, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionEnumerator{T}"/> class.
        /// Creates an instance that has no element to enumerate.
        /// </summary>
        public PartitionEnumerator()
        {
            Partition = null;
            Enumerator = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartitionEnumerator{T}"/> class.
        /// Creates an instance that enumerates objects starting at the specified position in <see cref="IPartition{T}"/>.
        /// </summary>
        /// <param name="partition">The enumerated partition.</param>
        /// <param name="segmentIndex">The segment index of the position of the first element to enumerate.</param>
        /// <param name="elementIndex">The element index of the position of the first element to enumerate.</param>
        public PartitionEnumerator(IPartition<T> partition, int segmentIndex, int elementIndex)
        {
            Debug.Assert(partition.IsValidPosition(segmentIndex, elementIndex, false));

            Partition = partition;
            SegmentIndex = segmentIndex;

            Enumerator = partition.GetSegmentEnumerator(segmentIndex, elementIndex, out SegmentCount);
        }

        /// <summary>
        /// Gets the element in the <see cref="IPartition{T}"/> at the current position of the enumerator.
        /// </summary>
        public T Current
        {
            get
            {
                if (Enumerator == null)
                    throw new InvalidOperationException();

                return Enumerator.Current;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the <see cref="IPartition{T}"/>. If there are no more elements, does nothing.
        /// </summary>
        /// <param name="partition">The <see cref="IPartition{T}"/> object over which this enumerator is iterating.</param>
        public void MoveNext(IPartition<T> partition)
        {
            if (partition == null || Enumerator == null)
                return;

            if (SegmentCount > 0)
            {
                SegmentCount--;

                bool Moved = Enumerator.MoveNext();
                Debug.Assert(Moved);

                return;
            }

            SegmentIndex = partition.NextSegmentIndex(SegmentIndex);
            if (SegmentIndex < 0)
            {
                Enumerator = null;
                return;
            }

            Enumerator = partition.GetSegmentEnumerator(SegmentIndex, 0, out SegmentCount);
            Enumerator.MoveNext();

            Debug.Assert(SegmentCount > 0);
            SegmentCount--;
        }

        #region Implementation of IDisposable
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="PartitionEnumerator{T}"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="isDisposing">True if the object is being disposed of.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
                DisposeNow();
        }

        private void DisposeNow()
        {
            Partition = null;
            Enumerator = null;
        }
        #endregion

        private IPartition<T>? Partition;
        private int SegmentIndex;
        private IEnumerator<T>? Enumerator;
        private int SegmentCount;
    }
}
