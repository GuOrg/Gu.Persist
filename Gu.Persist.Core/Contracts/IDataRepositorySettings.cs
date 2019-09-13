namespace Gu.Persist.Core
{
    /// <summary>
    /// Settings for <see cref="DataRepository{TSetting}"/>.
    /// </summary>
    public interface IDataRepositorySettings : IRepositorySettings
    {
        /// <summary>
        /// Gets a value indicating whether save with null deletes the file.
        /// </summary>
        bool SaveNullDeletesFile { get; }
    }
}