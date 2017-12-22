namespace Generator.Core
{
    public interface IGenerator<out T, in TU> : IProcessor<T>, IWrite
    {
        T GenerateSpecificDataObject(TU dataObject);
    }
}