﻿using LargeList;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LargeList")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("LargeList")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

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

[assembly: AssemblyVersion("1.0.0.35")]
[assembly: AssemblyFileVersion("1.0.0.44")]
