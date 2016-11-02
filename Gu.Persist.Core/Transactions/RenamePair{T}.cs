namespace Gu.Persist.Core
{
    /// <summary>
    /// The items before and after a rename operation.
    /// </summary>
    public class RenamePair<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenamePair{T}"/> class.
        /// </summary>
        public RenamePair(T current, T renamed)
        {
            this.Current = current;
            this.Renamed = renamed;
        }

        /// <summary>
        /// The current name of the file.
        /// </summary>
        public T Current { get; }

        /// <summary>
        /// The name of the file after the rename.
        /// </summary>
        public T Renamed { get; }
    }
}