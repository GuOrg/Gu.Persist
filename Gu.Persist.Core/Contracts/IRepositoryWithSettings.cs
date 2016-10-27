namespace Gu.Persist.Core
{
    public interface IRepositoryWithSettings
    {
        IRepositorySettings Settings { get; }
    }
}