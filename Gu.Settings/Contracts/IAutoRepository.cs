namespace Gu.Settings
{
    public interface IAutoRepository
    {
        IRepositorySetting Setting { get; }

        /// <summary>
        /// Reads from file {Settings.Directory}/typeof(T).Name.{Settings.Extension}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>();

        /// <summary>
        /// Saves to file {Settings.Directory}/typeof(T).Name.{Settings.Extension}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void Save<T>(T item);
    }
}