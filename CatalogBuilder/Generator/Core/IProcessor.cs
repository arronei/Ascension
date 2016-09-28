namespace MS.Internal.Generator.Core
{
    public interface IProcessor
    {
        T ProcessJsonObject<T, TU>(TU jsonObject);
    }
}