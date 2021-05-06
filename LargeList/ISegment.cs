namespace LargeList
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Contracts;

    /// <summary>
    /// Supports a linear storage of objects of type <typeparamref name="T"/> and fast operations to copy or move elements from a <see cref="ISegment{T}"/> object to another.
    /// </summary>
    /// <typeparam name="T">The type of objects to store and manipulate.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    interface ISegment<T>
    {
        /// <summary>
        /// Gets the total number of elements the <see cref="ISegment{T}"/> can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the <see cref="ISegment{T}"/> can contain before resizing is required.
        /// </returns>
        int Capacity { get; }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ISegment{T}"/>.
        /// </returns>
        int Count { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        T this[int index] { get; set; }

        /// <summary>
        /// Gets the number of elements that can be substracted to <see cref="ISegment{T}"/>.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be substracted to <see cref="ISegment{T}"/>.Capacity.
        /// </returns>
        int Trimmable { get; }

        /// <summary>
        /// Gets the number of elements that can be added before reaching the maximum allowed value for <see cref="ISegment{T}"/>.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be added before reaching the maximum allowed value for <see cref="ISegment{T}"/>.Capacity.
        /// </returns>
        int Extendable { get; }

        /// <summary>
        /// Determines whether an element is in the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ISegment{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ISegment{T}"/>; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="ISegment{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ISegment{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="ISegment{T}"/> that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        int IndexOf(T item, int startIndex, int count);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="ISegment{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ISegment{T}"/>. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="ISegment{T}"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        int LastIndexOf(T item, int startIndex, int count);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="index">Index of the segment.</param>
        /// <returns>
        /// An enumerator for the <see cref="ISegment{T}"/>.
        /// </returns>
        IEnumerator<T> GetEnumerator(long index);

        /// <summary>
        /// Removes all elements from the <see cref="ISegment{T}"/>. Sets the <see cref="ISegment{T}"/>.Count to zero.
        /// </summary>
        void Clear();

        /// <summary>
        /// Extends the number of stored elements by the <see cref="ISegment{T}"/> and leave them uninitialized.
        /// </summary>
        /// <param name="extended">The number of elements added to this <see cref="ISegment{T}"/>.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        void Extend(int extended, out int effectiveExtended);

        /// <summary>
        /// Reduces the maximum number of elements this <see cref="ISegment{T}"/> can store.
        /// </summary>
        /// <param name="trimmed">The number of elements that this <see cref="ISegment{T}"/> can no longer store and is substracted to <see cref="ISegment{T}"/>.Capacity.</param>
        void Trim(int trimmed);

        /// <summary>
        /// Makes room for a number of elements at the specified index. Elements already the specified position and beyond are moved toward the end of the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        void MakeRoom(int index, int count, out int effectiveExtended);

        /// <summary>
        /// Moves elements from this <see cref="ISegment{T}"/> to another from and to the specified indexes. Moved elements are replaced by default values in the source, and override existing elements in the destination.
        /// </summary>
        /// <param name="destination">The destination <see cref="ISegment{T}"/> object.</param>
        /// <param name="toIndex">The zero-based index at which the new elements should be moved in the destination.</param>
        /// <param name="fromIndex">The zero-based index in the source from which elements should be moved.</param>
        /// <param name="count">The number of elements to move.</param>
        void MoveTo(ISegment<T> destination, int toIndex, int fromIndex, int count);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="ISegment{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the <see cref="ISegment{T}"/>.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes a range of elements from the <see cref="ISegment{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        void RemoveRange(int index, int count);

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the <see cref="ISegment{T}"/>.
        /// </returns>
        int RemoveAll(Predicate<T> match);

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="ISegment{T}"/> using the specified comparer.
        /// </summary>
        /// <param name="low">The position of the first item in the range.</param>
        /// <param name="high">The position of the last item in the range.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements.</param>
        void Sort(int low, int high, IComparer<T> comparer);
    }
}
