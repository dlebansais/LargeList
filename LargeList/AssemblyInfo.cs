using System.Runtime.InteropServices;
using LargeList;

#if STRICT
[assembly: LargeListAssembly(IsStrict = true)]
#else
[assembly: LargeListAssembly(IsStrict = false, DefaultMaxSegmentCapacity = LargeListAssemblyAttribute.GlobalDefaultMaxSegmentCapacity)]
#endif

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("49708123-ffcc-4fda-9d6a-53d7d07fac52")]
