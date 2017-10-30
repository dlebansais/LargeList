# Status: Beta
Please wait a few days while I sort out unit tests, performance and doc.

You can download the latest build [here](https://github.com/dlebansais/LargeList/releases/download/1.0.0.3/LargeList.dll). Target framework: 4.5.2, Windows 64-bits only.

TODO list:
* [ ] Integrate unit testing with travis-ci.
* [ ] Optimize lengthy operations within segments.
* [ ] Garbage collect segments to reduce memory usage.
* [ ] Measure operation time in O(n) and compare with List&lt;>.
* [ ] Customizable segment capacity.
* [ ] Customizable use of the Large Object Heap.
* [ ] Code cleanup.

# LargeList
![Build Status](https://travis-ci.org/dlebansais/LargeList.svg?branch=master)

LargeList is an implementation of collections that can hold a number of elements limited only by the available memory, tested up to 8 billions.
The current implementation of Collection&lt;> and List&lt;> in .NET (4.6.1) can only hold up to 250 millions of reference per collection or list, but LargeList can break this barrier using a partition scheme.

## Caveats
* Because LargeList doesn't use a single array to store data, and is not integrated with .NET for optimal performance, it is slower than the standard implementation of List&lt;>. However, this is compensated by optimizations resulting from the partition scheme used, that give better performance for many operations such as Insert(), and negligible overhead for others, as long as the number of elements is large. Therefore, using LargeList is not recommended if the number of elements that will be stored is relatively low (less than 10000), and recommended if it will always be large or if performance when it is low is not a concern.
* Exposed members of the LargeList namespace have been designed to be a copy, as close as possible to the original, of the Collection&lt;> and List&lt;> classes, as well as accompanying interfaces, including documentation. However the original documentation and implementation do not match exactly, and some features of List&lt;> could be regarded as bugs. Therefore, in my own implementation I chose to diverge from the original documentation, and behave slightly differently than the original implementation. But full compatibility is available, see the STRICT mode section below.
* .NET has a ReadOnlyCollection&lt;> class, but no ReadOnlyList&lt;> class, and therefore doesn't provides a safe way to expose features such as FindLastIndex on a read-only list. The LargeList namespace includes a new class to do this.
* Downcasting a reference to a large collection or list to one of their compatible interface, and then using this interface to access the collection or list has not been tested extensively. It should work, but using the original reference to the object by its class is recommended.
* Classes of the LargeList namespace offer minimal support for inheritance, for example if one wants to back the data on disk rather than in memory. On the other hand the source code is available to compensate.
* The code is free and open without restriction whatsoever, but if it goes into production, please credit me somehow. Thank you!

## Performance
This table lists theoretical and observed performance.
The theoretical performance will never be achieved in practice because of the default partitioning, that make large *n* to really become large only with memory amount way beyond realistic. However, if you specify custom partitioning (see the customization section) it is conceivable you observe it in practice.
The red number means performance degradation: if an operation takes one second with Collection&lt;> or List&lt;>, it takes that many seconds (>1, in red) to complete with LargeList.
The green number means performance improvement: if an operation takes one second with LargeList, it takes that many seconds (>1, in green) to complete with Collection&lt;> or List&lt;>.

(TODO)

## Backward compatibility
The implementation, interface and documentation of classes and interfaces in the LargeList namespace is backward compatible as much as possible with the corresponding class or interface in .NET, when it exists. But there are some differences:
* Indexes are long (64-bits values), not int.
* When the .NET documentation specifies different names for a method parameter between an interface and its implementation, the name is identical. When the .NET actual implementation use a yet different name (this can be seen in exception messages, where the parameter name appears) the LargeList implementation use the same name as documented.
* In the case of FindLastIndex, the .NET documentation does not match the implementation. LargeList chooses a consistent choice that often will match the documentation but is not backward compatible with .NET

If backward compatibility is an issue, the code can be recompiled in STRICT mode. In this mode, everything is the same as .NET except the type of indexes. Note that you must recompile yourself, downloaded binaries do *not* use STRICT mode.

## STRICT mode
To recompile in STRICT mode (as close as possible to .NET for compatibility with existing code), open the project properties, select the "Build" tab and replace "CODE_ANALYSIS" with "CODE_ANALYSIS;STRICT" in the conditional compilation symbols.
To know if the version you use was recompiled in STRICT mode or not, include the following excerpt in your code:
```
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
In addition to the STRICT mode, one can read what's the default value for segments in the partition scheme (units on contiguous elements). Just write the same code than above but to get the value of `Attribute.DefaultMaxSegmentCapacity`.

# Examples
To use LargeList:
* Download [the lastest release](https://github.com/dlebansais/LargeList/releases/download/1.0.0.3/LargeList.dll) and save it with the name 'LargeList.dll' somewhere convenient in your project files.
* In the Solution Explorer of Visual Studio, right-click on your project and select `Add` / `Reference...`
* Select the `Browse` panel and click the `Browse...` button. Then select LargeList.dll.
* Add code similar to this in your project.

```
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
