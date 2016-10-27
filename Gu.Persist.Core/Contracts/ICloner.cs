namespace Gu.Persist.Core
{
    public interface ICloner
    {
        T Clone<T>(T item);
    }
}
