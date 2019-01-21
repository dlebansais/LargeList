namespace LargeList
{
    using System;

    /// <summary>
    /// Custom attribute for the assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LargeListAssemblyAttribute : Attribute
    {
#if SMALL_SEGMENTS
        /// <summary>
        /// Default max capacity for segments.
        /// </summary>
        internal const int GlobalDefaultMaxSegmentCapacity = 10;
#else
        internal const int GlobalDefaultMaxSegmentCapacity = 0x01000000;
#endif

        /// <summary>
        /// Gets or sets a value indicating whether the assembly was compiled in STRICT mode.
        /// </summary>
        public bool IsStrict { get; set; }

        /// <summary>
        /// Gets or sets the default maximum capacity of a partition's segment.
        /// </summary>
        public int DefaultMaxSegmentCapacity { get; set; } = GlobalDefaultMaxSegmentCapacity;
    }
}
