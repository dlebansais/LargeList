namespace LargeList
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides a debugging class to display the content of generic large collections.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class LargeCollectionDebugView<T>
    {
        private ILargeCollection<T> collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeCollectionDebugView{T}"/> class.
        /// Creates a proxy for the specified collection.
        /// </summary>
        /// <param name="collection">The collection that is wrapped by the new collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public LargeCollectionDebugView(ILargeCollection<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            this.collection = collection;
        }

        /// <summary>
        /// Gets a System.Array&lt;T&gt; proxy for the ILargeCollection&lt;T&gt;.
        /// </summary>
        /// <returns>
        /// A System.Array&lt;T&gt; proxy for the ILargeCollection&lt;T&gt;.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items
        {
            get
            {
                T[] array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
        }
    }
}
