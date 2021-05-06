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
}
