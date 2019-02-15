namespace Gu.Persist.Core
{
    using System;

    public struct TempFileSettings : IFileSettings, IEquatable<TempFileSettings>
    {
        public TempFileSettings(string directory, string extension)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNull(extension, nameof(extension));
            this.Directory = directory;
            this.Extension = FileHelper.PrependDotIfMissing(extension);
        }

        public string Directory { get; }

        public string Extension { get; }

        public static bool operator ==(TempFileSettings left, TempFileSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TempFileSettings left, TempFileSettings right)
        {
            return !left.Equals(right);
        }

        public bool Equals(TempFileSettings other) => string.Equals(this.Directory, other.Directory, StringComparison.Ordinal) &&
                                                      string.Equals(this.Extension, other.Extension, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is TempFileSettings other &&
                                                   this.Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Directory.GetHashCode() * 397) ^ this.Extension.GetHashCode();
            }
        }
    }
}