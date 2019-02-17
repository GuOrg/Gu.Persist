namespace Gu.Persist.Core
{
    /// <summary>
    /// The items before and after a rename operation.
    /// </summary>
    /// <typeparam name="T">The type of <see cref="Current"/> and <see cref="Renamed"/>.</typeparam>
    public class RenamePair<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenamePair{T}"/> class.
        /// </summary>
        /// <param name="current">The current name of the file..</param>
        /// <param name="renamed">The name of the file after the rename.</param>
        public RenamePair(T current, T renamed)
        {
            this.Current = current;
            this.Renamed = renamed;
        }

        /// <summary>
        /// Gets the current name of the file.
        /// </summary>
        public T Current { get; }

        /// <summary>
        /// Gets the name of the file after the rename.
        /// </summary>
        public T Renamed { get; }
    }
}