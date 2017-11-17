using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LargeList
{
    /// <summary>
    /// Supports a linear storage of objects of type <typeparamref name="T"/> and fast operations to copy or move elements from a ISegment&lt;T&gt; object to another.
    /// </summary>
    /// <typeparam name="T">The type of objects to store and manipulate.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    interface ISegment<T>
    {
        /// <summary>
        /// Gets or sets the total number of elements the ISegment&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the ISegment&lt;T&gt; can contain before resizing is required.
        /// </returns>
        int Capacity { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the ISegment&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the ISegment&lt;T&gt;.
        /// </returns>
        int Count { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        T this[int index] { get; set; }

        /// <summary>
        /// Gets the number of elements that can be substracted to ISegment&lt;T&gt;.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be substracted to ISegment&lt;T&gt;.Capacity.
        /// </returns>
        int Trimmable { get; }

        /// <summary>
        /// Gets the number of elements that can be added before reaching the maximum allowed value for ISegment&lt;T&gt;.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be added before reaching the maximum allowed value for ISegment&lt;T&gt;.Capacity.
        /// </returns>
        int Extendable { get; }

        /// <summary>
        /// Determines whether an element is in the ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the ISegment&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the ISegment&lt;T&gt;; otherwise, false.
        /// </returns>
        bool Contains(T item);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the ISegment&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the ISegment&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the ISegment&lt;T&gt; that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        int IndexOf(T item, int startIndex, int count);

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the ISegment&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the ISegment&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the ISegment&lt;T&gt; that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        int LastIndexOf(T item, int startIndex, int count);

        /// <summary>
        /// Returns an enumerator that iterates through the ISegment&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An enumerator for the ISegment&lt;T&gt;.
        /// </returns>
        IEnumerator<T> GetEnumerator(long index);

        /// <summary>
        /// Removes all elements from the ISegment&lt;T&gt;. Sets the ISegment&lt;T&gt;.Count to zero.
        /// </summary>
        void Clear();

        /// <summary>
        /// Extends the number of stored elements by the ISegment&lt;T&gt; and leave them uninitialized.
        /// </summary>
        /// <param name="extended">The number of elements added to this ISegment&lt;T&gt;.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        void Extend(int extended, out int effectiveExtended);

        /// <summary>
        /// Reduces the maximum number of elements this ISegment&lt;T&gt; can store.
        /// </summary>
        /// <param name="trimmed">The number of elements that this ISegment&lt;T&gt; can no longer store and is substracted to ISegment&lt;T&gt;.Capacity.</param>
        void Trim(int trimmed);

        /// <summary>
        /// Makes room for a number of elements at the specified index. Elements already the specified position and beyond are moved toward the end of the ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        void MakeRoom(int index, int count, out int effectiveExtended);

        /// <summary>
        /// Inserts an element to the ISegment&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the ISegment&lt;T&gt;.</param>
        void Insert(int index, T item);

        /// <summary>
        /// Inserts the elements of a collection into the ISegment&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the ISegment&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <param name="count">The number of elements in <paramref name="collection"/>.</param>
        void InsertRange(int index, IEnumerable<T> collection, int count);

        /// <summary>
        /// Moves elements from this ISegment&lt;T&gt; to another from and to the specified indexes. Moved elements are replaced by default values in the source, and override existing elements in the destination.
        /// </summary>
        /// <param name="destination">The destination ISegment&lt;T&gt; object.</param>
        /// <param name="toIndex">The zero-based index at which the new elements should be moved in the destination.</param>
        /// <param name="fromIndex">The zero-based index in the source from which elements should be moved.</param>
        /// <param name="count">The number of elements to move.</param>
        void MoveTo(ISegment<T> destination, int toIndex, int fromIndex, int count);

        /// <summary>
        /// Copies elements from this ISegment&lt;T&gt; to another from and to the specified indexes. Copied elements override existing elements in the destination. If the destination ISegment&lt;T&gt;.Count of elements is lower than the index of the last copied element, it is increased to reflect the new number of elements in the destination.
        /// </summary>
        /// <param name="destination">The destination ISegment&lt;T&gt; object.</param>
        /// <param name="toIndex">The zero-based index at which the new elements should be copied in the destination.</param>
        /// <param name="fromIndex">The zero-based index in the source from which elements are copied.</param>
        /// <param name="count">The number of elements to copy.</param>
        void CopyTo(ISegment<T> destination, int toIndex, int fromIndex, int count);

        /// <summary>
        /// Removes the first occurrence of a specific object from the ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the ISegment&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the ISegment&lt;T&gt;.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes a range of elements from the ISegment&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        void RemoveRange(int index, int count);

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the ISegment&lt;T&gt;.
        /// </returns>
        int RemoveAll(Predicate<T> match);

        /// <summary>
        /// Sorts the elements in a range of elements in ISegment&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="low">The position of the first item in the range.</param>
        /// <param name="high">The position of the last item in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        void Sort(int low, int high, IComparer<T> comparer);
    }

    /// <summary>
    /// Represents a linear storage of objects of type <typeparamref name="T"/> and fast operations to copy or move elements from a Segment&lt;T&gt; object to another.
    /// </summary>
    /// <typeparam name="T">The type of stored elements.</typeparam>
#if STRICT
    internal
#else
    public
#endif
    class Segment<T> : ISegment<T>
    {
        #region Initialization
        /// <summary>
        /// Initializes a new instance of the Segment&lt;T&gt; class that is empty, has no pre-allocated capacity and has the specified maximum capacity.
        /// </summary>
        /// <param name="maxCapacity">The maximum number of elements that the new Segment&lt;T&gt; can ever store.</param>
        public Segment(int maxCapacity)
        {
            Debug.Assert(maxCapacity > 0);

            Content = new T[maxCapacity];
            MaxCapacity = maxCapacity;

            AssertInvariant();
        }

        /// <summary>
        /// Initializes a new instance of the Segment&lt;T&gt; class that is empty and has the specified initial and maximum capacities.
        /// </summary>
        /// <param name="initialCapacity">The maximum number of elements that the new Segment&lt;T&gt; can initially store.</param>
        /// <param name="maxCapacity">The maximum number of elements that the new Segment&lt;T&gt; can ever store.</param>
        public Segment(int initialCapacity, int maxCapacity)
        {
            Debug.Assert(initialCapacity >= 0);
            Debug.Assert(maxCapacity > 0);
            Debug.Assert(initialCapacity <= maxCapacity);

            Content = new T[initialCapacity];
            MaxCapacity = maxCapacity;

            AssertInvariant();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the total number of elements the Segment&lt;T&gt; can hold without resizing.
        /// </summary>
        /// <returns>
        /// The number of elements that the Segment&lt;T&gt; can contain before resizing is required.
        /// </returns>
        public int Capacity
        {
            get { return Content.Length; }
            set
            {
                Debug.Assert(value >= 0 && value <= MaxCapacity);

                Array.Resize(ref Content, value);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the Segment&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the Segment&lt;T&gt;.
        /// </returns>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>
        /// The element at the specified index.
        /// </returns>
        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0 && index < Count);

                return Content[index];
            }
            set
            {
                Debug.Assert(index >= 0 && index < Count);

                Content[index] = value;
            }
        }

        /// <summary>
        /// Gets the number of elements that can be substracted to Segment&lt;T&gt;.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be substracted to Segment&lt;T&gt;.Capacity.
        /// </returns>
        public int Trimmable
        {
            get { return Capacity - Count; }
        }

        /// <summary>
        /// Gets the number of elements that can be added before reaching the maximum allowed value for Segment&lt;T&gt;.Capacity.
        /// </summary>
        /// <returns>
        /// The number of elements that can be added before reaching the maximum allowed value for Segment&lt;T&gt;.Capacity.
        /// </returns>
        public int Extendable
        {
            get { return MaxCapacity - Count; }
        }
        #endregion

        #region Queries
        /// <summary>
        /// Determines whether an element is in the Segment&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to locate in the Segment&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the Segment&lt;T&gt;; otherwise, false.
        /// </returns>
        public bool Contains(T item)
        {
            bool Result = false;

            for (int l = 0; l < Count; l++)
                if (Content[l].Equals(item))
                {
                    Result = true;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the range of elements in the Segment&lt;T&gt; that starts at the specified index and contains the specified number of elements.
        /// </summary>
        /// <param name="item">The object to locate in the Segment&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the search. 0 (zero) is valid in an empty list.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="item"/> within the range of elements in the Segment&lt;T&gt; that starts at <paramref name="startIndex"/> and contains <paramref name="count"/> number of elements, if found; otherwise, –1.
        /// </returns>
        public int IndexOf(T item, int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0 && startIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + count <= Count);

            int Result = -1;

            for (int l = startIndex; l < startIndex + count; l++)
                if (Content[l].Equals(item))
                {
                    Result = l;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last occurrence within the range of elements in the Segment&lt;T&gt; that contains the specified number of elements and ends at the specified index.
        /// </summary>
        /// <param name="item">The object to locate in the Segment&lt;T&gt;. The value can be null for reference types.</param>
        /// <param name="startIndex">The zero-based starting index of the backward search.</param>
        /// <param name="count">The number of elements in the section to search.</param>
        /// <returns>
        /// The zero-based index of the last occurrence of <paramref name="item"/> within the range of elements in the Segment&lt;T&gt; that contains <paramref name="count"/> number of elements and ends at <paramref name="startIndex"/>, if found; otherwise, –1.
        /// </returns>
        public int LastIndexOf(T item, int startIndex, int count)
        {
            Debug.Assert(startIndex >= 0 && startIndex < Count);
            Debug.Assert(count >= 0);
            Debug.Assert(startIndex + 1 >= count);

            int Result = -1;

            for (int l = 0; l < count; l++)
                if ((item == null && Content[startIndex - l] == null) || (item != null && item.Equals(Content[startIndex - l])))
                {
                    Result = startIndex - l;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Segment&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// An enumerator for the Segment&lt;T&gt;.
        /// </returns>
        public IEnumerator<T> GetEnumerator(long index)
        {
            Debug.Assert(index >= 0 && index <= Count);

            IEnumerator<T> enumerator = GetEnumerable(Content).GetEnumerator();

            for (int i = 0; i < index; i++)
                enumerator.MoveNext();

            return enumerator;
        }

        private static IEnumerable<T> GetEnumerable(T[] content)
        {
            return content;
        }
        #endregion

        #region Commands
        /// <summary>
        /// Removes all elements from the Segment&lt;T&gt;. Sets the Segment&lt;T&gt;.Count to zero.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < Capacity; i++)
                Content[i] = default(T);

            Count = 0;

            AssertInvariant();
        }

        /// <summary>
        /// Extends the number of stored elements by the Segment&lt;T&gt; and leave them uninitialized.
        /// </summary>
        /// <param name="extended">The number of elements added to this Segment&lt;T&gt;.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        public void Extend(int extended, out int effectiveExtended)
        {
            Debug.Assert(extended >= 0);
            Debug.Assert(Count + extended <= MaxCapacity);

            if (Count + extended > Capacity)
            {
                effectiveExtended = Count + extended - Capacity;
                Array.Resize(ref Content, Count + extended);
            }
            else
                effectiveExtended = 0;

            AssertInvariant();
        }

        /// <summary>
        /// Reduces the maximum number of elements this Segment&lt;T&gt; can store.
        /// </summary>
        /// <param name="trimmed">The number of elements that this Segment&lt;T&gt; can no longer store and is substracted to Segment&lt;T&gt;.Capacity.</param>
        public void Trim(int trimmed)
        {
            Debug.Assert(trimmed >= 0);
            Debug.Assert(Count + trimmed <= Capacity);

            Array.Resize(ref Content, Capacity - trimmed);

            AssertInvariant();
        }

        /// <summary>
        /// Makes room for a number of elements at the specified index. Elements already the specified position and beyond are moved toward the end of the Segment&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based index at which uninitialized elements should be inserted.</param>
        /// <param name="count">The number of elements to insert.</param>
        /// <param name="effectiveExtended">The amount of extended capacity this operation generated.</param>
        public void MakeRoom(int index, int count, out int effectiveExtended)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(index + count <= MaxCapacity);

            Extend(count, out effectiveExtended);
            Count += count;

            for (long l = Count; l > index + count; l--)
                Content[l - 1] = Content[l - 1 - count];
        }

        /// <summary>
        /// Inserts an element to the Segment&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The object to insert into the Segment&lt;T&gt;.</param>
        public void Insert(int index, T item)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(Capacity + 1 <= MaxCapacity);

            if (Count + 1 > Capacity)
                Array.Resize(ref Content, Capacity + 1);

            for (long l = Count; l > index; l--)
                Content[l] = Content[l - 1];

            Content[index] = item;
            Count++;

            AssertInvariant();
        }

        /// <summary>
        /// Inserts the elements of a collection into the Segment&lt;T&gt; at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the Segment&lt;T&gt;. The collection itself cannot be null, but it can contain elements that are null, if type <typeparamref name="T"/> is a reference type.</param>
        /// <param name="count">The number of elements in <paramref name="collection"/>.</param>
        public void InsertRange(int index, IEnumerable<T> collection, int count)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(Count + count <= MaxCapacity);

            if (Count + count > Capacity)
                Array.Resize(ref Content, Count + count);

            for (long l = Count + count - 1; l >= index + count; l--)
                Content[l] = Content[l - count];

            foreach (T item in collection)
                Content[index++] = item;

            Count += count;

            AssertInvariant();
        }

        /// <summary>
        /// Moves elements from this Segment&lt;T&gt; to another from and to the specified indexes. Moved elements are replaced by default values in the source, and override existing elements in the destination.
        /// </summary>
        /// <param name="destination">The destination Segment&lt;T&gt; object.</param>
        /// <param name="toIndex">The zero-based index at which the new elements should be moved in the destination.</param>
        /// <param name="fromIndex">The zero-based index in the source from which elements should be moved.</param>
        /// <param name="count">The number of elements to move.</param>
        public void MoveTo(ISegment<T> destination, int toIndex, int fromIndex, int count)
        {
            Debug.Assert(destination != null);
            Debug.Assert(toIndex >= 0 && toIndex <= destination.Count);
            Debug.Assert(fromIndex >= 0 && fromIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(toIndex + count <= destination.Count);
            Debug.Assert(fromIndex + count <= Count);

            Segment<T> Destination = destination as Segment<T>;

            for (int i = 0; i < count; i++)
                Destination.Content[toIndex + i] = Content[fromIndex + i];

            for (int i = fromIndex; i < Count - count; i++)
                Content[i] = Content[i + count];

            for (int i = Count - count; i < Count; i++)
                Content[i] = default(T);

            Count -= count;

            AssertInvariant();
            Destination.AssertInvariant();
        }

        /// <summary>
        /// Copies elements from this Segment&lt;T&gt; to another from and to the specified indexes. Copied elements override existing elements in the destination. If the destination Segment&lt;T&gt;.Count of elements is lower than the index of the last copied element, it is increased to reflect the new number of elements in the destination.
        /// </summary>
        /// <param name="destination">The destination Segment&lt;T&gt; object.</param>
        /// <param name="toIndex">The zero-based index at which the new elements should be copied in the destination.</param>
        /// <param name="fromIndex">The zero-based index in the source from which elements are copied.</param>
        /// <param name="count">The number of elements to copy.</param>
        public void CopyTo(ISegment<T> destination, int toIndex, int fromIndex, int count)
        {
            Debug.Assert(destination != null);
            Debug.Assert(toIndex >= 0 && toIndex <= destination.Capacity);
            Debug.Assert(fromIndex >= 0 && fromIndex <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(toIndex + count <= destination.Capacity);
            Debug.Assert(fromIndex + count <= Count);

            Segment<T> Destination = destination as Segment<T>;

            for (int i = 0; i < count; i++)
                Destination.Content[toIndex + i] = Content[fromIndex + i];

            if (Destination.Count < toIndex + count)
                Destination.Count = toIndex + count;

            AssertInvariant();
            Destination.AssertInvariant();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the Segment&lt;T&gt;.
        /// </summary>
        /// <param name="item">The object to remove from the Segment&lt;T&gt;. The value can be null for reference types.</param>
        /// <returns>
        /// true if <paramref name="item"/> is successfully removed; otherwise, false. This method also returns false if <paramref name="item"/> was not found in the Segment&lt;T&gt;.
        /// </returns>
        public bool Remove(T item)
        {
            bool Result = false;

            for (int l = 0; l < Count; l++)
                if (Content[l].Equals(item))
                {
                    for (; l + 1 < Count; l++)
                        Content[l] = Content[l + 1];

                    Content[Count - 1] = default(T);
                    Count--;
                    Result = true;
                    break;
                }

            AssertInvariant();

            return Result;
        }

        /// <summary>
        /// Removes a range of elements from the Segment&lt;T&gt;.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public void RemoveRange(int index, int count)
        {
            Debug.Assert(index >= 0 && index <= Count);
            Debug.Assert(count >= 0);
            Debug.Assert(index + count <= Count);

            long l;

            for (l = index; l + count < Count; l++)
                Content[l] = Content[l + count];

            for (; l < Count; l++)
                Content[l] = default(T);

            Count -= count;
        }

        /// <summary>
        /// Removes all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The System.Predicate&lt;T&gt; delegate that defines the conditions of the elements to remove.</param>
        /// <returns>
        /// The number of elements removed from the Segment&lt;T&gt;.
        /// </returns>
        public int RemoveAll(Predicate<T> match)
        {
            Debug.Assert(match != null);

            int RemovedCount = 0;

            for (int l = 0; l < Count; l++)
                if (match(Content[l]))
                {
                    for (int n = l; n + 1 < Count; n++)
                        Content[n] = Content[n + 1];

                    Content[Count - 1] = default(T);
                    Count--;

                    l--;
                    RemovedCount++;
                }

            AssertInvariant();

            return RemovedCount;
        }

        /// <summary>
        /// Sorts the elements in a range of elements in Segment&lt;T&gt; using the specified comparer.
        /// </summary>
        /// <param name="low">The position of the first item in the range.</param>
        /// <param name="high">The position of the last item in the range.</param>
        /// <param name="comparer">The System.Collections.Generic.IComparer&lt;T&gt; implementation to use when comparing elements.</param>
        public void Sort(int low, int high, IComparer<T> comparer)
        {
            Debug.Assert(low >= 0 && low < Content.Length);
            Debug.Assert(high >= 0 && high < Content.Length);
            Debug.Assert(low <= high);

            Array.Sort(Content, low, high - low + 1, comparer);
        }
        #endregion

        private T[] Content;
        private int MaxCapacity;

        #region Debugging
        /// <summary>
        /// Converts this instance to its equivalent string representation.
        /// </summary>
        /// <returns>
        /// The string representation of the value of this instance.
        /// </returns>
        public override string ToString()
        {
            return Count.ToString() + " / " + Capacity.ToString();
        }
        #endregion

        #region Contracts
        private void AssertInvariant()
        {
            Debug.Assert(Capacity <= MaxCapacity);
            Debug.Assert(Count <= Capacity);
        }
        #endregion
    }
}
