namespace LargeCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;

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
            if (list is null)
                throw new ArgumentNullException(nameof(list), "Value cannot be null.");
#else
            if (list is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("collection", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#endif

#if STRICT
#else
            Initialize();
#endif

            List = list;
        }

        /// <inheritdoc cref="ILargeList{T}.this[long]" />
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

        /// <inheritdoc cref="ILargeList.this[long]" />
        object ILargeList.this[long index]
        {
            // ! The interface is not null-aware.
            get => this[index]!;
            set => this[index] = (T)value;
        }

        /// <inheritdoc cref="ILargeCollection{T}.Count" />
        public long Count => List.Count;

        /// <summary>
        /// Gets a <see cref="ILargeList{T}"/> wrapper around the <see cref="LargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="ILargeList{T}"/> wrapper around the <see cref="LargeCollection{T}"/>.
        /// </returns>
        protected virtual ILargeList<T> Items => List;

        /// <inheritdoc cref="ILargeCollection{T}.Add(T)" />
        public void Add(T item)
        {
            InsertItem(Count, item);
        }

        /// <inheritdoc cref="ILargeList.Add(object)" />
        long ILargeList.Add(object value)
        {
            long index = Count;
            InsertItem(index, (T)value);
            return index;
        }

        /// <inheritdoc cref="ILargeCollection{T}.Clear" />
        public void Clear()
        {
            ClearItems();
        }

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

#if STRICT
        /// <inheritdoc cref="ILargeCollection{T}.CopyTo(T[], int)" />
        public void CopyTo(T[] array, int index)
        {
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            if (index < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            List.CopyTo(array, index);
        }

        /// <inheritdoc cref="ILargeCollection.CopyTo(Array, int)" />
        void ILargeCollection.CopyTo(Array array, int index)
        {
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            if (index < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, index);
        }
#else
        /// <inheritdoc cref="ILargeCollection{T}.CopyTo(T[], int)" />
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");

            List.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc cref="ILargeCollection.CopyTo(Array, int)" />
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, arrayIndex);
        }
#endif

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
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

        /// <inheritdoc cref="ILargeList{T}.Insert(long, T)" />
        public void Insert(long index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");

            InsertItem(index, item);
        }

        /// <inheritdoc cref="ILargeList.Insert(long, object)" />
        void ILargeList.Insert(long index, object item)
        {
            Insert(index, (T)item);
        }

        /// <inheritdoc cref="ILargeCollection{T}.Remove(T)" />
        public bool Remove(T item)
        {
            long index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveItem(index);
            return true;
        }

        /// <inheritdoc cref="ILargeList.Remove(object)" />
        void ILargeList.Remove(object item)
        {
            Remove((T)item);
        }

        /// <inheritdoc cref="ILargeList{T}.RemoveAt(long)" />
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

        /// <inheritdoc cref="ILargeCollection{T}.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection<T>.IsReadOnly => List.IsReadOnly;

        /// <inheritdoc cref="ILargeList.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeList.IsReadOnly => List.IsReadOnly;

        /// <inheritdoc cref="ILargeCollection.IsSynchronized" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection.IsSynchronized => false;

        /// <inheritdoc cref="ILargeList.IsFixedSize" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="Same as Collection<T>")]
        bool ILargeList.IsFixedSize => List.IsReadOnly;

        /// <inheritdoc cref="ILargeCollection.SyncRoot" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        object ILargeCollection.SyncRoot => List;

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
