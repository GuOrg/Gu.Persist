namespace Gu.Settings.Core
{
    public interface ICloner
    {
        T Clone<T>(T item);
    }
}
