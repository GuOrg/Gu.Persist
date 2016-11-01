namespace Gu.Persist.Core
{
    public class RenamePair<T>
    {
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