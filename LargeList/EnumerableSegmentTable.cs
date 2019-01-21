namespace LargeList
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Simple representation of partially enumerable <see cref="Partition{T}"/> object.
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
        /// Initializes a new instance of the <see cref="EnumerableSegmentTable{T}"/> class.
        /// </summary>
        /// <param name="partition">The <see cref="Partition{T}"/> this class is representing.</param>
        /// <param name="index">The index of the first element to enumerate in <paramref name="partition"/>.</param>
        /// <param name="count">The number of elements to enumerate in <paramref name="partition"/>.</param>
        public EnumerableSegmentTable(IPartition<T> partition, long index, long count)
        {
            Partition = partition;
            Index = index;
            Count = count;
        }

        /// <summary>
        /// Gets the <see cref="Partition{T}"/> this class is representing.
        /// </summary>
        /// <returns>
        /// The <see cref="Partition{T}"/> this class is representing.
        /// </returns>
        public IPartition<T> Partition { get; private set; }

        /// <summary>
        /// Gets the index of the first element to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
        /// </summary>
        /// <returns>
        /// The index of the first element to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
        /// </returns>
        public long Index { get; private set; }

        /// <summary>
        /// Gets the number of elements to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
        /// </summary>
        /// <returns>
        /// The number of elements to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
        /// </returns>
        public long Count { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Partition{T}"/>.
        /// </summary>
        /// <returns>
        /// An enumerator for the <see cref="Partition{T}"/>.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new LargeList<T>.LargeEnumerator(Partition, Index, Count);
        }
#pragma warning disable SA1600
        IEnumerator IEnumerable.GetEnumerator()
#pragma warning restore SA1600
        {
            return GetEnumerator();
        }
    }
}
