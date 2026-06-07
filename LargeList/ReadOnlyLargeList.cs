namespace LargeCollections
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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(list);
#else
            if (list is null)
                throw new ArgumentNullException(nameof(list));
#endif

            List = list;
        }

        /// <inheritdoc cref="IReadOnlyLargeList{T}.this[long]" />
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return List[index];
            }
        }

        /// <inheritdoc cref="ILargeList{T}.this[long]" />
        T ILargeList<T>.this[long index]
        {
            get { return this[index]; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc cref="ILargeList.this[long]" />
        object ILargeList.this[long index]
        {
            // ! The interface is not null-aware.
            get => this[index]!;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
            set => throw new NotSupportedException();
        }

        /// <inheritdoc cref="ILargeCollection{T}.Count" />
        public long Count => List.Count;

        /// <summary>
        /// Gets the <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeList{T}"/> wraps.
        /// </summary>
        /// <returns>
        /// The <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeList{T}"/> wraps.
        /// </returns>
        protected virtual ILargeList<T> Items => List;

        /// <inheritdoc cref="ILargeCollection{T}.Contains(T)" />
        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        /// <inheritdoc cref="ILargeList.Contains(object)" />
        bool ILargeList.Contains(object item)
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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif

            if (Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check the array's lower bounds.");

            List.CopyTo(array);
        }

        /// <inheritdoc cref="ILargeCollection{T}.CopyTo(T[], int)" />
        public void CopyTo(T[] array, int arrayIndex)
        {
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
#endif

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check arrayIndex, and the array's lower bounds.");

            List.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc cref="ILargeCollection.CopyTo(Array, int)" />
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
#endif

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check arrayIndex, and the array's lower bounds.");

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (arrayIndex + count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check arrayIndex, and the array's lower bounds.");

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

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="ILargeList{T}.IndexOf(T)" />
        public long IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        /// <inheritdoc cref="ILargeList.IndexOf(object)" />
        long ILargeList.IndexOf(object item)
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
#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (index + count >= Count)
            {
                const Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.", InnerException);
            }

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
#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(converter);
#else
            if (converter is null)
                throw new ArgumentNullException(nameof(converter));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
#else
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (startIndex + count > Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(startIndex);
#else
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (startIndex + count > Count)
            {
                const Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.", InnerException);
            }

            return List.FindLastIndex(startIndex, count, match);
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="ReadOnlyLargeList{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="System.Action{T}"/> delegate to perform on each element of the <see cref="ReadOnlyLargeList{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void ForEach(Action<T> action)
        {
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(action);
#else
            if (action is null)
                throw new ArgumentNullException(nameof(action));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (index + count > Count)
            {
                const Exception? InnerException = null;
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from startIndex to the end of the source collection.", InnerException);
            }

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(match);
#else
            if (match is null)
                throw new ArgumentNullException(nameof(match));
#endif

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
#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(index);
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
#endif

#if NET10_0_OR_GREATER
            ArgumentOutOfRangeException.ThrowIfNegative(count);
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
#endif

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

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

        /// <inheritdoc cref="ILargeCollection{T}.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection<T>.IsReadOnly => true;

        /// <inheritdoc cref="ILargeList.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsReadOnly => true;

        /// <inheritdoc cref="ILargeList.IsFixedSize" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsFixedSize => true;

        /// <inheritdoc cref="ILargeCollection.IsSynchronized" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection.IsSynchronized => false;

        /// <inheritdoc cref="ILargeCollection.SyncRoot" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        object ILargeCollection.SyncRoot => List;

        /// <inheritdoc cref="ILargeList{T}.Insert(long, T)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList<T>.Insert(long index, T value)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList.Insert(long, object)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList.Insert(long index, object value)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeCollection{T}.Remove(T)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        bool ILargeCollection<T>.Remove(T value)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList.Remove(object)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList.Remove(object value)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList{T}.RemoveAt(long)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList<T>.RemoveAt(long index)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList.RemoveAt(long)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList.RemoveAt(long index)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeCollection{T}.Add(T)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeCollection<T>.Add(T item)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList.Add(object)" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        long ILargeList.Add(object item)
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeCollection{T}.Clear" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeCollection<T>.Clear()
        { throw new NotSupportedException(); }

        /// <inheritdoc cref="ILargeList.Clear" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
        void ILargeList.Clear()
        { throw new NotSupportedException(); }

        private LargeList<T> List;
    }
}
