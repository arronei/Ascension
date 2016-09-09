namespace MS.Internal
{
    public interface IGenerator : IDeserializeJson
    {
        string OutputPath { get; set; }

        T GenerateJsonObject<T, TU>(TU dataObject);

        string SerializeObject<T>(T jsonObject);

        void WriteFile(string jsonString);

        void WriteFile(string path, string jsonString);
    }

    public interface IDeserializeJson
    {
        TU DeserializeJsonDataFile<TU>(string fileName);
    }
}