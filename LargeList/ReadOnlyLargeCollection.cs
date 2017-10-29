using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
    /// <summary>
    /// Represents a read-only, non-generic large collection of elements.
    /// </summary>
    public interface IReadOnlyLargeCollection : IEnumerable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        long Count { get; }
    }

    /// <summary>
    /// Represents a strongly-typed, read-only large collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements.This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.</typeparam>
    public interface IReadOnlyLargeCollection<out T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        long Count { get; }
    }

    /// <summary>
    /// Provides the base class for a generic read-only large collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    public class ReadOnlyLargeCollection<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the ReadOnlyLargeCollection&lt;T&gt; class that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        public ReadOnlyLargeCollection(ILargeList<T> list)
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
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para>index is equal to or greater than ReadOnlyLargeCollection&lt;T&gt;.Count.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

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
        /// Gets the number of elements contained in the ReadOnlyLargeCollection&lt;T&gt; instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the ReadOnlyLargeCollection&lt;T&gt; instance.
        /// </returns>
        public long Count { get { return List.Count; } }
        /// <summary>
        /// Returns the ILargeList&lt;T&gt; that the ReadOnlyLargeCollection&lt;T&gt; wraps.
        /// </summary>
        /// <returns>
        /// The ILargeList&lt;T&gt; that the ReadOnlyLargeCollection&lt;T&gt; wraps.
        /// </returns>
        protected ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Determines whether an element is in the ReadOnlyLargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="value">The object to locate in the ReadOnlyLargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if value is found in the ReadOnlyLargeCollection&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T value)
        {
            return List.Contains(value);
        }
        bool ILargeList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Copies the entire ReadOnlyLargeCollection&lt;T&gt; to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ReadOnlyLargeCollection&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source ReadOnlyLargeCollection&lt;T&gt; is greater than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        public void CopyTo(T[] array, int index)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array));
#endif

#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
#else
            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(index) + " and length, and the array's lower bounds.");
#endif

            List.CopyTo(array, index);
        }
        void ILargeCollection.CopyTo(Array array, int index)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array));
#endif

#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Number was less than the array's lower bound in the first dimension.");
#endif

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(index) + " and length, and the array's lower bounds.");

            ILargeCollection AsCollection = List as ILargeCollection;
            AsCollection.CopyTo(array, index);
        }
        /// <summary>
        /// Returns an enumerator that iterates through the ReadOnlyLargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An System.Collections.Generic.IEnumerator&lt;T&gt; for the ReadOnlyLargeCollection&lt;T&gt;.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire ReadOnlyLargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="value">The object to locate in the ReadOnlyLargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value"/> within the entire ReadOnlyLargeCollection&lt;T&gt;, if found; otherwise, -1.
        /// </returns>
        public long IndexOf(T value)
        {
            return List.IndexOf(value);
        }
        long ILargeList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire ReadOnlyLargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ReadOnlyLargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire ReadOnlyLargeCollection&lt;T&gt;, if found; otherwise, -1.
        /// </returns>
        public long IndexOf(T item)
        {
            return List.IndexOf(item);
        }
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item);
        }
#endif

        /// <summary>
        /// Gets a value indicating whether the ReadOnlyLargeCollection&lt;T&gt; is read-only.
        /// </summary>
        /// <returns>
        /// true if the ReadOnlyLargeCollection&lt;T&gt; is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return true; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsReadOnly { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether the ReadOnlyLargeCollection&lt;T&gt; has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the ReadOnlyLargeCollection&lt;T&gt; has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsFixedSize { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether access to the ReadOnlyLargeCollection&lt;T&gt; is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the ReadOnlyLargeCollection&lt;T&gt; is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the ReadOnlyLargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the ReadOnlyLargeCollection&lt;T&gt;.
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

        private ILargeList<T> List;
    }
}
