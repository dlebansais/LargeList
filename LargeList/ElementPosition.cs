using System.Runtime.InteropServices;

namespace LargeList
{
    /// <summary>
    /// Describes the position of an element by its segment index and index within the segment.
    /// </summary>
#if STRICT
    internal
#else
    public
#endif
    struct ElementPosition
    {
        /// <summary>
        /// Initializes an instance of the ElementPosition class with the specified segment and element indexes.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment in the segment table containing the item at the position.</param>
        /// <param name="elementIndex">Index of the element in the segment specified by <paramref name="segmentIndex"/>.</param>
        /// <param name="cacheIndex">Index of the cache line that was used to obtain this position.</param>
        public ElementPosition(int segmentIndex, int elementIndex, int cacheIndex)
        {
            SegmentIndex = segmentIndex;
            ElementIndex = elementIndex;
            CacheIndex = cacheIndex;
        }

        /// <summary>
        /// Index of the segment where the element can be found in a partition.
        /// </summary>
        public int SegmentIndex { get; private set; }

        /// <summary>
        /// Index of the element in the segment specified by ElementPosition.SegmentIndex.
        /// </summary>
        public int ElementIndex { get; private set; }

        /// <summary>
        /// Index of the cache line that was used to obtain the position, -1 if not available.
        /// </summary>
        public int CacheIndex { get; private set; }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> is lesser than <paramref name="p2"/>; false otherwise.
        /// </returns>
        public static bool operator <(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex < p2.SegmentIndex || (p1.SegmentIndex == p2.SegmentIndex && p1.ElementIndex < p2.ElementIndex);
        }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> is greater than <paramref name="p2"/>; false otherwise.
        /// </returns>
        public static bool operator >(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex > p2.SegmentIndex || (p1.SegmentIndex == p2.SegmentIndex && p1.ElementIndex > p2.ElementIndex);
        }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> is lesser than or equal to <paramref name="p2"/>; false otherwise.
        /// </returns>
        public static bool operator <=(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex < p2.SegmentIndex || (p1.SegmentIndex == p2.SegmentIndex && p1.ElementIndex <= p2.ElementIndex);
        }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> is greater than or equal to <paramref name="p2"/>; false otherwise.
        /// </returns>
        public static bool operator >=(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex > p2.SegmentIndex || (p1.SegmentIndex == p2.SegmentIndex && p1.ElementIndex >= p2.ElementIndex);
        }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> and <paramref name="p2"/> are the same position; false otherwise.
        /// </returns>
        public static bool operator ==(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex == p2.SegmentIndex && p1.ElementIndex == p2.ElementIndex;
        }

        /// <summary>
        /// Compares the position of two elements.
        /// </summary>
        /// <param name="p1">The first position to compare.</param>
        /// <param name="p2">The second position to compare.</param>
        /// <returns>
        /// true if <paramref name="p1"/> and <paramref name="p2"/> are different positions; false otherwise.
        /// </returns>
        public static bool operator !=(ElementPosition p1, ElementPosition p2)
        {
            return p1.SegmentIndex != p2.SegmentIndex || p1.ElementIndex != p2.ElementIndex;
        }

        /// <summary>
        /// Compares this position with the position of another element.
        /// </summary>
        /// <param name="obj">The other position to compare.</param>
        /// <returns>
        /// true if <paramref name="obj"/> is the same position; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Unmodified override of base.GetHashCode().
        /// </summary>
        /// <returns>
        /// base.GetHashCode()
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Creates a simple string representation of this object.
        /// </summary>
        /// <returns>
        /// A simple string representation of this object.
        /// </returns>
        public override string ToString()
        {
            return SegmentIndex.ToString() + "," + ElementIndex.ToString() + " (" + CacheIndex + ")";
        }
    }
}
