using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
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
        /// Gets the number of elements contained in the ILargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the ILargeCollection&lt;T&gt;.
        /// </returns>
        long Count { get; }

        /// <summary>
        /// Gets a value indicating whether the ILargeCollection&lt;T&gt; is read-only.
        /// </summary>
        /// <returns>
        /// true if the ILargeCollection&lt;T&gt; is read-only; otherwise, false.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Adds an item to the ILargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to add to the ILargeCollection&lt;T&gt;.</param>
        /// <exception cref="NotSupportedException">The ILargeCollection&lt;T&gt; is read-only.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Add(T item);

        /// <summary>
        /// Removes all items from the ILargeCollection&lt;T&gt;.
        /// </summary>
        /// <exception cref="NotSupportedException">The ILargeCollection&lt;T&gt; is read-only.</exception>
        void Clear();

        /// <summary>
        /// Determines whether the ILargeCollection&lt;T&gt; contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeCollection&lt;T&gt;.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the ILargeCollection&lt;T&gt;; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Copies the elements of the ILargeCollection&lt;T&gt; to a System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from ILargeCollection&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source ILargeCollection&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the ILargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the ILargeCollection&lt;T&gt;.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the ILargeCollection&lt;T&gt;; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original ILargeCollection&lt;T&gt;.
        /// </returns>
        /// <exception cref="NotSupportedException">The ILargeCollection&lt;T&gt; is read-only.</exception>
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
        /// Initializes a new instance of the LargeCollection&lt;T&gt; class that is empty.
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
        /// Initializes a new instance of the LargeCollection&lt;T&gt; class as a wrapper for the specified list.
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
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than LargeCollection&lt;T&gt;.Count.</para></exception>
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
        object ILargeList.this[long index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements actually contained in the LargeCollection&lt;T&gt;.
        /// </returns>
        public long Count { get { return List.Count; } }

        /// <summary>
        /// Gets a ILargeList&lt;T&gt; wrapper around the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// A ILargeList&lt;T&gt; wrapper around the LargeCollection&lt;T&gt;.
        /// </returns>
        protected virtual ILargeList<T> Items { get { return List; } }

        /// <summary>
        /// Adds an object to the end of the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to be added to the end of the LargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Add(T item)
        {
            InsertItem(Count, item);
        }
        long ILargeList.Add(object value)
        {
            long index = Count;
            InsertItem(index, (T)value);
            return index;
        }

        /// <summary>
        /// Removes all elements from the LargeCollection&lt;T&gt;.
        /// </summary>
        public void Clear()
        {
            ClearItems();
        }

        /// <summary>
        /// Determines whether an element is in the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the LargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the LargeCollection&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return List.Contains(item);
        }
        bool ILargeList.Contains(object item)
        {
            return Contains((T)item);
        }

#if STRICT
        /// <summary>
        /// Copies the entire LargeCollection&lt;T&gt; to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from LargeCollection&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source LargeCollection&lt;T&gt; is greater than the available space from <paramref name="index"/> to the end of the destination array.</exception>
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

            ILargeCollection AsCollection = List as ILargeCollection;
            AsCollection.CopyTo(array, index);
        }
#else
        /// <summary>
        /// Copies the entire LargeCollection&lt;T&gt; to a compatible one-dimensional System.Array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from LargeCollection&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source LargeCollection&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
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
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");

            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");

            ILargeCollection AsCollection = List as ILargeCollection;
            AsCollection.CopyTo(array, arrayIndex);
        }
#endif

        /// <summary>
        /// Returns an enumerator that iterates through the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// A System.Collections.Generic.IEnumerator&lt;T&gt; for the LargeCollection&lt;T&gt;.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire LargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the LargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire LargeCollection&lt;T&gt;, if found; otherwise, -1.
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
        /// Inserts an element into the LargeCollection&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is greater than LargeCollection&lt;T&gt;.Count.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Insert(long index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");

            InsertItem(index, item);
        }
        void ILargeList.Insert(long index, object item)
        {
            Insert(index, (T)item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the LargeCollection&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the original LargeCollection&lt;T&gt;.
        /// </returns>
        public bool Remove(T item)
        {
            long index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveItem(index);
            return true;
        }
        void ILargeList.Remove(object item)
        {
            Remove((T)item);
        }

        /// <summary>
        /// Removes the element at the specified index of the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than zero.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than LargeCollection&lt;T&gt;.Count.</para></exception>
        public void RemoveAt(long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            RemoveItem(index);
        }

        /// <summary>
        /// Removes all elements from the LargeCollection&lt;T&gt;.
        /// </summary>
        protected virtual void ClearItems()
        {
            List.Clear();
        }

        /// <summary>
        /// Inserts an element into the LargeCollection&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="index"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        protected virtual void InsertItem(long index, T item)
        {
            List.Insert(index, item);
        }

        /// <summary>
        /// Removes the element at the specified index of the LargeCollection&lt;T&gt;.
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
        /// Gets a value indicating whether the LargeCollection&lt;T&gt; is read-only.
        /// </summary>
        /// <returns>
        /// true if the LargeCollection&lt;T&gt; is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return List.IsReadOnly; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeList.IsReadOnly { get { return List.IsReadOnly; } }

        /// <summary>
        /// Gets a value indicating whether access to the LargeCollection&lt;T&gt; is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the LargeCollection&lt;T&gt; is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as Collection<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the LargeCollection&lt;T&gt; has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the LargeCollection&lt;T&gt; has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification="Same as Collection<T>")]
        bool ILargeList.IsFixedSize { get { return List.IsReadOnly; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the LargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the LargeCollection&lt;T&gt;.
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
        /// <para>This method is called once, from constructors, at the begining, after LargeCollection&lt;T&gt;.Initialize.</para>
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
