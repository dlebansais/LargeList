namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Contracts;

#pragma warning disable CS1710 // XML comment has a duplicate typeparam tag
    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
#pragma warning restore CS1710 // XML comment has a duplicate typeparam tag
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    public partial class LargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeList{T}"/> class.
        /// Creates an empty instance with the default initial capacity.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        public LargeList()
        {
#if STRICT
            Partition = new Partition<T>(0, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#else
            Initialize();
            Partition = CreatePartition(0, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeList{T}"/> class.
        /// Creates an instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        public LargeList(long capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");

#if STRICT
            Partition = new Partition<T>(capacity, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#else
            Initialize();
            Partition = CreatePartition(capacity, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeList{T}"/> class.
        /// Creates an instance that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        public LargeList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Value cannot be null.");

#if STRICT
#else
            Initialize();
#endif

            long CollectionCount = GetCollectionCount(collection);

#if STRICT
            Partition = new Partition<T>(CollectionCount, CollectionCount, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#else
            Partition = CreatePartition(CollectionCount, CollectionCount, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

            Partition.SetItemRange(0, 0, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeList{T}"/> class.
        /// Creates an instance with the specified initial capacity, either uninitialized elements or elements copied from a collection, and max segment capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <param name="count">The number of uninitialized elements that the new list should start with.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
#if STRICT
        internal 
#else
        public
#endif
        LargeList(long capacity, long count, int maxSegmentCapacity, IEnumerable<T> collection)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");

            if (count < 0 && collection == null)
                throw new ArgumentException(nameof(count) + " must be greater than or equal to zero, or " + nameof(collection) + " must not be null.");

            if (count >= 0 && collection != null)
                throw new ArgumentException(nameof(count) + " can't be greater than or equal to zero if " + nameof(collection) + " is not null.");

            Debug.Assert((count >= 0 && collection == null) || (count < 0 && collection != null));
            Debug.Assert(maxSegmentCapacity > 0);
            Debug.Assert(count <= capacity);

#if STRICT
#else
            Initialize();
#endif

            if (count >= 0)
            {
#if STRICT
                Partition = new Partition<T>(capacity, count, maxSegmentCapacity);
#else
                Partition = CreatePartition(capacity, count, maxSegmentCapacity);
#endif
            }
            else
            {
                Contract.RequireNotNull(collection, out IEnumerable<T> Collection);
                long CollectionCount = GetCollectionCount(Collection);

#if STRICT
                Partition = new Partition<T>(capacity, CollectionCount, maxSegmentCapacity);
#else
                Partition = CreatePartition(capacity, CollectionCount, maxSegmentCapacity);
#endif
                Partition.SetItemRange(0, 0, Collection);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeList{T}"/> class.
        /// Creates an instance with the specified initial capacity and number of elements, and max segment capacity.
        /// This particular constructor is private.
        /// </summary>
        /// <param name="count">The number of uninitialized elements that the new list should start with.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        private LargeList(long count, int maxSegmentCapacity)
        {
            Debug.Assert(count >= 0);
            Debug.Assert(maxSegmentCapacity > 0);

#if STRICT
#else
            Initialize();
#endif

#if STRICT
            Partition = new Partition<T>(capacity, count, maxSegmentCapacity);
#else
            Partition = CreatePartition(count, count, maxSegmentCapacity);
#endif

#if DEBUG
            AssertInvariant();
#endif
        }
    }
}
