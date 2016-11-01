namespace Gu.Persist.Core
{
    public struct TempFileSettings : IFileSettings
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
    }
}