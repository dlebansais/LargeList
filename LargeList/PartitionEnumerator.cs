namespace LargeCollections
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(partition);
#else
            if (partition is null)
                throw new ArgumentNullException(nameof(partition));
#endif

            Debug.Assert(partition.IsValidPosition(segmentIndex, elementIndex, false));

            Partition = partition;
            SegmentIndex = segmentIndex;

            Enumerator = partition.GetSegmentEnumerator(segmentIndex, elementIndex, out SegmentCount);
        }

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        public T Current
        {
            get
            {
                if (Enumerator is null)
                    throw new InvalidOperationException();

                return Enumerator.Current;
            }
        }

        /// <inheritdoc cref="IPartitionEnumerator{T}.MoveNext" />
        public void MoveNext(IPartition<T> partition)
        {
            if (partition is null || Enumerator is null)
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
        /// <inheritdoc cref="IDisposable.Dispose" />
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
            Enumerator?.Dispose();
            Enumerator = null;
        }
        #endregion

        private IPartition<T>? Partition;
        private int SegmentIndex;
        private IEnumerator<T>? Enumerator;
        private int SegmentCount;
    }
}
