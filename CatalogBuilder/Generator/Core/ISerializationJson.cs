namespace MS.Internal.Generator.Core
{
    public interface ISerializationJson
    {

        T DeserializeJsonDataFile<T>(string fileName);

        string SerializeObject<T>(T jsonObject);
    }
}