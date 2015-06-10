namespace Gu.Settings
{
    public interface ICloner
    {
        T Clone<T>(T item);
    }
}
