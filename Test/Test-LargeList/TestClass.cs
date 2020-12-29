using System;

namespace Test
{
    public class TestClass : IComparable, IComparable<TestClass>
    {
        public TestClass(int IntegerValue, string StringValue)
        {
            this.IntegerValue = IntegerValue;
            this.StringValue = StringValue;
        }

        public int IntegerValue { get; private set; }
        public string StringValue { get; private set; }

        public int CompareTo(object obj)
        {
            TestClass Other = obj as TestClass;

            if (Other == null)
                return 1;
            else
                return CompareTo(Other);
        }

        public int CompareTo(TestClass other)
        {
            if (other == null)
                return 1;

            else if (IntegerValue > other.IntegerValue)
                return 1;

            else if (IntegerValue < other.IntegerValue)
                return -1;

            else
                return StringValue.CompareTo(other.StringValue);
        }

        public override string ToString()
        {
            return IntegerValue.ToString() + StringValue;
        }
    }
}
