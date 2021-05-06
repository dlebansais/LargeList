namespace LargeList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a read-only, non-generic large collection of elements.
    /// </summary>
    public interface IReadOnlyLargeCollection : IEnumerable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        long Count { get; }
    }

    /// <summary>
    /// Represents a strongly-typed, read-only large collection of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements.This type parameter is covariant. That is, you can use either the type you specified or any type that is more derived. For more information about covariance and contravariance, see Covariance and Contravariance in Generics.</typeparam>
    public interface IReadOnlyLargeCollection<out T> : IEnumerable<T>, IEnumerable
    {
        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        /// <returns>
        /// The number of elements in the collection.
        /// </returns>
        long Count { get; }
    }
}
