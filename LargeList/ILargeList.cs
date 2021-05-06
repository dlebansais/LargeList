namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Contracts;

    /// <summary>
    /// Represents a nongeneric large collection of objects that can be individually accessed by index.
    /// </summary>
    public interface ILargeList : ILargeCollection, IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException">The property is set and the ILargeList is read-only.</exception>
        object this[long index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ILargeList has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the ILargeList has a fixed size; otherwise, false.
        /// </returns>
        bool IsFixedSize { get; }

        /// <summary>
        /// Gets a value indicating whether the ILargeList is read-only.
        /// </summary>
        /// <returns>
        /// true if the ILargeList is read-only; otherwise, false.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Adds an object to the ILargeList.
        /// </summary>
        /// <param name="item">The object to add to the ILargeList.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that <paramref name="item"/> was not inserted into the collection.
        /// </returns>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        long Add(object item);

        /// <summary>
        /// Removes all items from the ILargeList.
        /// </summary>
        /// <exception cref="NotSupportedException">The ILargeList is read-only.</exception>
        void Clear();

        /// <summary>
        /// Determines whether the ILargeList contains a specific object.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeList.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the ILargeList; otherwise, false.
        /// </returns>
        bool Contains(object item);

        /// <summary>
        /// Determines the index of a specific object in the ILargeList.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeList.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        long IndexOf(object item);

        /// <summary>
        /// Inserts an object to the ILargeList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the ILargeList.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Insert(long index, object item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the ILargeList.
        /// </summary>
        /// <param name="item">The object to remove from the ILargeList.</param>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        void Remove(object item);

        /// <summary>
        /// Removes the ILargeList element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        void RemoveAt(long index);
    }

    /// <summary>
    /// Represents a generic large collection of objects that can be individually accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface ILargeList<T> : ILargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ILargeList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The property is set and the <see cref="ILargeList{T}"/> is read-only.</exception>
        T this[long index] { get; set; }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="ILargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ILargeList{T}"/>.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        long IndexOf(T item);

        /// <summary>
        /// Inserts an item to the <see cref="ILargeList{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="ILargeList{T}"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ILargeList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The <see cref="ILargeList{T}"/> is read-only.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Insert(long index, T item);

        /// <summary>
        /// Removes the <see cref="ILargeList{T}"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="ILargeList{T}"/>.</exception>
        /// <exception cref="NotSupportedException">The <see cref="ILargeList{T}"/> is read-only.</exception>
        void RemoveAt(long index);
    }
}
