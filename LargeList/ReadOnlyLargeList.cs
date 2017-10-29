using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
    /// <summary>
    /// Represents a read-only large collection of non-generic elements that can be accessed by index.
    /// </summary>
    public interface IReadOnlyLargeList : IReadOnlyLargeCollection, IEnumerable
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        object this[long index] { get; }
    }

    /// <summary>
    /// Represents a read-only large collection of elements that can be accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the read-only list. This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.</typeparam>
    public interface IReadOnlyLargeList<out T> : IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the element at the specified index in the read-only list.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>
        /// The element at the specified index in the read-only list.
        /// </returns>
        T this[long index] { get; }
    }

    /// <summary>
    /// Provides the base class for a generic read-only large list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    public class ReadOnlyLargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyLargeList&lt;T&gt; class that is a read-only wrapper around the specified list.
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
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than ReadOnlyLargeList&lt;T&gt;.Count.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return List[index];
            }
        }
        T ILargeList<T>.this[long index]
        {
            get { return this[index]; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
            set { throw new NotSupportedException(); }
        }
        object ILargeList.this[long index]
        {
            get { return this[index]; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of elements contained in the ReadOnlyLargeList&lt;T&gt; instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the ReadOnlyLargeList&lt;T&gt; instance.
        /// </returns>
        public long Count { get { return List.Count; } }
        /// <summary>
        /// Returns the ILargeList&lt;T&gt; that the ReadOnlyLargeList&lt;T&gt; wraps.
        /// </summary>
        /// <returns>
        /// The ILargeList&lt;T&gt; that the ReadOnlyLargeList&lt;T&gt; wraps.
        /// </returns>
        protected ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Determines whether an element is in the ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the ReadOnlyLargeList&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return List.Contains(item);
        }
        bool ILargeList.Contains(object item)
        {
            return Contains((T)item);
        }

        /// <summary>
        /// Copies the entire ReadOnlyLargeList&lt;T&gt; to a compatible one-dimensional System.Array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ReadOnlyLargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source ReadOnlyLargeList&lt;T&gt; is greater than the number of elements that the destination array can contain.</exception>
        public void CopyTo(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (Count > array.Length)
                throw new ArgumentException();

            List.CopyTo(array);
        }
        /// <summary>
        /// Copies the entire ReadOnlyLargeList&lt;T&gt; to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ReadOnlyLargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source ReadOnlyLargeList&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
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
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException();

            ILargeCollection AsCollection = List as ILargeCollection;
            AsCollection.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Copies a range of elements from the ReadOnlyLargeList&lt;T&gt; to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source ReadOnlyLargeList&lt;T&gt; at which copying begins.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ReadOnlyLargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="arrayIndex"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> is equal to or greater than the ReadOnlyLargeList&lt;T&gt;.Count of the source ReadOnlyLargeList&lt;T&gt;.</para><para>-or-</para><para>The number of elements from <paramref name="index"/> to the end of the source ReadOnlyLargeList&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para></exception>
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
        /// Returns an enumerator that iterates through the ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerator&lt;T&gt; for the ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        public LargeList<T>.LargeEnumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire ReadOnlyLargeList&lt;T&gt;, if found; otherwise, -1.
        /// </returns>
        public long IndexOf(T item)
        {
            return List.IndexOf(item);
        }
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item);
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from <paramref name="index"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</exception>
        public long IndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return List.IndexOf(item, index);
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the ReadOnlyLargeList&lt;T&gt; that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the ReadOnlyLargeList&lt;T&gt;.</para></exception>
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
        /// Searches the entire sorted ReadOnlyLargeList&lt;T&gt; for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of item in the sorted ReadOnlyLargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of ReadOnlyLargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="InvalidOperationException">The default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public long BinarySearch(T item)
        {
            return List.BinarySearch(item);
        }
        /// <summary>
        /// Searches the entire sorted ReadOnlyLargeList&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted ReadOnlyLargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of ReadOnlyLargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public long BinarySearch(T item, IComparer<T> comparer)
        {
            return List.BinarySearch(item, comparer);
        }
        /// <summary>
        /// Searches a range of elements in the sorted ReadOnlyLargeList&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted ReadOnlyLargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of ReadOnlyLargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the ReadOnlyLargeList&lt;T&gt;.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
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
        /// Converts the elements in the current ReadOnlyLargeList&lt;T&gt; to another type, and returns a list containing the converted elements.
        /// </summary>
        /// <param name="converter">A System.Converter&lt;T, TOutput&gt; delegate that converts each element from one type to another type.</param>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <returns>
        /// A ReadOnlyLargeList&lt;T&gt; of the target type containing the converted elements from the current ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is null.</exception>
        public LargeList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return List.ConvertAll<TOutput>(converter);
        }
        /// <summary>
        /// Determines whether the ReadOnlyLargeList&lt;T&gt; contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// true if the ReadOnlyLargeList&lt;T&gt; contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool Exists(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.Exists(match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
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
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A ReadOnlyLargeList&lt;T&gt; containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public LargeList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.FindAll(match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
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
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</exception>
        public long FindIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return List.FindIndex(startIndex, Count - startIndex, match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</para><para>-or-</para><para>count is less than 0.</para><para>-or-</para><para>startIndex and count do not specify a valid section in the ReadOnlyLargeList&lt;T&gt;.</para></exception>
        public long FindIndex(long startIndex, long count, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0 || count < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            if (startIndex + count > Count)
                throw new ArgumentOutOfRangeException();

            return List.FindIndex(startIndex, count, match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
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
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
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
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</exception>
        public long FindLastIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return List.FindLastIndex(startIndex, match);
        }
        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the ReadOnlyLargeList&lt;T&gt;.</para></exception>
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
        /// Performs the specified action on each element of the ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the ReadOnlyLargeList&lt;T&gt;.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            List.ForEach(action);
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the entire the ReadOnlyLargeList&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public long LastIndexOf(T item)
        {
            return List.LastIndexOf(item);
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the ReadOnlyLargeList&lt;T&gt; that extends from the first element to index, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</exception>
        public long LastIndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return List.LastIndexOf(item, index);
        }
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the ReadOnlyLargeList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the ReadOnlyLargeList&lt;T&gt; that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the ReadOnlyLargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the ReadOnlyLargeList&lt;T&gt;.</para></exception>
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
        /// Determines whether every element in the ReadOnlyLargeList&lt;T&gt; matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// true if every element in the ReadOnlyLargeList&lt;T&gt; matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            return List.TrueForAll(match);
        }
        /// <summary>
        /// Creates a shallow copy of a range of elements in the source ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based ReadOnlyLargeList&lt;T&gt; index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// A shallow copy of a range of elements in the source ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the ReadOnlyLargeList&lt;T&gt;.</exception>
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
        /// Copies the elements of the ReadOnlyLargeList&lt;T&gt; to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public T[] ToArray()
        {
            return List.ToArray();
        }
        /// <summary>
        /// Gets a value indicating whether the ReadOnlyLargeList&lt;T&gt; is read-only.
        /// </summary>
        /// <returns>
        /// true if the ReadOnlyLargeList&lt;T&gt; is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return true; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsReadOnly { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether the ReadOnlyLargeList&lt;T&gt; has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the ReadOnlyLargeList&lt;T&gt; has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsFixedSize { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether access to the ReadOnlyLargeList&lt;T&gt; is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the ReadOnlyLargeList&lt;T&gt; is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ReadOnlyLargeList&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the ReadOnlyLargeList&lt;T&gt;.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        object ILargeCollection.SyncRoot { get { return List; } }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList<T>.Insert(long index, T value)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList.Insert(long index, object value)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool ILargeCollection<T>.Remove(T value)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList.Remove(object value)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList<T>.RemoveAt(long index)
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList.RemoveAt(long index)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeCollection<T>.Add(T item)
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        long ILargeList.Add(object item)
        { throw new NotSupportedException(); }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeCollection<T>.Clear()
        { throw new NotSupportedException(); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void ILargeList.Clear()
        { throw new NotSupportedException(); }

        private LargeList<T> List;
    }
}
