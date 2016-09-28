using System.IO;

namespace MS.Internal.Generator.Core
{
    public class BaseWrite : IWrite
    {
        public string OutputPath { get; set; }

        public void Write(string jsonString)
        {
            Write(OutputPath, jsonString);
        }

        public void Write(string path, string jsonString)
        {
            File.WriteAllText(path, jsonString);
        }
    }
}