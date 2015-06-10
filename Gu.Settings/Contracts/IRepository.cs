namespace Gu.Settings
{
    using System.IO;

    public interface IRepository
    {
        IRepositorySetting Setting { get; }

        T Read<T>(string fileName);

        void Save<T>(T item, string fileName);

        T Read<T>(FileInfo file);

        void Save<T>(T item, FileInfo file);

        void Save<T>(T item, IFileInfos fileInfos);
        bool Exists<T>();
        bool Exists<T>(string fileName);
        bool Exists<T>(FileInfo file);
    }
}