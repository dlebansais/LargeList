using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
    /// <summary>
    /// Represents a nongeneric large collection of objects that can be individually accessed by index.
    /// </summary>
    public interface ILargeList : ILargeCollection, IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException">The property is set and the ILargeList is read-only.</exception>
        object this[long index] { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ILargeList has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the ILargeList has a fixed size; otherwise, false.
        /// </returns>
        bool IsFixedSize { get; }

        /// <summary>
        /// Gets a value indicating whether the ILargeList is read-only.
        /// </summary>
        /// <returns>
        /// true if the ILargeList is read-only; otherwise, false.
        /// </returns>
        bool IsReadOnly { get; }

        /// <summary>
        /// Adds an object to the ILargeList.
        /// </summary>
        /// <param name="item">The object to add to the ILargeList.</param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that <paramref name="item"/> was not inserted into the collection.
        /// </returns>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        long Add(object item);

        /// <summary>
        /// Removes all items from the ILargeList.
        /// </summary>
        /// <exception cref="NotSupportedException">The ILargeList is read-only.</exception>
        void Clear();

        /// <summary>
        /// Determines whether the ILargeList contains a specific object.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeList.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the ILargeList; otherwise, false.
        /// </returns>
        bool Contains(object item);

        /// <summary>
        /// Determines the index of a specific object in the ILargeList.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeList.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        long IndexOf(object item);

        /// <summary>
        /// Inserts an object to the ILargeList at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the ILargeList.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Insert(long index, object item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the ILargeList.
        /// </summary>
        /// <param name="item">The object to remove from the ILargeList.</param>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        void Remove(object item);

        /// <summary>
        /// Removes the ILargeList element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList.</exception>
        /// <exception cref="NotSupportedException"><para>The ILargeList is read-only.</para><para>-or-</para><para>The ILargeList has a fixed size.</para></exception>
        void RemoveAt(long index);
    }

    /// <summary>
    /// Represents a generic large collection of objects that can be individually accessed by index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public interface ILargeList<T> : ILargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList&lt;T&gt;.</exception>
        /// <exception cref="NotSupportedException">The property is set and the ILargeList&lt;T&gt; is read-only.</exception>
        T this[long index] { get; set; }

        /// <summary>
        /// Determines the index of a specific item in the ILargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ILargeList&lt;T&gt;.</param>
        /// <returns>
        /// The index of <paramref name="item"/> if found in the list; otherwise, -1.
        /// </returns>
        long IndexOf(T item);

        /// <summary>
        /// Inserts an item to the ILargeList&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the ILargeList&lt;T&gt;.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList&lt;T&gt;.</exception>
        /// <exception cref="NotSupportedException">The ILargeList&lt;T&gt; is read-only.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        void Insert(long index, T item);

        /// <summary>
        /// Removes the ILargeList&lt;T&gt; item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the ILargeList&lt;T&gt;.</exception>
        /// <exception cref="NotSupportedException">The ILargeList&lt;T&gt; is read-only.</exception>
        void RemoveAt(long index);
    }

    /// <summary>
    /// Represents a strongly typed list of objects that can be accessed by index. Provides methods to search, sort, and manipulate lists.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(LargeCollectionDebugView<>))]
    public class LargeList<T> : ILargeList<T>, ILargeCollection<T>, ILargeList, ILargeCollection, IReadOnlyLargeList<T>, IReadOnlyLargeCollection<T>, IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the LargeList&lt;T&gt; class that is empty and has the default initial capacity.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        public LargeList()
        {
#if STRICT
            Partition = new Partition<T>(0, 0);
#else
            Initialize();
            Partition = CreatePartition(0, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the LargeList&lt;T&gt; class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Totally on purpose, see the documentation of LargeList<T>.Initialize and LargeList<T>.CreatePartition")]
        public LargeList(long capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Non-negative number required.");

#if STRICT
            Partition = new Partition<T>(capacity, 0);
#else
            Initialize();
            Partition = CreatePartition(capacity, 0, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the LargeList&lt;T&gt; class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
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
            Partition = new Partition<T>(CollectionCount, CollectionCount);
#else
            Partition = CreatePartition(CollectionCount, CollectionCount, LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity);
#endif

            Partition.SetItemRange(Partition.Begin, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Initializes a new instance of the LargeList&lt;T&gt; class with the specified initial capacity, either uninitialized elements or element copied from a collection, and max segment capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new list can initially store.</param>
        /// <param name="count">The number of uninitialized elements that the new list should start with.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0</exception>
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

            Initialize();

            if (count >= 0)
                Partition = CreatePartition(capacity, count, maxSegmentCapacity);

            else
            {
                long CollectionCount = GetCollectionCount(collection);

                Partition = CreatePartition(capacity, CollectionCount, maxSegmentCapacity);
                Partition.SetItemRange(Partition.Begin, collection);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than LargeList&lt;T&gt;.Count.</para></exception>
        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                ElementPosition position = Partition.PositionOf(index);
                return Partition.GetItem(position);
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

                ElementPosition position = Partition.PositionOf(index);
                Partition.SetItem(position, value);
            }
        }
        object ILargeList.this[long index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the LargeList&lt;T&gt; can contain before resizing is required.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">LargeList&lt;T&gt;.Capacity is set to a value that is less than LargeList&lt;T&gt;.Count.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public long Capacity
        {
            get { return Partition.Capacity; }
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException(nameof(value), "capacity was less than the current size.");

                if (value > Capacity)
                    Partition.ExtendCapacity(value - Capacity);
                else if (value < Capacity)
                    Partition.TrimCapacity(Capacity - value);

#if DEBUG
                AssertInvariant();
#endif
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the LargeList&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the LargeList&lt;T&gt;.
        /// </returns>
        public long Count { get { return Partition.Count; } }

        /// <summary>
        /// Adds an object to the end of the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to be added to the end of the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Add(T item)
        {
            ElementPosition Position;
            long Index = Count;

            Position = Partition.PositionOf(Index);
            Partition.MakeRoom(Position, 1);

            Position = Partition.PositionOf(Index);
            Partition.SetItem(Position, item);

#if DEBUG
            AssertInvariant();
#endif
        }
        long ILargeList.Add(object value)
        {
            long Index = Count;
            Add((T)value);
            return Index;
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the LargeList&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Value cannot be null.");

            long CollectionCount = GetCollectionCount(collection);
            ElementPosition Position;
            long Index = Count;

            Position = Partition.PositionOf(Index);
            Partition.MakeRoom(Position, CollectionCount);

            Position = Partition.PositionOf(Index);
            Partition.SetItemRange(Position, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Returns a read-only LargeCollection&lt;T&gt; wrapper for the current list.
        /// </summary>
        /// <returns>
        /// A ReadOnlyLargeCollection&lt;T&gt; that acts as a read-only wrapper around the current LargeList&lt;T&gt;.
        /// </returns>
        public ReadOnlyLargeCollection<T> AsReadOnly()
        {
            return new ReadOnlyLargeCollection<T>(this);
        }

        /// <summary>
        /// Searches the entire sorted LargeList&lt;T&gt; for an element using the default comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted LargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of LargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="InvalidOperationException">The default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public long BinarySearch(T item)
        {
            return BinarySearchItem(0, Count, item, Comparer<T>.Default);
        }

        /// <summary>
        /// Searches the entire sorted LargeList&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted LargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of LargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public long BinarySearch(T item, IComparer<T> comparer)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;
            if (comparer == null)
                throw new InvalidOperationException();

            return BinarySearchItem(0, Count, item, comparer);
        }

        /// <summary>
        /// Searches a range of elements in the sorted LargeList&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted LargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of LargeList&lt;T&gt;.Count.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range in the LargeList&lt;T&gt;.</exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public long BinarySearch(long index, long count, T item, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            if (comparer == null)
                comparer = Comparer<T>.Default;
            if (comparer == null)
                throw new InvalidOperationException();

            return BinarySearchItem(index, count, item, comparer);
        }

        /// <summary>
        /// Removes all elements from the LargeList&lt;T&gt;.
        /// </summary>
        public void Clear()
        {
            Partition.Clear();

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Determines whether an element is in the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the LargeList&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            return Partition.Contains(item);
        }
        bool ILargeList.Contains(object item)
        {
            return Contains((T)item);
        }

        /// <summary>
        /// Converts the elements in the current LargeList&lt;T&gt; to another type, and returns a list containing the converted elements.
        /// </summary>
        /// <param name="converter">A System.Converter&lt;T, TOutput&gt; delegate that converts each element from one type to another type.</param>
        /// <typeparam name="TOutput">The type of the elements of the target array.</typeparam>
        /// <returns>
        /// A LargeList&lt;T&gt; of the target type containing the converted elements from the current LargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="converter"/> is null.</exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public LargeList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter), "Value cannot be null.");

            LargeList<TOutput> Result = new LargeList<TOutput>(Count, Count, Partition.MaxSegmentCapacity, null);

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                Result[l] = converter(Partition.GetItem(p));
                p = Partition.NextPosition(p);
            }

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }

        /// <summary>
        /// Copies the entire LargeList&lt;T&gt; to a compatible one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from LargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source LargeList&lt;T&gt; is greater than the number of elements that the destination array can contain.</exception>
        public void CopyTo(T[] array)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

            CopyTo(0, array, 0, Count);
        }

        /// <summary>
        /// Copies the entire LargeList&lt;T&gt; to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from LargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source LargeList&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
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
        void ILargeCollection.CopyTo(Array array, int arrayIndex)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
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

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                array.SetValue(Partition.GetItem(p), l + arrayIndex);
                p = Partition.NextPosition(p);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Copies a range of elements from the LargeList&lt;T&gt; to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="index">The zero-based index in the source LargeList&lt;T&gt; at which copying begins.</param>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from LargeList&lt;T&gt;. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="arrayIndex"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> is equal to or greater than the LargeList&lt;T&gt;.Count of the source LargeList&lt;T&gt;.</para><para>-or-</para><para>The number of elements from <paramref name="index"/> to the end of the source LargeList&lt;T&gt; is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination array.</para></exception>
        public void CopyTo(long index, T[] array, int arrayIndex, long count)
        {
#if STRICT
            if (array == null)
                throw new ArgumentNullException("dest", "Value cannot be null.");
#else
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Value cannot be null.");
#endif

#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("srcIndex", "Number was less than the array's lower bound in the first dimension.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("dstIndex", "Number was less than the array's lower bound in the first dimension.");
#else
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Number was less than the array's lower bound in the first dimension.");
#endif

#if STRICT
            if (count < 0)
                throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
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

            ElementPosition p = Partition.PositionOf(index);
            for (long l = 0; l < count; l++)
            {
                array[l + arrayIndex] = Partition.GetItem(p);
                p = Partition.NextPosition(p);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Determines whether the LargeList&lt;T&gt; contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// true if the LargeList&lt;T&gt; contains one or more elements that match the conditions defined by the specified predicate; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool Exists(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                if (match(Partition.GetItem(p)))
                    return true;

                p = Partition.NextPosition(p);
            }

            return false;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the first occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                T item = Partition.GetItem(p);
                if (match(item))
                    return item;

                p = Partition.NextPosition(p);
            }

            return default(T);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A LargeList&lt;T&gt; containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty LargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public LargeList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            LargeList<T> Result = new LargeList<T>();

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                T item = Partition.GetItem(p);
                if (match(item))
                    Result.Add(item);

                p = Partition.NextPosition(p);
            }

            return Result;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            return FindIndex(0, Count, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long FindIndex(long startIndex, Predicate<T> match)
        {
            if (startIndex < 0 || startIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            return FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long FindIndex(long startIndex, long count, Predicate<T> match)
        {
            if (startIndex < 0 || startIndex > Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");

            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            ElementPosition p = Partition.PositionOf(startIndex);
            for (long l = 0; l < count; l++)
            {
                if (match(Partition.GetItem(p)))
                    return l + startIndex;

                p = Partition.NextPosition(p);
            }

            return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the last occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The last element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public T FindLast(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            ElementPosition p = Partition.PositionOf(Count);
            for (long l = 0; l < Count; l++)
            {
                p = Partition.PreviousPosition(p);
                T item = Partition.GetItem(p);
                if (match(item))
                    return item;
            }

            return default(T);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long FindLastIndex(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (Count > 0)
                return FindLastIndex(Count - 1, Count, match);
            else
                return -1;
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long FindLastIndex(long startIndex, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (startIndex < -1 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate, and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the element to search for.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of an element that matches the conditions defined by <paramref name="match"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long FindLastIndex(long startIndex, long count, Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            if (startIndex < -1 || (Count == 0 && startIndex < 0) || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive and count must refer to a location within the string/array/collection.");

            if (startIndex + 1 < count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            ElementPosition p = Partition.PositionOf(startIndex);
            for (long l = 0; l < count; l++)
            {
                if (match(Partition.GetItem(p)))
                    return startIndex - l;

                p = Partition.PreviousPosition(p);
            }

            return -1;
        }

        /// <summary>
        /// Performs the specified action on each element of the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="action">The System.Action&lt;T&gt; delegate to perform on each element of the LargeList&lt;T&gt;.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null</exception>
        public void ForEach(Action<T> action)
        {
#if STRICT
            if (action == null)
                throw new ArgumentNullException("match", "Value cannot be null.");
#else
            if (action == null)
                throw new ArgumentNullException(nameof(action), "Value cannot be null.");
#endif

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                action(Partition.GetItem(p));
                p = Partition.NextPosition(p);
            }

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Returns an enumerator that iterates through the LargeList&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// A LargeEnumerator for the LargeList&lt;T&gt;.
        /// </returns>
        public LargeEnumerator GetEnumerator()
        {
            return new LargeEnumerator(Partition, 0, Count);
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
        /// Creates a shallow copy of a range of elements in the source LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based LargeList&lt;T&gt; index at which the range starts.</param>
        /// <param name="count">The number of elements in the range.</param>
        /// <returns>
        /// A shallow copy of a range of elements in the source LargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the LargeList&lt;T&gt;.</exception>
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
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the entire LargeList&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public long IndexOf(T item)
        {
            return Partition.IndexOf(item, 0, Count);
        }

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that extends from <paramref name="index"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long IndexOf(T item, long index)
        {
#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
#endif

            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.IndexOf(item, index, Count - index);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the specified index to the last element.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that extends from <paramref name="startIndex"/> to the last element, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long IndexOf(T item, long startIndex)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.IndexOf(item, startIndex, Count - startIndex);
        }
#endif

#if STRICT
            /// <summary>
            /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
            /// </summary>
            /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
            /// <param name="index">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
            /// <param name="count">The number of elements in the section to search.</param>
            /// <returns>
            /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that starts at <paramref name="index"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
            /// </returns>
            /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long IndexOf(T item, long index, long count)
        {
#if STRICT
            if (index < 0)
                throw new ArgumentOutOfRangeException("startIndex", "Index was out of range. Must be non-negative and less than the size of the collection.");
#else
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
#endif

            if (index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return Partition.IndexOf(item, index, count);
        }
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item, 0, Count);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the LargeList&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long IndexOf(T item, long startIndex, long count)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (startIndex + count > Count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return Partition.IndexOf(item, startIndex, count);
        }
        long ILargeList.IndexOf(object item)
        {
            return IndexOf((T)item, 0, Count);
        }
#endif

        /// <summary>
        /// Inserts an element into the LargeList&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is greater than LargeList&lt;T&gt;.Count.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void Insert(long index, T item)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the List.");

            ElementPosition Position;

            Position = Partition.PositionOf(index);
            Partition.MakeRoom(Position, 1);

            Position = Partition.PositionOf(index);
            Partition.SetItem(Position, item);

#if DEBUG
            AssertInvariant();
#endif
        }
        void ILargeList.Insert(long index, object item)
        {
            Insert(index, (T)item);
        }

        /// <summary>
        /// Inserts the elements of a collection into the LargeList&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the LargeList&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is greater than LargeList&lt;T&gt;.Count.</para></exception>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public void InsertRange(long index, IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), "Value cannot be null.");

            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            ElementPosition Position = Partition.PositionOf(index);
            long CollectionCount = GetCollectionCount(collection);

            Partition.MakeRoom(Position, CollectionCount);
            Position = Partition.PositionOf(index);
            Partition.SetItemRange(Position, collection);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the entire LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the entire the LargeList&lt;T&gt;, if found; otherwise, –1.
        /// </returns>
        public long LastIndexOf(T item)
        {
            if (Count > 0)
                return Partition.LastIndexOf(item, Count - 1, Count);
            else
                return -1;
        }

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that extends from the first element to <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long LastIndexOf(T item, long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.LastIndexOf(item, index, index + 1);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that extends from the first element to the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that extends from the first element to <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</exception>
        public long LastIndexOf(T item, long startIndex)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            return Partition.LastIndexOf(item, startIndex, startIndex + 1);
        }
#endif

#if STRICT
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="index">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that contains <paramref name="count"/> number of elements and ends at <paramref name="index"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long LastIndexOf(T item, long index, long count)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + 1 < count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            return Partition.LastIndexOf(item, index, count);
        }
#else
        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the LargeList&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the LargeList&lt;T&gt; that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="startIndex"/> is outside the range of valid indexes for the LargeList&lt;T&gt;.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para><para>-or-</para><para><paramref name="startIndex"/> and <paramref name="count"/> do not specify a valid section in the LargeList&lt;T&gt;.</para></exception>
        public long LastIndexOf(T item, long startIndex, long count)
        {
            if (startIndex < 0 || startIndex >= Count)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Index was out of range. Must be non-negative and less than the size of the collection.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (startIndex + 1 < count)
                throw new ArgumentOutOfRangeException("Offset and length were out of bounds for the array or count is greater than the number of elements from startIndex to the end of the source collection.");

            return Partition.LastIndexOf(item, startIndex, count);
        }
#endif

        /// <summary>
        /// Removes the first occurrence of a specific object from the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the LargeList&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the LargeList&lt;T&gt;.
        /// </returns>
        public bool Remove(T item)
        {
            bool Result = false;

            if (Partition.Remove(item))
                Result = true;

#if DEBUG
            AssertInvariant();
#endif

            return Result;
        }
        void ILargeList.Remove(object item)
        {
            Remove((T)item);
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the LargeList&lt;T&gt; .
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public long RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            long RemovedCount = Partition.RemoveAll(match);

#if DEBUG
            AssertInvariant();
#endif

            return RemovedCount;
        }

        /// <summary>
        /// Removes the element at the specified index of the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="index"/> is equal to or greater than LargeList&lt;T&gt;.Count.</para></exception>
        public void RemoveAt(long index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");

            Partition.RemoveRange(Partition.PositionOf(index), 1);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Removes a range of elements from the LargeList&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the LargeList&lt;T&gt;.</exception>
        public void RemoveRange(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            Partition.RemoveRange(Partition.PositionOf(index), count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Reverses the order of the elements in the entire LargeList&lt;T&gt;.
        /// </summary>
        public void Reverse()
        {
            Reverse(0, Count);
        }

        /// <summary>
        /// Reverses the order of the elements in the specified range.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to reverse.</param>
        /// <param name="count">The number of elements in the range to reverse.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> and <paramref name="count"/> do not denote a valid range of elements in the LargeList&lt;T&gt;.</exception>
        public void Reverse(long index, long count)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            Partition.Reverse(Partition.PositionOf(index), Partition.PositionOf(index + count), count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Sorts the elements in the entire LargeList&lt;T&gt; using the default comparer.
        /// </summary>
        /// <exception cref="InvalidOperationException">The default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find an implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public void Sort()
        {
            SortItems(0, Count, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts the elements in the entire LargeList&lt;T&gt; using the specified System.Comparison&lt;T&gt;.
        /// </summary>
        /// <param name="comparison">The System.Comparison&lt;T&gt; to use when comparing elements.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparison"/> is null.</exception>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparison"/> caused an error during the sort. For example, <paramref name="comparison"/> might not return 0 when comparing an item with itself.</exception>
        public void Sort(Comparison<T> comparison)
        {
#if STRICT
            if (comparison == null)
                throw new ArgumentNullException("match", "Value cannot be null.");
#else
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison), "Value cannot be null.");
#endif

            SortItems(0, Count, Comparer<T>.Create(comparison));
        }

        /// <summary>
        /// Sorts the elements in the entire LargeList&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        /// <exception cref="ArgumentException">The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</exception>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer == null)
                comparer = Comparer<T>.Default;
            if (comparer == null)
                throw new InvalidOperationException();

            SortItems(0, Count, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in LargeList&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default.</param>
        /// <exception cref="ArgumentOutOfRangeException"><para><paramref name="index"/> is less than 0.</para><para>-or-</para><para><paramref name="count"/> is less than 0.</para></exception>
        /// <exception cref="ArgumentException"><para><paramref name="index"/> and <paramref name="count"/> do not specify a valid range in the LargeList&lt;T&gt;.</para><para>-or-</para><para>The implementation of <paramref name="comparer"/> caused an error during the sort. For example, <paramref name="comparer"/> might not return 0 when comparing an item with itself.</para></exception>
        /// <exception cref="InvalidOperationException"><paramref name="comparer"/> is null, and the default comparer System.Collections.Generic.Comparer&lt;T&gt;.Default cannot find implementation of the System.IComparable&lt;T&gt; generic interface or the System.IComparable interface for type <typeparamref name="T"/>.</exception>
        public void Sort(long index, long count, IComparer<T> comparer)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");

            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");

            if (index + count > Count)
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");

            if (comparer == null)
                comparer = Comparer<T>.Default;
            if (comparer == null)
                throw new InvalidOperationException();

            SortItems(index, count, comparer);
        }

        /// <summary>
        /// Copies the elements of the LargeList&lt;T&gt; to a new array.
        /// </summary>
        /// <returns>
        /// An array containing copies of the elements of the LargeList&lt;T&gt;.
        /// </returns>
        /// <exception cref="OutOfMemoryException">There is not enough memory available on the system.</exception>
        public T[] ToArray()
        {
            T[] Result = new T[Count];

            CopyTo(0, Result, 0, Count);

            return Result;
        }

        /// <summary>
        /// Sets the capacity to the actual number of elements in the LargeList&lt;T&gt;, if that number is less than a threshold value.
        /// </summary>
        public void TrimExcess()
        {
            if (Count + 4 < Capacity)
                Partition.TrimCapacity(Capacity - Count);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Determines whether every element in the LargeList&lt;T&gt; matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions to check against the elements.</param>
        /// <returns>
        /// true if every element in the LargeList&lt;T&gt; matches the conditions defined by the specified predicate; otherwise, false. If the list has no elements, the return value is true.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is null.</exception>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match), "Value cannot be null.");

            ElementPosition p = Partition.PositionOf(0);
            for (long l = 0; l < Count; l++)
            {
                if (!match(Partition.GetItem(p)))
                    return false;

                p = Partition.NextPosition(p);
            }

            return true;
        }

        /// <summary>
        /// Gets a value indicating whether the LargeList&lt;T&gt; is read-only.
        /// </summary>
        /// <returns>
        /// true if the LargeList&lt;T&gt; is read-only; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeCollection<T>.IsReadOnly { get { return false; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeList.IsReadOnly { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether the LargeList&lt;T&gt; has a fixed size.
        /// </summary>
        /// <returns>
        /// true if the LargeList&lt;T&gt; has a fixed size; otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeList.IsFixedSize { get { return false; } }

        /// <summary>
        /// Gets a value indicating whether access to the LargeList&lt;T&gt; is synchronized (thread safe).
        /// </summary>
        /// <returns>
        /// true if access to the LargeList&lt;T&gt; is synchronized (thread safe); otherwise, false.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        bool ILargeCollection.IsSynchronized { get { return false; } }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the LargeList&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An object that can be used to synchronize access to the LargeList&lt;T&gt;.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Same as List<T>")]
        object ILargeCollection.SyncRoot { get { return Partition; } }

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
        /// <para>This method is called once, from constructors, at the begining, after LargeList&lt;T&gt;.Initialize.</para>
        /// </summary>
        /// <param name="capacity">The number of elements that the new partition can initially store.</param>
        /// <param name="count">The number of uninitialized elements that the new partition should have.</param>
        /// <param name="maxSegmentCapacity">The maximum size of a segment in the partition.</param>
        /// <returns>
        /// The partition used to store elements.
        /// </returns>
        protected IPartition<T> CreatePartition(long capacity, long count, int maxSegmentCapacity)
        {
            return new Partition<T>(capacity, count, maxSegmentCapacity);
        }
#endif

        /// <summary>
        /// Searches a range of elements in the sorted LargeList&lt;T&gt; for an element using the specified comparer and returns the zero-based index of the element.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to search.</param>
        /// <param name="count">The length of the range to search.</param>
        /// <param name="item">The object to locate. The value can be null for reference types.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        /// <returns>
        /// The zero-based index of <paramref name="item"/> in the sorted LargeList&lt;T&gt;, if <paramref name="item"/> is found; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <paramref name="item"/> or, if there is no larger element, the bitwise complement of LargeList&lt;T&gt;.Count.
        /// </returns>
        private long BinarySearchItem(long index, long count, T item, IComparer<T> comparer)
        {
            return Partition.BinarySearch(index, count, item, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in LargeList&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        private void SortItems(long index, long count, IComparer<T> comparer)
        {
            Partition.Sort(Partition.PositionOf(index), Partition.PositionOf(index + count), count, comparer);

#if DEBUG
            AssertInvariant();
#endif
        }

        /// <summary>
        /// Returns the number of elements of a System.Collections.Generic.IEnumerable&lt;T&gt; collection.
        /// </summary>
        /// <param name="collection">The collection to enumerate.</param>
        /// <returns>
        /// The number of elements in the System.Collections.Generic.IEnumerable&lt;T&gt; collection.
        /// </returns>
        private long GetCollectionCount(IEnumerable<T> collection)
        {
            long CollectionCount;
            ICollection AsICollection;
            ICollection<T> AsICollectionT;

            if ((AsICollection = collection as ICollection) != null)
                CollectionCount = AsICollection.Count;

            else if ((AsICollectionT = collection as ICollection<T>) != null)
                CollectionCount = AsICollectionT.Count;

            else
            {
                IEnumerator<T> enumerator;

                enumerator = collection.GetEnumerator();
                CollectionCount = 0;
                while (enumerator.MoveNext())
                    CollectionCount++;
            }

            return CollectionCount;
        }

        private IPartition<T> Partition;

        #region Contracts
        private void AssertInvariant()
        {
        }
        #endregion

        /// <summary>
        /// Enumerates the elements of a LargeList&lt;T&gt;.
        /// </summary>
        public struct LargeEnumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            /// <summary>
            /// Initializes a new instance of the LargeEnumerator class that enumerates objects over a collection of segments.
            /// </summary>
            /// <param name="partition">The collection of segments enumerated by the new enumerator.</param>
            /// <param name="index">The index of the first item to enumerate.</param>
            /// <param name="count">The number of items to enumerate.</param>
            internal LargeEnumerator(IPartition<T> partition, long index, long count)
            {
                Partition = partition;
                Enumerator = Partition.GetEnumerator(Partition.PositionOf(index));
                Count = count;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <returns>
            /// The element in the LargeList&lt;T&gt; at the current position of the enumerator.
            /// </returns>
            public T Current { get { return Enumerator.Current; } }
            object IEnumerator.Current { get { return Current; } }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// Note: this method will always throw System.NotSupportedException.
            /// </summary>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            /// <exception cref="NotSupportedException">Always.</exception>
            void IEnumerator.Reset()
            {
                // Required despite what the doc says.
                throw new NotSupportedException();
            }

            /// <summary>
            /// Advances the enumerator to the next element of the LargeList&lt;T&gt;.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            public bool MoveNext()
            {
                if (Count == 0)
                    return false;

                Count--;

                Enumerator.MoveNext(Partition);
                return true;
            }

            #region Implementation of IDisposable
            /// <summary>
            /// Releases all resources used by the LargeEnumerator.
            /// </summary>
            public void Dispose()
            {
                Partition = null;
                Enumerator = null;
            }
            #endregion

            private IPartition<T> Partition;
            private IPartitionEnumerator<T> Enumerator;
            private long Count;
        }
    }
}
