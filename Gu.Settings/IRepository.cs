namespace Gu.Settings
{
    using System.Runtime.InteropServices.ComTypes;

    public interface IRepository
    {
        T Read<T>(string fileName);

        void Save(string fullFileName);

        void Save<T>(T setting, string fileName);

        void RestoreBackup(string fullFileName);
    }
}