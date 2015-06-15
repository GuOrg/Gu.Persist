namespace Gu.Settings
{
    public interface IRepositoryWithSettings
    {
        IRepositorySettings Settings { get; }
    }
}