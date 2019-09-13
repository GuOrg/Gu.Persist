namespace Gu.Persist.Core
{
    public interface IDataRepositorySettings : IRepositorySettings
    {
        /// <summary>
        /// Gets a value indicating whether save with null deletes the file.
        /// </summary>
        bool SaveNullDeletesFile { get; }
    }
}