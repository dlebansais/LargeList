namespace TestLargeList
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using LargeList;
    using NUnit.Framework;

    [TestFixture]
    public class TestSet
    {
        [OneTimeSetUp]
        public static void InitTestSession()
        {
            CultureInfo enUS = CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.DefaultThreadCurrentCulture = enUS;
            CultureInfo.DefaultThreadCurrentUICulture = enUS;
            Thread.CurrentThread.CurrentCulture = enUS;
            Thread.CurrentThread.CurrentUICulture = enUS;

            Assembly LargeListAssembly;

            try
            {
                LargeListAssembly = Assembly.Load("LargeList");
            }
            catch
            {
                LargeListAssembly = null;
            }

            Assume.That(LargeListAssembly != null);

            LargeListAssemblyAttribute Attribute = LargeListAssembly.GetCustomAttribute(typeof(LargeListAssemblyAttribute)) as LargeListAssemblyAttribute;
            Assume.That(Attribute != null);

            bool IsStrict = Attribute.IsStrict;
            int DefaultMaxSegmentCapacity = Attribute.DefaultMaxSegmentCapacity;

            TestLargeList<int>.Init(IsStrict, DefaultMaxSegmentCapacity);
            TestLargeList<string>.Init(IsStrict, DefaultMaxSegmentCapacity);
            TestLargeList<TestClass>.Init(IsStrict, DefaultMaxSegmentCapacity);
        }

        private const int MaxCollectionLoops = 10;
#if DEBUG
        private const int MaxListLoops = 100;
#else
        private const int MaxListLoops = 7;
#endif

        #region int
        [Test]
        public static void TestSessionInteger_collection()
        {
            TestStatus Status;

            Status = TestLargeList<int>.Test_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionInteger_list()
        {
            TestStatus Status;

            Status = TestLargeList<int>.Test_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionInteger_readonly_collection()
        {
            TestStatus Status;

            Status = TestLargeList<int>.Test_readonly_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionInteger_readonly_list()
        {
            TestStatus Status;

            Status = TestLargeList<int>.Test_readonly_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionIntegerSimultaneous_collections(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxCollectionLoops)
                Status = TestLargeList<int>.SimultaneousTest_collections(Loop, 0, CreateInt);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionIntegerSimultaneous_lists(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            if (loopDigit1 > 2 || loopDigit0 > 2)
                return;

            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxListLoops)
                Status = TestLargeList<int>.SimultaneousTest_lists(Loop, 0, CreateInt);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        private static int CreateInt(Random rand, int maxIntValue)
        {
            return rand.Next(maxIntValue);
        }
        #endregion

        #region string
        [Test]
        public static void TestSessionString_collection()
        {
            TestStatus Status;

            Status = TestLargeList<string>.Test_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionString_list()
        {
            TestStatus Status;

            Status = TestLargeList<string>.Test_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionString_readonly_collection()
        {
            TestStatus Status;

            Status = TestLargeList<string>.Test_readonly_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionString_readonly_list()
        {
            TestStatus Status;

            Status = TestLargeList<string>.Test_readonly_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionStringSimultaneous_collections(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxCollectionLoops)
                Status = TestLargeList<string>.SimultaneousTest_collections(Loop, 0, CreateString);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionStringSimultaneous_lists(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            if (loopDigit1 > 2 || loopDigit0 > 2)
                return;

            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxListLoops)
                Status = TestLargeList<string>.SimultaneousTest_lists(Loop, 0, CreateString);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        private static string CreateString(Random rand, int maxIntValue)
        {
            return rand.Next(maxIntValue).ToString();
        }
        #endregion

        #region Generic
        [Test]
        public static void TestSessionGeneric_collection()
        {
            TestStatus Status;

            Status = TestLargeList<TestClass>.Test_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionGeneric_list()
        {
            TestStatus Status;

            Status = TestLargeList<TestClass>.Test_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionGeneric_readonly_collection()
        {
            TestStatus Status;

            Status = TestLargeList<TestClass>.Test_readonly_collection();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        public static void TestSessionGeneric_readonly_list()
        {
            TestStatus Status;

            Status = TestLargeList<TestClass>.Test_readonly_list();
            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionTestClassSimultaneous_collections(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxCollectionLoops)
                Status = TestLargeList<TestClass>.SimultaneousTest_collections(Loop, 0, CreateTestClass);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        [Test]
        [Combinatorial]
        public static void TestSessionTestClassSimultaneous_lists(
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit1,
            [Values(0, 1, 2, 3, 4, 5, 6, 7, 8, 9)] int loopDigit0)
        {
            if (loopDigit1 > 2 || loopDigit0 > 2)
                return;

            int Loop = (loopDigit1 * 10) + loopDigit0;
            TestStatus Status;

            if (Loop < MaxListLoops)
                Status = TestLargeList<TestClass>.SimultaneousTest_lists(Loop, 0, CreateTestClass);
            else
                Status = TestStatus.Success;

            Assert.That(Status.Succeeded, Status.Name);
        }

        private static TestClass CreateTestClass(Random rand, int maxIntValue)
        {
            int IntegerValue = rand.Next(maxIntValue);
            string StringValue = rand.Next(maxIntValue).ToString();
            return new TestClass(IntegerValue, StringValue);
        }
        #endregion
    }
}
