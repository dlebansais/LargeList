namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Provides the base class for a generic read-only large list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    [Serializable]
    public class ReadOnlyLargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyLargeList{T}"/> class.
        /// Creates an instance that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public ReadOnlyLargeList(LargeList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            List = list;
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than <see cref="ReadOnlyLargeList{T}.Count"/>.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return List[index];
            }
        }
#pragma warning disable SA1600
        T ILargeList<T>.this[long index]
#pragma warning restore SA1600
        {
            get { return this[index]; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
            set { throw new NotSupportedException(); }
        }
#pragma warning disable SA1600
        object ILargeList.this[long index]
#pragma warning restore SA1600
        {
            get { return this[index]!; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ReadOnlyLargeList{T}"/> instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ReadOnlyLargeList{T}"/> instance.
        /// </returns>
        public long Count { get { return List.Count; } }

        /// <summary>
        /// Gets the <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeList{T}"/> wraps.
        /// </summary>
        /// <returns>
        /// The <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeList{T}"/> wraps.
        /// </returns>
        protected virtual ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Determines whether an element is in the <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="ReadOnlyLargeList{T}"/>; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return List.Contains(item);
        }
#pragma warning disable SA1600
        bool ILargeList.Contains(object item)
#pragma warning restore SA1600
        {
            return Contains((T)item);
        }

        /// <summary>
        /// Copies the entire <see cref="ReadOnlyLargeList{T}"/> to a compatible one-dimensional System.Array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ReadOnlyLargeList{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlyLargeList{T}"/> is greater than the number of elements that the destination array can contain.</exception>
        public void CopyTo(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (Count > array.Length)
                throw new ArgumentException();

            List.CopyTo(array);
        }

        /// <summary>
        /// Copies the entire <see cref="ReadOnlyLargeList{T}"/> to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ReadOnlyLargeList{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlyLargeList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException();

            List.CopyTo(array, arrayIndex);
        }
#pragma warning disable SA1600
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
#pragma warning restore SA1600
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException();

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="ReadOnlyLargeList{T}"/> to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="ReadOnlyLargeList{T}"/> at which copying begins.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ReadOnlyLargeList{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="arrayIndex"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> is equal to or greater than the <see cref="ReadOnlyLargeList{T}.Count"/> of the source <see cref="ReadOnlyLargeList{T}"/>.</para><para>-or-</para><para>The number of elements from <paramref name="index"/> to the end of the source <see cref="ReadOnlyLargeList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para></exception>
        public void CopyTo(long index, T[] array, int arrayIndex, long count)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (arrayIndex + count > array.Length)
                throw new ArgumentException();

            List.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}"/> for the <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        public LargeList<T>.LargeEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }
#pragma warning disable SA1600
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
#pragma warning restore SA1600
        {
            return GetEnumerator();
        }
#pragma warning disable SA1600
        IEnumerator IEnumerable.GetEnumerator()
#pragma warning restore SA1600
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="ReadOnlyLargeList{T}"/>, if found; otherwise, -1.
        /// </returns>
        public long IndexOf(T item)
        {
            return List.IndexOf(item);
        }
#pragma warning disable SA1600
        long ILargeList.IndexOf(object item)
#pragma warning restore SA1600
        {
            return IndexOf((T)item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from <paramref name="index"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        public long IndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return List.IndexOf(item, index);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="ReadOnlyLargeList{T}"/>.</para></exception>
        public long IndexOf(T item, long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (index + count >= Count)
                throw new ArgumentOutOfRangeException();

            return List.IndexOf(item, index, count);
        }

        /// <summary>
        /// Searches the entire sorted <see cref="ReadOnlyLargeList{T}"/> for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of item in the sorted <see cref="ReadOnlyLargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="ReadOnlyLargeList{T}.Count"/>.
        /// </returns>
        public long BinarySearch(T item)
        {
            return List.BinarySearch(item);
        }

        /// <summary>
        /// Searches the entire sorted <see cref="ReadOnlyLargeList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ReadOnlyLargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="ReadOnlyLargeList{T}.Count"/>.
        /// </returns>
        public long BinarySearch(T item, IComparer<T> comparer)
        {
            return List.BinarySearch(item, comparer);
        }

        /// <summary>
        /// Searches a range of elements in the sorted <see cref="ReadOnlyLargeList{T}"/> for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}"/> implementation to use when comparing elements, or null to use the default comparer <see cref="System.Collections.Generic.Comparer{T}.Default"/>.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted <see cref="ReadOnlyLargeList{T}"/>, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of <see cref="ReadOnlyLargeList{T}.Count"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        public long BinarySearch(long index, long count, T item, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (index + count > Count)
                throw new ArgumentException();

            return List.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Converts the elements in the current <see cref="ReadOnlyLargeList{T}"/> to another type, and returns a list containing the converted elements.
        /// </summary>
        /// <param name="converter">A <see cref="System.Converter{T, TOutput}"/> delegate that converts each element from one type to another type.</param>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <returns>
        /// A <see cref="ReadOnlyLargeList{T}"/> of the target type containing the converted elements from the current <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is null.</exception>
        public LargeList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return List.ConvertAll<TOutput>(converter);
        }

        /// <summary>
        /// Determines whether the <see cref="ReadOnlyLargeList{T}"/> contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// true if the <see cref="ReadOnlyLargeList{T}"/> contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool Exists(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.Exists(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.Find(match);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A <see cref="ReadOnlyLargeList{T}"/> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public LargeList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.FindAll(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.FindIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        public long FindIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return List.FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</para><para>-or-</para><para>count is less than 0.</para><para>-or-</para><para>startIndex and count do not specify a valid section in the <see cref="ReadOnlyLargeList{T}"/>.</para></exception>
        public long FindIndex(long startIndex, long count, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (startIndex + count > Count)
                throw new ArgumentOutOfRangeException();

            return List.FindIndex(startIndex, count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T FindLast(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.FindLast(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindLastIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.FindLastIndex(match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        public long FindLastIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return List.FindLastIndex(startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the <see cref="ReadOnlyLargeList{T}"/>.</para></exception>
        public long FindLastIndex(long startIndex, long count, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (startIndex + count > Count)
                throw new ArgumentOutOfRangeException();

            return List.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="System.Action{T}"/> delegate to perform on each element of the <see cref="ReadOnlyLargeList{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            List.ForEach(action);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the entire the <see cref="ReadOnlyLargeList{T}"/>, if found; otherwise, –1.
        /// </returns>
        public long LastIndexOf(T item)
        {
            return List.LastIndexOf(item);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that extends from the first element to index, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        public long LastIndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return List.LastIndexOf(item, index);
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeList{T}"/>. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the <see cref="ReadOnlyLargeList{T}"/> that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the <see cref="ReadOnlyLargeList{T}"/>.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the <see cref="ReadOnlyLargeList{T}"/>.</para></exception>
        public long LastIndexOf(T item, long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (index + count > Count)
                throw new ArgumentOutOfRangeException();

            return List.LastIndexOf(item, index, count);
        }

        /// <summary>
        /// Determines whether every element in the <see cref="ReadOnlyLargeList{T}"/> matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The <see cref="System.Predicate{T}"/> delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// true if every element in the <see cref="ReadOnlyLargeList{T}"/> matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.TrueForAll(match);
        }

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="ReadOnlyLargeList{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// A shallow copy of a range of elements in the source <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="ReadOnlyLargeList{T}"/>.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public LargeList<T> GetRange(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (index + count > Count)
                throw new ArgumentException();

            return List.GetRange(index, count);
        }

        /// <summary>
        /// Copies the elements of the <see cref="ReadOnlyLargeList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public T[] ToArray()
        {
            return List.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyLargeList{T}"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ReadOnlyLargeList{T}"/> is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return true; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
#pragma warning disable SA1600
        bool ILargeList.IsReadOnly { get { return true; } }
#pragma warning restore SA1600

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyLargeList{T}"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ReadOnlyLargeList{T}"/> has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsFixedSize { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ReadOnlyLargeList{T}"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="ReadOnlyLargeList{T}"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="ReadOnlyLargeList{T}"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        object ILargeCollection.SyncRoot { get { return List; } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList<T>.Insert(long index, T value)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList.Insert(long index, object value)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        bool ILargeCollection<T>.Remove(T value)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList.Remove(object value)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList<T>.RemoveAt(long index)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList.RemoveAt(long index)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeCollection<T>.Add(T item)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        long ILargeList.Add(object item)
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeCollection<T>.Clear()
#pragma warning restore SA1600
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
#pragma warning disable SA1600
        void ILargeList.Clear()
#pragma warning restore SA1600
        { throw new NotSupportedException(); }

        private LargeList<T> List;
    }
}
