using System.Collections;
using System.Collections.Generic;

namespace LargeList
{
    /// <summary>
    /// Simple representation of partially enumerable Partition&lt;T&gt; object.
    /// </summary>
    /// <typeparam name="T">The type of elements in the partition.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    class EnumerableSegmentTable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the EnumerableSegmentTable&lt;T&gt; class.
        /// </summary>
        /// <param name="partition">The Partition&lt;T&gt; this class is representing.</param>
        /// <param name="index">The index of the first element to enumerate in <paramref name="partition"/>.</param>
        /// <param name="count">The number of elements to enumerate in <paramref name="partition"/>.</param>
        public EnumerableSegmentTable(IPartition<T> partition, long index, long count)
        {
            Partition = partition;
            Index = index;
            Count = count;
        }

        /// <summary>
        /// Gets the Partition&lt;T&gt; this class is representing.
        /// </summary>
        /// <returns>
        /// The Partition&lt;T&gt; this class is representing.
        /// </returns>
        public IPartition<T> Partition { get; private set; }

        /// <summary>
        /// Gets the index of the first element to enumerate in EnumerableSegmentTable&lt;T&gt;.Partition.
        /// </summary>
        /// <returns>
        /// The index of the first element to enumerate in EnumerableSegmentTable&lt;T&gt;.Partition.
        /// </returns>
        public long Index { get; private set; }

        /// <summary>
        /// Gets the number of elements to enumerate in EnumerableSegmentTable&lt;T&gt;.Partition.
        /// </summary>
        /// <returns>
        /// The number of elements to enumerate in EnumerableSegmentTable&lt;T&gt;.Partition.
        /// </returns>
        public long Count { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the Partition&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An enumerator for the Partition&lt;T&gt;.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new LargeList<T>.LargeEnumerator(Partition, Index, Count);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
