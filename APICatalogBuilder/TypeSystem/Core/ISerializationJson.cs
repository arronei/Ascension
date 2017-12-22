namespace TypeSystem.Data.Core
{
    public interface ISerializationJson
    {
        T DeserializeJsonDataFile<T>(string fileName);

        string SerializeObject<T>(T jsonObject);
    }
}