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
            Segment = null;
            Enumerator = null;
            SegmentCount = 0;
        }

        /// <summary>
        /// Initializes a new instance of the PartitionEnumerator&lt;T&gt; class that enumerates objects starting at a given element in a given segment.
        /// </summary>
        /// <param name="segment">The segment iteration is starting from.</param>
        /// <param name="index">The index in <paramref name="segment"/> of the first element to enumerate.</param>
        public PartitionEnumerator(ISegment<T> segment, int index)
        {
            Debug.Assert(segment != null);
            Debug.Assert(index >= 0 && index < segment.Count);

            Segment = segment;
            Enumerator = Segment.GetEnumerator(index);
            SegmentCount = segment.Count - index;
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
            if (Segment == null || Enumerator == null)
                return;

            if (SegmentCount > 0)
            {
                SegmentCount--;

                bool Moved = Enumerator.MoveNext();
                Debug.Assert(Moved);

                return;
            }

            do
            {
                Segment = partition.NextSegment(Segment);
                if (Segment == null)
                {
                    Enumerator = null;
                    return;
                }
            }
            while (Segment.Count == 0);

            Enumerator = Segment.GetEnumerator(0);
            Enumerator.MoveNext();
            SegmentCount = Segment.Count - 1;
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
            Segment = null;
            Enumerator = null;
        }
        #endregion

        private ISegment<T> Segment;
        private IEnumerator<T> Enumerator;
        private int SegmentCount;
    }
}
