namespace Gu.Persist.Core
{
    /// <summary>
    /// A repository with settings.
    /// </summary>
    public interface IRepositoryWithSettings
    {
        /// <summary>
        /// Gets the <see cref="IRepositorySettings"/>.
        /// </summary>
        IRepositorySettings Settings { get; }
    }
}