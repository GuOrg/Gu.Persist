namespace Gu.Persist.Core
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// Specifies how to update the contents of files.
    /// For example when reading older versions.
    /// </summary>
    public abstract class Migration
    {
        /// <summary>
        /// Apply the transforms passed in.
        /// </summary>
        /// <param name="stream">The stream read from the file.</param>
        /// <param name="updated">The updated <see cref="Stream"/>.</param>
        /// <returns>True if <paramref name="stream"/> was updated.</returns>
        public abstract bool TryUpdate(Stream stream, [NotNullWhen(true)] out Stream? updated);
    }
}