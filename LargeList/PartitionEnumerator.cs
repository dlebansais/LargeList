using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
    /// <summary>
    /// Supports a simple iteration over a IPartition&lt;T&gt; object.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    interface IPartitionEnumerator<T>
    {
        /// <summary>
        /// Gets the element in the IPartition&lt;T&gt; at the current position of the enumerator.
        /// </summary>
        T Current { get; }

        /// <summary>
        /// Advances the enumerator to the next element of the IPartition&lt;T&gt;. If there are no more elements, does nothing.
        /// </summary>
        /// <param name="partition">The IPartition&lt;T&gt; object over which this enumerator is iterating.</param>
        void MoveNext(IPartition<T> partition);
    }

    /// <summary>
    /// Enumerates the elements of a IPartition&lt;T&gt; object.
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
        /// Initializes a new instance of the PartitionEnumerator&lt;T&gt; class that has no element to enumerate.
        /// </summary>
        public PartitionEnumerator()
        {
            Partition = null;
            Enumerator = null;
        }

        /// <summary>
        /// Initializes a new instance of the PartitionEnumerator&lt;T&gt; class that enumerates objects starting at the specified position in IPartition&lt;T&gt;.
        /// </summary>
        /// <param name="partition">The enumerated partition.</param>
        /// <param name="position">The position in <paramref name="partition"/> of the first element to enumerate.</param>
        public PartitionEnumerator(IPartition<T> partition, ElementPosition position)
        {
            Debug.Assert(partition.IsValidPosition(position, false));

            Partition = partition;
            Position = position;

            Enumerator = partition.GetSegmentEnumerator(position, out SegmentCount);
        }

        /// <summary>
        /// Gets the element in the IPartition&lt;T&gt; at the current position of the enumerator.
        /// </summary>
        public T Current { get { return Enumerator.Current; } }

        /// <summary>
        /// Advances the enumerator to the next element of the IPartition&lt;T&gt;. If there are no more elements, does nothing.
        /// </summary>
        /// <param name="partition">The IPartition&lt;T&gt; object over which this enumerator is iterating.</param>
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

            int SegmentIndex = partition.NextSegmentIndex(Position.SegmentIndex);
            if (SegmentIndex < 0)
            {
                Enumerator = null;
                return;
            }

            Position = new ElementPosition(SegmentIndex, 0, -1);
            Enumerator = partition.GetSegmentEnumerator(Position, out SegmentCount);
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
        /// Releases the unmanaged resources used by the PartitionEnumerator&lt;T&gt; and optionally releases the managed resources.
        /// </summary>
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

        private IPartition<T> Partition;
        private ElementPosition Position;
        private IEnumerator<T> Enumerator;
        private int SegmentCount;
    }
}
