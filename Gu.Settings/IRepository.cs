namespace Gu.Settings
{
    public interface IRepository
    {
        T Read<T>(string fileName = null);

        void Save<T>(T setting, bool createBackup, string fileName = null);

        RepositorySetting Setting { get; }
    }
}