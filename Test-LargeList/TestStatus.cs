using System.Runtime.CompilerServices;

namespace Test
{
    public class TestStatus
    {
        public static TestStatus Success = new TestStatus();

        public static TestStatus Failed(string Name, [CallerFilePath] string File = "", [CallerLineNumber] int Line = 0)
        {
            return new TestStatus(Name, File, Line);
        }

        private TestStatus()
        {
            Succeeded = true;
            File = null;
            Line = -1;
        }

        private TestStatus(string Name, string File, int Line)
        {
            Succeeded = false;
            this.Name = Name;
            this.File = File;
            this.Line = Line;
        }

        public bool Succeeded { get; private set; }
        public string Name { get; private set; }
        public string File { get; private set; }
        public int Line { get; private set; }
    }
}
