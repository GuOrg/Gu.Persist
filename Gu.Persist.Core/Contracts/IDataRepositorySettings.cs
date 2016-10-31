namespace Gu.Persist.Core
{
    public interface IDataRepositorySettings : IRepositorySettings
    {
        /// <summary>
        /// If true calling save with null deletes the file.
        /// </summary>
        bool SaveNullDeletesFile { get; }
    }
}