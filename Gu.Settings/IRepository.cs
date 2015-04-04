namespace Gu.Settings
{
    using System.Runtime.InteropServices.ComTypes;

    public interface IRepository
    {
        T Read<T>(string fileName);

        void Save<T>(T setting, string fileName);
    }
}