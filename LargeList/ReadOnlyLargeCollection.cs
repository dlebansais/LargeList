namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

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
    [Serializable]
    public class ReadOnlyLargeCollection<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyLargeCollection{T}"/> class.
        /// Creates an instance that is a read-only wrapper around the specified list.
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
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para>index is equal to or greater than <see cref="ReadOnlyLargeCollection{T}"/>.Count.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

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
            get { return this[index] !; }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Mot implemented")]
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ReadOnlyLargeCollection{T}"/> instance.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="ReadOnlyLargeCollection{T}"/> instance.
        /// </returns>
        public long Count { get { return List.Count; } }

        /// <summary>
        /// Gets the <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeCollection{T}"/> wraps.
        /// </summary>
        /// <returns>
        /// The <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeCollection{T}"/> wraps.
        /// </returns>
        protected virtual ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Determines whether an element is in the <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="ReadOnlyLargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if value is found in the <see cref="ReadOnlyLargeCollection{T}"/>; otherwise, false.
        /// </returns>
        public bool Contains(T value)
        {
            return List.Contains(value);
        }
#pragma warning disable SA1600
        bool ILargeList.Contains(object value)
#pragma warning restore SA1600
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Copies the entire <see cref="ReadOnlyLargeCollection{T}"/> to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="ReadOnlyLargeCollection{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ReadOnlyLargeCollection{T}"/> is greater than the available space from <paramref name="index"/> to the end of the destination array.</exception>
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
#pragma warning disable SA1600
        void ILargeCollection.CopyTo(Array array, int index)
#pragma warning restore SA1600
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

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, index);
        }
        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.Generic.IEnumerator{T}"/> for the <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }
#pragma warning disable SA1600
        IEnumerator IEnumerable.GetEnumerator()
#pragma warning restore SA1600
        {
            return GetEnumerator();
        }
#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </summary>
        /// <param name="value">The object to locate in the <see cref="ReadOnlyLargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="value"/> within the entire <see cref="ReadOnlyLargeCollection{T}"/>, if found; otherwise, -1.
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
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ReadOnlyLargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="ReadOnlyLargeCollection{T}"/>, if found; otherwise, -1.
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
#endif

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyLargeCollection{T}"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ReadOnlyLargeCollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return true; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
#pragma warning disable SA1600
        bool ILargeList.IsReadOnly { get { return true; } }
#pragma warning restore SA1600

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReadOnlyLargeCollection{T}"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="ReadOnlyLargeCollection{T}"/> has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeList.IsFixedSize { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ReadOnlyLargeCollection{T}"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="ReadOnlyLargeCollection{T}"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as ReadOnlyCollection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ReadOnlyLargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="ReadOnlyLargeCollection{T}"/>.
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

        private ILargeList<T> List;
    }
}
