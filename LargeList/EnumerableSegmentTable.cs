namespace LargeCollections;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple representation of partially enumerable <see cref="Partition{T}"/> object.
/// </summary>
/// <typeparam name="T">The type of elements in the partition.</typeparam>
/// <param name="partition">The <see cref="Partition{T}"/> this class is representing.</param>
/// <param name="index">The index of the first element to enumerate in <paramref name="partition"/>.</param>
/// <param name="count">The number of elements to enumerate in <paramref name="partition"/>.</param>
#if STRICT
internal sealed
#else
public
#endif
class EnumerableSegmentTable<T>(IPartition<T> partition, long index, long count) : IEnumerable<T>
{
    /// <summary>
    /// Gets the <see cref="Partition{T}"/> this class is representing.
    /// </summary>
    /// <returns>
    /// The <see cref="Partition{T}"/> this class is representing.
    /// </returns>
    public IPartition<T> Partition { get; } = partition;

    /// <summary>
    /// Gets the index of the first element to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
    /// </summary>
    /// <returns>
    /// The index of the first element to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
    /// </returns>
    public long Index { get; } = index;

    /// <summary>
    /// Gets the number of elements to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
    /// </summary>
    /// <returns>
    /// The number of elements to enumerate in <see cref="EnumerableSegmentTable{T}.Partition"/>.
    /// </returns>
    public long Count { get; } = count;

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
    public IEnumerator<T> GetEnumerator()
    {
        return new LargeList<T>.LargeEnumerator(Partition, Index, Count);
    }

    /// <inheritdoc cref="IEnumerable.GetEnumerator" />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
