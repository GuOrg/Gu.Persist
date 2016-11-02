namespace Gu.Persist.Core
{
    /// <summary>
    /// Use this when you only want one setting per type.
    /// When using this it is important that T is the same when reading and saving.
    /// </summary>
    public interface IGenericRepository
    {
        /// <summary>
        /// <see cref="IRepository.Read{T}()"/>
        /// </summary>
        T Read<T>();

        /// <summary>
        /// <see cref="IRepository.Read{T}()"/>
        /// </summary>
        void Save<T>(T item);
    }
}