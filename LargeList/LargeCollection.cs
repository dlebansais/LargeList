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

    /// <summary>
    /// Provides the base class for a generic large collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    [Serializable]
    public class LargeCollection<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeCollection{T}"/> class.
        /// Creates an empty instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeCollection<T>.Initialize and LargeCollection<T>.CreateList")]
        public LargeCollection()
        {
#if STRICT
            List = new LargeList<T>();
#else
            Initialize();
            List = CreateList();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeCollection{T}"/> class.
        /// Creates a wrapper for the specified list.
        /// </summary>
        /// <param name="list">The list that is wrapped by the new collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeCollection<T>.Initialize")]
        public LargeCollection(ILargeList<T> list)
        {
#if STRICT
            if (list == null)
                throw new ArgumentNullException(nameof(list), "Value cannot be null.");
#else
            if (list == null)
                throw new ArgumentNullException("collection", "Value cannot be null.");
#endif

#if STRICT
#else
            Initialize();
#endif

            List = list;
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than <see cref="LargeCollection{T}"/>.Count.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                return List[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                SetItem(index, value);
            }
        }
#pragma warning disable SA1600
        object ILargeList.this[long index]
#pragma warning restore SA1600
        {
            get { return this[index] !; }
            set { this[index] = (T)value; }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// The number of elements actually contained in the <see cref="LargeCollection{T}"/>.
        /// </returns>
        public long Count { get { return List.Count; } }

        /// <summary>
        /// Gets a <see cref="ILargeList{T}"/> wrapper around the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ILargeList{T}"/> wrapper around the <see cref="LargeCollection{T}"/>.
        /// </returns>
        protected virtual ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Adds an object to the end of the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to be added to the end of the <see cref="LargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Add(T item)
        {
            InsertItem(Count, item);
        }
#pragma warning disable SA1600
        long ILargeList.Add(object value)
#pragma warning restore SA1600
        {
            long index = Count;
            InsertItem(index, (T)value);
            return index;
        }

        /// <summary>
        /// Removes all elements from the <see cref="LargeCollection{T}"/>.
        /// </summary>
        public void Clear()
        {
            ClearItems();
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="LargeCollection{T}"/>; otherwise, false.
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

#if STRICT
        /// <summary>
        /// Copies the entire <see cref="LargeCollection{T}"/> to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="LargeCollection{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="LargeCollection{T}"/> is greater than the available space from <paramref name="index"/> to the end of the destination array.</exception>
        public void CopyTo(T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");

            if (index < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            List.CopyTo(array, index);
        }
        void ILargeCollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");

            if (index < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            ILargeCollection AsCollection = Items as ILargeCollection;
            AsCollection.CopyTo(array, index);
        }
#else
        /// <summary>
        /// Copies the entire <see cref="LargeCollection{T}"/> to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="LargeCollection{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="LargeCollection{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");

            List.CopyTo(array, arrayIndex);
        }
#pragma warning disable SA1600
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
#pragma warning restore SA1600
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, arrayIndex);
        }
#endif

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Collections.Generic.IEnumerator{T}"/> for the <see cref="LargeCollection{T}"/>.
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

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="LargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire <see cref="LargeCollection{T}"/>, if found; otherwise, -1.
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
        /// Inserts an element into the <see cref="LargeCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is greater than <see cref="LargeCollection{T}"/>.Count.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Insert(long index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");

            InsertItem(index, item);
        }
#pragma warning disable SA1600
        void ILargeList.Insert(long index, object item)
#pragma warning restore SA1600
        {
            Insert(index, (T)item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="LargeCollection{T}"/>. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the original <see cref="LargeCollection{T}"/>.
        /// </returns>
        public bool Remove(T item)
        {
            long index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveItem(index);
            return true;
        }
#pragma warning disable SA1600
        void ILargeList.Remove(object item)
#pragma warning restore SA1600
        {
            Remove((T)item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than <see cref="LargeCollection{T}"/>.Count.</para></exception>
        public void RemoveAt(long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            RemoveItem(index);
        }

        /// <summary>
        /// Removes all elements from the <see cref="LargeCollection{T}"/>.
        /// </summary>
        protected virtual void ClearItems()
        {
            List.Clear();
        }

        /// <summary>
        /// Inserts an element into the <see cref="LargeCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="index"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        protected virtual void InsertItem(long index, T item)
        {
            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected virtual void RemoveItem(long index)
        {
            List.RemoveAt(index);
        }

        /// <summary>
        /// <para>Replaces the element at the specified index.</para>
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index. The value can be null for reference types.</param>
        protected virtual void SetItem(long index, T item)
        {
            List[index] = item;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="LargeCollection{T}"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="LargeCollection{T}"/> is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return List.IsReadOnly; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
#pragma warning disable SA1600
        bool ILargeList.IsReadOnly { get { return List.IsReadOnly; } }
#pragma warning restore SA1600

        /// <summary>
        /// Gets a value indicating whether access to the <see cref="LargeCollection{T}"/> is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the <see cref="LargeCollection{T}"/> is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the <see cref="LargeCollection{T}"/> has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the <see cref="LargeCollection{T}"/> has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="Same as Collection<T>")]
        bool ILargeList.IsFixedSize { get { return List.IsReadOnly; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the <see cref="LargeCollection{T}"/>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        object ILargeCollection.SyncRoot { get { return List; } }

#if STRICT
#else
        /// <summary>
        /// <para>Initializes the collection.</para>
        /// <para>This method is called once, from constructors, at the begining, after parameter validation.</para>
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// <para>Creates the list this collection wraps.</para>
        /// <para>This method is called once, from constructors, at the begining, after <see cref="LargeCollection{T}"/>.Initialize.</para>
        /// </summary>
        /// <returns>
        /// The list this collection wraps.
        /// </returns>
        protected virtual ILargeList<T> CreateList()
        {
            return new LargeList<T>();
        }
#endif

        private ILargeList<T> List;
    }
}
