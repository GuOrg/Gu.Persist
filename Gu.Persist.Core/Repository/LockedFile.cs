namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [System.Diagnostics.DebuggerDisplay("{File.Name}")]
    public sealed class LockedFile : IDisposable
    {
        private LockedFile(FileInfo file, Stream stream)
        {
            this.File = file;
            this.Stream = stream;
        }

        public FileInfo File { get; }

        /// <summary>
        /// The stream locking the <see cref="File"/>
        /// Can be null if file does not exits.
        /// </summary>
        public Stream Stream { get; }

        public static async Task<LockedFile> CreateAsync(FileInfo file)
        {
            start:
            while (file.Exists)
            {
                await Task.Delay(100)
                          .ConfigureAwait(false);
                file.Refresh();
            }

            try
            {
                return Create(file, f => f.Create());
            }
            catch
            {
                goto start;
            }
        }

        public static LockedFile Create(FileInfo file, Func<FileInfo, Stream> stream)
        {
            return new LockedFile(file, stream(file));
        }

        public static LockedFile CreateIfExists(FileInfo file, Func<FileInfo, Stream> stream)
        {
            if (file == null)
            {
                return null;
            }

            if (file.Exists)
            {
                // we can never be sure that some process did not remove it after the exists check
                try
                {
                    return new LockedFile(file, stream(file));
                }
                catch (FileNotFoundException)
                {
                    return new LockedFile(file, null);
                }
            }

            return new LockedFile(file, null);
        }

        public void DisposeAndDeleteFile()
        {
            this.Stream?.Dispose();
            this.File.Delete();
        }

        public void Dispose()
        {
            this.Stream?.Dispose();
        }
    }
}