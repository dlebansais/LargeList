namespace TestLargeList
{
    using System.Runtime.CompilerServices;

    public class TestStatus
    {
        public static TestStatus Success { get; } = new TestStatus();

        public static TestStatus Failed(string name, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            return new TestStatus(name, file, line);
        }

        private TestStatus()
        {
            Succeeded = true;
            File = null;
            Line = -1;
        }

        private TestStatus(string name, string file, int line)
        {
            Succeeded = false;
            Name = name;
            File = file;
            Line = line;
        }

        public bool Succeeded { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }
        public int Line { get; private set; }
    }
}
