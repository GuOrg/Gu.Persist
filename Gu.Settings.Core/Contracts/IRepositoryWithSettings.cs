namespace Gu.Settings.Core
{
    public interface IRepositoryWithSettings
    {
        IRepositorySettings Settings { get; }
    }
}