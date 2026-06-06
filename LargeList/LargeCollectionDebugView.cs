namespace LargeCollections
{
    using System;
    using System.Diagnostics;
    using System.Linq;

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
#if NET10_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(collection);
#else
            if (collection is null)
                throw new ArgumentNullException(nameof(collection));
#endif

            this.collection = collection;
        }

        /// <summary>
        /// Gets a <see cref="System.Array"/> proxy for the <see cref="ILargeCollection{T}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Array"/> proxy for the <see cref="ILargeCollection{T}"/>.
        /// </returns>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public ReadOnlySpan<T> Items
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
