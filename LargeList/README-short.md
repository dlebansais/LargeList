# LargeList

LargeList is an implementation of collections that can hold a number of elements limited only by the available memory, tested up to 8 billions.
The current implementation of Collection&lt;> and List&lt;> in .NET (4.6.1) can only hold up to 268 millions of reference per collection or list, but LargeList is able to break this barrier using a partition scheme.
