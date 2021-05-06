namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Defines size, enumerators, and synchronization methods for all nongeneric large collections.
    /// </summary>
    public interface ILargeCollection : IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the ILargeCollection.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the ILargeCollection.
        /// </returns>
        long Count { get; }

        /// <summary>
        /// Gets a value indicating whether access to the ILargeCollection is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the ILargeCollection is synchronized (thread safe); otherwise, false.
        /// </returns>
        bool IsSynchronized { get; }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ILargeCollection.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the ILargeCollection.
        /// </returns>
        object SyncRoot { get; }

#if STRICT
        /// <summary>
        /// Copies the elements of the ILargeCollection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ILargeCollection. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para><paramref name="array"/> is multidimensional.</para><para>-or-</para><para>The number of elements in the source ILargeCollection is greater than the available space from <paramref name="index"/> to the end of the destination array.</para><para>-or-</para><para>The type of the source ILargeCollection cannot be cast automatically to the type of the destination array.</para></exception>
        void CopyTo(Array array, int index);
#else
        /// <summary>
        /// Copies the elements of the ILargeCollection to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ILargeCollection. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException"><para><paramref name="array"/> is multidimensional.</para><para>-or-</para><para>The number of elements in the source ILargeCollection is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para><para>-or-</para><para>The type of the source ILargeCollection cannot be cast automatically to the type of the destination array.</para></exception>
        void CopyTo(Array array, int arrayIndex);
#endif
    }

    /// <summary>
    /// Defines methods to manipulate large generic collections.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    public interface ILargeCollection<T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="ILargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ILargeCollection{T}"/>.
        /// </returns>
        long Count { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ILargeCollection{T}"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ILargeCollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Adds an item to the <see cref="ILargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="ILargeCollection{T}"/>.</param>
        /// <exception cref="NotSupportedException">The <see cref="ILargeCollection{T}"/> is read-only.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Add(T item);

        /// <summary>
        /// Removes all items from the <see cref="ILargeCollection{T}"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The <see cref="ILargeCollection{T}"/> is read-only.</exception>
        void Clear();

        /// <summary>
        /// Determines whether the <see cref="ILargeCollection{T}"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ILargeCollection{T}"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ILargeCollection{T}"/>; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Copies the elements of the <see cref="ILargeCollection{T}"/> to a System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ILargeCollection{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ILargeCollection{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ILargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ILargeCollection{T}"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="ILargeCollection{T}"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="ILargeCollection{T}"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">The <see cref="ILargeCollection{T}"/> is read-only.</exception>
        bool Remove(T item);
    }
}
