namespace Generator.Core
{
    public interface IProcessor<out T>
    {
        T ProcessJsonObject<TV>(TV jsonObject);
    }
}