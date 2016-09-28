namespace MS.Internal.Generator.Core
{
    public interface IWrite
    {
        string OutputPath { get; set; }

        void Write(string jsonString);

        void Write(string path, string jsonString);
    }
}