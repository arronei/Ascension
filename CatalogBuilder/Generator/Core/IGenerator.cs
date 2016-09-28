namespace MS.Internal.Generator.Core
{
    public interface IGenerator : IProcessor, ISerializationJson, IWrite
    {
        T GenerateSpecificDataObject<T, TU>(TU dataObject);
    }
}