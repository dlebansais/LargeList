namespace LargeCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

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
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

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
        /// Gets the <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeCollection{T}"/> wraps.
        /// </summary>
        /// <returns>
        /// The <see cref="ILargeList{T}"/> that the <see cref="ReadOnlyLargeCollection{T}"/> wraps.
        /// </returns>
        protected virtual ILargeList<T> Items => List;

        /// <inheritdoc cref="ILargeCollection{T}.Contains(T)" />
        public bool Contains(T value)
        {
            return List.Contains(value);
        }

        /// <inheritdoc cref="ILargeList.Contains(object)" />
        bool ILargeList.Contains(object value)
        {
            return Contains((T)value);
        }

        /// <inheritdoc cref="ILargeCollection{T}.CopyTo(T[], int)" />
#pragma warning disable CA1725 // Parameter names should match base declaration: the name is forced on us by a bug in Collection<T>.
        public void CopyTo(T[] array, int index)
#pragma warning restore CA1725 // Parameter names should match base declaration
        {
#if STRICT
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array, "dest");
#else
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#endif
#else
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif
#endif

#if STRICT
            if (index < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
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

        /// <inheritdoc cref="ILargeCollection.CopyTo(Array, int)" />
        void ILargeCollection.CopyTo(Array array, int index)
        {
#if STRICT
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array, "dest");
#else
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#endif
#else
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(array);
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array));
#endif
#endif

#if STRICT
            if (index < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Number was less than the array's lower bound in the first dimension.");
#endif

            if (index + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(index) + " and length, and the array's lower bounds.");

            ILargeCollection AsCollection = (ILargeCollection)Items;
            AsCollection.CopyTo(array, index);
        }

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
#if STRICT
        /// <inheritdoc cref="ILargeList{T}.IndexOf(T)" />
        public long IndexOf(T value)
        {
            return List.IndexOf(value);
        }

        /// <inheritdoc cref="ILargeList.IndexOf(object)" />
        long ILargeList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }
#else
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
#endif

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

        private ILargeList<T> List;
    }
}
