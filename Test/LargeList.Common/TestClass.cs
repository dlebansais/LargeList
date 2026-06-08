namespace TestLargeList;

using System;
using System.Globalization;

internal sealed class TestClass(int integerValue, string stringValue) : IComparable, IComparable<TestClass>
{
    public int IntegerValue { get; } = integerValue;
    public string StringValue { get; } = stringValue;

    public int CompareTo(object obj)
    {
        TestClass Other = obj as TestClass;

        if (Other is null)
            return 1;
        else
            return CompareTo(Other);
    }

    public int CompareTo(TestClass other)
    {
        if (other is null)
            return 1;
        else if (IntegerValue > other.IntegerValue)
            return 1;
        else if (IntegerValue < other.IntegerValue)
            return -1;
        else
            return StringValue.CompareTo(other.StringValue, StringComparison.InvariantCulture);
    }

    public override string ToString()
    {
        return IntegerValue.ToString(CultureInfo.InvariantCulture) + StringValue;
    }
}
