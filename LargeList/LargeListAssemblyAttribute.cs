using System;

namespace LargeList
{
    /// <summary>
    /// Custom attribute for the assembly.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LargeListAssemblyAttribute : Attribute
    {
        internal const int GlobalDefaultMaxSegmentCapacity = 10;

        /// <summary>
        /// Indicates if the assembly was compiled in STRICT mode.
        /// </summary>
        public bool IsStrict { get; set; }

        /// <summary>
        /// Reports the default maximum capacity of a partition's segment.
        /// </summary>
        public int DefaultMaxSegmentCapacity { get; set; } = GlobalDefaultMaxSegmentCapacity;
    }
}
