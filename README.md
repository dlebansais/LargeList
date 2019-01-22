# LargeList
![Build Status](https://img.shields.io/travis/dlebansais/LargeList/master.svg)
[![NuGet](https://img.shields.io/nuget/v/CSharp.Largelist.svg)](https://www.nuget.org/packages/CSharp.Largelist)
[![CodeFactor](https://www.codefactor.io/repository/github/dlebansais/largelist/badge)](https://www.codefactor.io/repository/github/dlebansais/largelist)

[Download](https://github.com/dlebansais/LargeList/releases) (v1.0.0.289). Target framework: 4.5.2, Windows 64-bits only.

LargeList is an implementation of collections that can hold a number of elements limited only by the available memory, tested up to 8 billions.
The current implementation of Collection&lt;> and List&lt;> in .NET (4.6.1) can only hold up to 268 millions of reference per collection or list, but LargeList is able to break this barrier using a partition scheme.

## Caveats
* Because LargeList doesn't use a single array to store data, and is not integrated with .NET for optimal performance, it is slower than the standard implementation of List&lt;>. However, this is compensated by optimizations resulting from the partition scheme used, that give better performance for many operations such as Insert(), and acceptable overhead for others, as long as the number of elements is large. Therefore, using LargeList is not recommended if the number of elements that will be stored is relatively low (less than 10000), and recommended if it will always be large or if performance when it is low is not a concern.
* Exposed members of the LargeList namespace have been designed to be a copy, as close as possible to the original, of the Collection&lt;> and List&lt;> classes, as well as accompanying interfaces, including documentation. However the original documentation and implementation do not match exactly, and some features of List&lt;> could be regarded as bugs. Therefore, in my own implementation I chose to diverge from the original documentation, and behave slightly differently than the original implementation. But full compatibility is available, see the [STRICT mode](#strict-mode) section below.
* Downcasting a reference to a large collection or list to one of their compatible interface, and then using this interface to access the collection or list has not been tested extensively. It should work, but using the original reference to the object by its class is recommended.
* The sorting algorithm can give a slightly different result for items that are considered equal by a [IComparer](https://msdn.microsoft.com/en-us/library/system.collections.icomparer.aspx) but not by [System.Object.Equals()](https://msdn.microsoft.com/en-us/library/w4hkze5k.aspx).
* Classes of the LargeList namespace offer minimal support for inheritance, for example if one wants to back the data on disk rather than in memory. On the other hand the source code is available to compensate.
* .NET has a ReadOnlyCollection&lt;> class, but no ReadOnlyList&lt;> class, and therefore doesn't provides a safe way to expose features such as FindLastIndex on a read-only list. The LargeList namespace includes a new class to do this.
* The code is free and open without restriction whatsoever, but if it goes into production, please credit me somehow. Thank you!

## Performance
This table lists theoretical and observed performance.
The theoretical performance will never be achieved in practice because of the default partitioning, that make large *n* to really become large only with memory amount way beyond realistic. However, if you specify custom partitioning (see the customization section) it is conceivable you observe it in practice.

Method | List<> | LargeList<>
------ | ------ | -----------
get[] | O(1) | O(1)
set[] | O(1) | O(1)
Capacity | O(1) | O(1)
Count | O(1) | O(1)
Add (within capacity) | O(1) | O(1)
Add (extending capacity) | O(n) | __O(1)__
AddRange (within capacity) | O(n) | O(n)
AddRange (extending capacity) | O(n+m) | O(n+m)
AsReadOnly | O(1) | O(1)
BinarySearch | O(log(n)) | O(log(n))
Clear | O(n) | O(n)
Contains | O(n) | O(n)
ConvertAll | O(n) | O(n)
CopyTo (at begining) | O(n) | O(n)
CopyTo (at end) | O(n) | O(n)
Exists | O(n) | O(n)
Find | O(n) | O(n)
FindAll | O(n) | O(n)
FindIndex | O(n) | O(n)
FindLast | O(n) | O(n)
FindLastIndex | O(n) | O(n)
ForEach | O(n) | O(n)
GetEnumerator | O(1) | O(1)
GetRange (at begining) | O(n) | O(n)
GetRange (at end) | O(n) | O(n)
IndexOf | O(n) | O(n)
Insert (at begining) | O(n) | __O(1)__
Insert (at end) | O(1) | O(1)
InsertRange (at begining) | O(n+m) | O(n+m)
InsertRange (at end) | O(n+m) | O(n+m)
LastIndexOf | O(n) | O(n)
Remove (at begining) | O(n) | __O(1)__
Remove (at end) | O(n) | O(n)
RemoveAt | O(n) | __O(1)__
RemoveRange (at begining) | O(n) | __O(1)__
RemoveRange (at end) | O(n) | O(n)
Reverse | O(n) | O(n)
Sort (optimal case) | O(n.log(n)) | O(n.log(n))
Sort (random items) | O(n.log(n)) | O(n.log(n))
ToArray | O(n) | O(n)
TrimExcess | O(1) | __O(n)__
TrueForAll | O(n) | O(n)

Graphs of measured performance can be found [here](/Performance/performance.md).

## Backward compatibility
The implementation, interface and documentation of classes and interfaces in the LargeList namespace is backward compatible as much as possible with the corresponding class or interface in .NET, when it exists. But there are some differences:
* Indexes are long (64-bits values), not int.
* When the .NET documentation specifies different names for a method parameter between an interface and its implementation, the name is identical. When the .NET actual implementation use a yet different name (this can be seen in exception messages, where the parameter name appears) the LargeList implementation use the same name as documented.
* In the case of FindLastIndex, the .NET documentation does not match the implementation. LargeList chooses a consistent choice that often will match the documentation but is not backward compatible with .NET

If backward compatibility is an issue, the code can be recompiled in STRICT mode. In this mode, everything is the same as .NET except the type of indexes. Note that you must recompile yourself, downloaded binaries do *not* use STRICT mode.

## STRICT mode
To recompile in STRICT mode (as close as possible to .NET for compatibility with existing code), open the project properties, select the "Build" tab and replace "CODE_ANALYSIS" with "CODE_ANALYSIS;STRICT" in the conditional compilation symbols.
To know if the version you use was recompiled in STRICT mode or not, include the following excerpt in your code:

```csharp
bool? IsStrict = null;

try
{
    Assembly LargeListAssembly = Assembly.Load("LargeList");
    LargeListAssemblyAttribute Attribute = LargeListAssembly.GetCustomAttribute(typeof(LargeListAssemblyAttribute)) as LargeListAssemblyAttribute;
    IsStrict = Attribute.IsStrict;
}
catch
{
    // Handle any exception here.
}
```

## Partition default values
In addition to the STRICT mode, one can read what's the default value for segments in the partition scheme (units of contiguous elements). Just write the same code than above but to get the value of `Attribute.DefaultMaxSegmentCapacity`.

## Customization
LargeList<> has one additional constructor that takes the following arguments:
```csharp
LargeList(long capacity, long count, int maxSegmentCapacity, IEnumerable<T> collection)
```

`capacity` is the usual initial capacity of the list. You can either create the list with `count` unintialized elements or with element copied from `collection`. These two options are mutually exclusive: if `count` is greater than or equal to zero, `collection` must be null. If `collection` is not null, `count` must be negative.
`maxSegmentCapacity` is the maximum size of one segment in the partition scheme. Note that it's a number of elements, not bytes.

# Examples
To use LargeList:
1. Download the assembly.
  - Manually:
    * Download [the lastest release](https://github.com/dlebansais/LargeList/releases) and save it with the name 'LargeList.dll' somewhere convenient in your project files.
    * In the Solution Explorer of Visual Studio, right-click on your project and select `Add` / `Reference...`
    * Select the `Browse` panel and click the `Browse...` button. Then select LargeList.dll.
  - Using the NuGet package installer:
    * In Visual Studio, select the `Tools` / `NuGet Package Manager` / `Manage NuGet Packages for Solution...` menu.
    * Click __Browse__ in the top left corner and in the search bar type `LargeList`.
    * Select `CSharp.LargeList`.
    * Install it.
2. Add code similar to this in your project.

```csharp
using LargeList;

namespace Test
{
    public class TestLargeList
    {
        public TestLargeList()
        {
            LargeList<int> NewList = new LargeList<int>();
        }
    }
}
```

# Certification

This library is digitally signed with a [CAcert](https://www.cacert.org/) certificate.