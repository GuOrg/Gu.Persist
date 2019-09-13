namespace Gu.Persist.Core
{
    /// <summary>
    /// For creating deep copies.
    /// </summary>
    public interface ICloner
    {
        /// <summary>
        /// Create a deep copy of <paramref name="item"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="item"/>.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>A deep copy of <paramref name="item"/>.</returns>
        T Clone<T>(T item);
    }
}
