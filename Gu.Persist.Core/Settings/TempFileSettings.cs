namespace Gu.Persist.Core
{
    using System;

    /// <summary>
    /// Setting for temp files.
    /// </summary>
    public readonly struct TempFileSettings : IFileSettings, IEquatable<TempFileSettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempFileSettings"/> struct.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="extension">The file extension.</param>
        public TempFileSettings(string directory, string extension)
        {
            this.Directory = directory ?? throw new ArgumentNullException(nameof(directory));
            this.Extension = FileHelper.PrependDotIfMissing(extension ?? throw new ArgumentNullException(nameof(extension)));
        }

        /// <inheritdoc/>
        public string Directory { get; }

        /// <inheritdoc/>
        public string Extension { get; }

        /// <summary>
        /// Check if <paramref name="left"/> is equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>True if <paramref name="left"/> is equal to <paramref name="right"/>.</returns>
        public static bool operator ==(TempFileSettings left, TempFileSettings right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Check if <paramref name="left"/> is not equal to <paramref name="right"/>.
        /// </summary>
        /// <param name="left">The left instance.</param>
        /// <param name="right">The right instance.</param>
        /// <returns>True if <paramref name="left"/> is not equal to <paramref name="right"/>.</returns>
        public static bool operator !=(TempFileSettings left, TempFileSettings right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc/>
        public bool Equals(TempFileSettings other) => string.Equals(this.Directory, other.Directory, StringComparison.Ordinal) &&
                                                      string.Equals(this.Extension, other.Extension, StringComparison.Ordinal);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is TempFileSettings other &&
                                                    this.Equals(other);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Directory.GetHashCode() * 397) ^ this.Extension.GetHashCode();
            }
        }
    }
}