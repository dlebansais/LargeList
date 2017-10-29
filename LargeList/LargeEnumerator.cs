using System;
using System.Collections;
using System.Collections.Generic;

namespace LargeList
{
    /// <summary>
    /// Enumerates the elements of a System.Collections.Generic.List`1.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public class LargeEnumerator<T> : IEnumerator<T>, IEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the LargeEnumerator`1 class that enumerates object over an array of items.
        /// </summary>
        /// <param name="SupportArray">The array enumerated by the new enumerator.</param>
        /// <param name="Count">The number of items to enumerate.</param>
        public LargeEnumerator(T[] SupportArray, long Count)
        {
            this.SupportArray = SupportArray;
            this.Count = Count;
            Index = 0;
        }

        /// <summary>
        /// Gets the element at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the System.Collections.Generic.List`1 at the current position of the enumerator.
        /// </returns>
        public T Current { get { return SupportArray[Index]; } }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public void Reset()
        {
            Index = 0;
        }
        /// <summary>
        /// Advances the enumerator to the next element of the System.Collections.Generic.List`1.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created.</exception>
        public bool MoveNext()
        {
            if (Index + 1 >= Count)
                return false;

            Index++;
            return true;
        }
        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// Releases all resources used by the System.Collections.Generic.List`1.Enumerator.
        /// </summary>
        public void Dispose()
        {

        }

        private T[] SupportArray;
        private long Count;
        private long Index;
    }
}
