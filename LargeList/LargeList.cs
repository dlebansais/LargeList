namespace LargeCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    [Serializable]
    public partial class LargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Returns a read-only <see cref="LargeCollection{T}"/> wrapper for the current list.
        /// </summary>
        /// <returns>
        /// A <see cref="ReadOnlyLargeCollection{T}"/> that acts as a read-only wrapper around the current <see cref="LargeList{T}"/>.
        /// </returns>
        public ReadOnlyLargeCollection<T> AsReadOnly()
        {
            return new ReadOnlyLargeCollection<T>(this);
        }

        /// <summary>
        /// Converts the elements in the current <see cref="LargeList{T}"/> to another type, and returns a list containing the converted elements.
        /// </summary>
        /// <param name="converter">A <see cref="System.Converter{T, TOutput}"/> delegate that converts each element from one type to another type.</param>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <returns>
        /// A <see cref="LargeList{T}"/> of the target type containing the converted elements from the current <see cref="LargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public LargeList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter is null)
                throw new ArgumentNullException(nameof(converter), "Value cannot be null.");

            LargeList<TOutput> Result = new LargeList<TOutput>(Count, Partition.MaxSegmentCapacity);

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                Result[l] = converter(Partition.GetItem(SegmentIndex, ElementIndex));
                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Copies the entire <see cref="LargeList{T}"/> to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="LargeList{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="LargeList{T}"/> is greater than the number of elements that the destination array can contain.</exception>
        public void CopyTo(T[] array)
        {
#if STRICT
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

            CopyTo(0, array, 0, Count);
        }

        /// <inheritdoc cref="ILargeCollection{T}.CopyTo(T[], int)" />
        public void CopyTo(T[] array, int arrayIndex)
        {
#if STRICT
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (arrayIndex < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
#else
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");
#endif

            CopyTo(0, array, arrayIndex, Count);
        }

        /// <inheritdoc cref="ILargeCollection.CopyTo(Array, int)" />
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
#if STRICT
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (arrayIndex < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
#else
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(arrayIndex) + " and length, and the array's lower bounds.");
#endif

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                array.SetValue(Partition.GetItem(SegmentIndex, ElementIndex), l + arrayIndex);
                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Copies a range of elements from the <see cref="LargeList{T}"/> to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source <see cref="LargeList{T}"/> at which copying begins.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from <see cref="LargeList{T}"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="arrayIndex"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> is equal to or greater than the <see cref="LargeList{T}.Count"/> of the source <see cref="LargeList{T}"/>.</para><para>-or-</para><para>The number of elements from <paramref name="index"/> to the end of the source <see cref="LargeList{T}"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para></exception>
        public void CopyTo(long index, T[] array, int arrayIndex, long count)
        {
#if STRICT
            if (array is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("dest", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (array is null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (index < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("srcIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (arrayIndex < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (count < 0)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
#endif

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

#if STRICT
            if (arrayIndex + count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check destIndex and length, and the array's lower bounds.");
#else
            if (arrayIndex + count > array.Length)
                throw new ArgumentException("Destination array was not long enough. Check " + nameof(index) + " and " + nameof(count) + ", and the array's lower bounds.");
#endif

            int SegmentIndex;
            int ElementIndex;
            int CacheIndex;
            Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

            for (long l = 0; l < count; l++)
            {
                array[l + arrayIndex] = Partition.GetItem(SegmentIndex, ElementIndex);
                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Performs the specified action on each element of the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="System.Action{T}"/> delegate to perform on each element of the <see cref="LargeList{T}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void ForEach(Action<T> action)
        {
#if STRICT
            if (action is null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly: the argument name is forced on us by a bug in Collection<T>.
                throw new ArgumentNullException("match", "Value cannot be null.");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
#else
            if (action is null)
                throw new ArgumentNullException(nameof(action), "Value cannot be null.");
#endif

            int SegmentIndex = 0;
            int ElementIndex = 0;
            for (long l = 0; l < Count; l++)
            {
                action(Partition.GetItem(SegmentIndex, ElementIndex));
                Partition.IncrementPosition(ref SegmentIndex, ref ElementIndex);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="LargeList{T}"/>.
        /// </summary>
        /// <returns>
        /// A LargeEnumerator for the <see cref="LargeList{T}"/>.
        /// </returns>
        public LargeEnumerator GetEnumerator()
        {
            return new LargeEnumerator(Partition, 0, Count);
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

        /// <summary>
        /// Creates a shallow copy of a range of elements in the source <see cref="LargeList{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based <see cref="LargeList{T}"/> index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// A shallow copy of a range of elements in the source <see cref="LargeList{T}"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the <see cref="LargeList{T}"/>.</exception>
        public LargeList<T> GetRange(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            EnumerableSegmentTable<T> EnumerableContent = new EnumerableSegmentTable<T>(Partition, index, count);
            return new LargeList<T>(EnumerableContent);
        }

        /// <summary>
        /// Copies the elements of the <see cref="LargeList{T}"/> to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the <see cref="LargeList{T}"/>.
        /// </returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public T[] ToArray()
        {
            T[] Result = new T[Count];

            CopyTo(0, Result, 0, Count);

            return Result;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the <see cref="LargeList{T}"/>, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            if (Count + 4 < Capacity)
                Partition.TrimCapacity(Capacity - Count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <inheritdoc cref="ILargeCollection{T}.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeCollection<T>.IsReadOnly => false;

        /// <inheritdoc cref="ILargeList.IsReadOnly" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeList.IsReadOnly => false;

        /// <inheritdoc cref="ILargeList.IsFixedSize" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeList.IsFixedSize => false;

        /// <inheritdoc cref="ILargeCollection.IsSynchronized" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeCollection.IsSynchronized => false;

        /// <inheritdoc cref="ILargeCollection.SyncRoot" />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        object ILargeCollection.SyncRoot => Partition;

#if STRICT
#else
        /// <summary>
        /// <para>Initializes the list.</para>
        /// <para>This method is called once, from constructors, at the begining, after parameter validation.</para>
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// <para>Creates the partition used to store elements.</para>
        /// <para>This method is called once, from constructors, at the begining, after <see cref="LargeList{T}"/>.Initialize.</para>
        /// </summary>
        /// <param name="capacity">The number of elements that the new partition can initially store.</param>
        /// <param name="count">The number of uninitialized elements that the new partition should have.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        /// <returns>
        /// The partition used to store elements.
        /// </returns>
        protected virtual IPartition<T> CreatePartition(long capacity, long count, int maxSegmentCapacity)
        {
            return new Partition<T>(capacity, count, maxSegmentCapacity);
        }
#endif

        /// <summary>
        /// Returns the number of elements of a <see cref="System.Collections.Generic.IEnumerable{T}"/> collection.
        /// </summary>
        /// <param name="collection">The collection to enumerate.</param>
        /// <returns>
        /// The number of elements in the <see cref="System.Collections.Generic.IEnumerable{T}"/> collection.
        /// </returns>
        private static long GetCollectionCount(IEnumerable<T> collection)
        {
            long CollectionCount = -1;

            if (collection is ICollection AsICollection)
                CollectionCount = AsICollection.Count;

            if (collection is ICollection<T> AsICollectionT)
                CollectionCount = AsICollectionT.Count;

            if (CollectionCount < 0)
            {
                IEnumerator<T> enumerator;

                enumerator = collection.GetEnumerator();
                CollectionCount = 0;
                while (enumerator.MoveNext())
                    CollectionCount++;
            }

            return CollectionCount;
        }

#pragma warning disable CA1859 // Use concrete types when possible for improved performance
        private IPartition<T> Partition;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

#if DEBUG
        private static void AssertInvariant()
        {
        }
#endif

        /// <summary>
        /// Enumerates the elements of a <see cref="LargeList{T}"/>.
        /// </summary>
        public struct LargeEnumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LargeEnumerator"/> struct.
            /// Creates an instance that enumerates objects over a collection of segments.
            /// </summary>
            /// <param name="partition">The collection of segments enumerated by the new enumerator.</param>
            /// <param name="index">The index of the first item to enumerate.</param>
            /// <param name="count">The number of items to enumerate.</param>
            internal LargeEnumerator(IPartition<T> partition, long index, long count)
            {
                Partition = partition;

                int SegmentIndex;
                int ElementIndex;
                int CacheIndex;
                Partition.GetPosition(index, out SegmentIndex, out ElementIndex, out CacheIndex);

                Enumerator = Partition.GetEnumerator(SegmentIndex, ElementIndex);
                Count = count;
            }

            /// <inheritdoc cref="IEnumerator{T}.Current" />
            public readonly T Current => Enumerator.Current;

            /// <inheritdoc cref="IEnumerator.Current" />
            // ! The interface is not null-aware.
            readonly object IEnumerator.Current => Current!;

            /// <inheritdoc cref="IEnumerator.Reset" />
            void IEnumerator.Reset()
            {
                // Required despite what the doc says.
                throw new NotSupportedException();
            }

            /// <inheritdoc cref="IEnumerator.MoveNext" />
            public bool MoveNext()
            {
                if (Count == 0)
                    return false;

                Count--;

                Enumerator.MoveNext(Partition);
                return true;
            }

            #region Implementation of IDisposable
            /// <inheritdoc cref="IDisposable.Dispose" />
            public void Dispose()
            {
                using (Enumerator)
                {
                }
            }
            #endregion

            private IPartition<T> Partition;
            private IPartitionEnumerator<T> Enumerator;
            private long Count;
        }
    }
}
