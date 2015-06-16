namespace Gu.Settings
{
    public interface IValueTracker : ITracker
    {
        object Value { get; }
    }
}