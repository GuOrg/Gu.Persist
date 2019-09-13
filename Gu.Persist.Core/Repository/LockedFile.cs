namespace Gu.Persist.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// For locking files in a <see cref="SaveTransaction"/>.
    /// </summary>
    [DebuggerDisplay("{File.Name}")]
    public sealed class LockedFile : IDisposable
    {
        private LockedFile(FileInfo file, Stream stream)
        {
            this.File = file;
            this.Stream = stream;
        }

        /// <summary>
        /// The locked file.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// The stream locking the <see cref="File"/>
        /// Can be null if file does not exits.
        /// </summary>
        public Stream Stream { get; }

        /// <summary>
        /// Create a locked file.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <param name="timeout">The max time to wait.</param>
        public static async Task<LockedFile> CreateAsync(FileInfo file, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            start:
            while (file.Exists)
            {
                await Task.Delay(10)
                          .ConfigureAwait(false);
                file.Refresh();
                if (stopwatch.Elapsed > timeout)
                {
                    throw new TimeoutException($"Could not create and lock file: {file}");
                }
            }

            try
            {
                return Create(file, f => f.Create());
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                goto start;
            }
        }

        /// <summary>
        /// Create a <see cref="LockedFile"/> throws if <paramref name="file"/> does not exits.
        /// </summary>
        public static LockedFile Create(FileInfo file, Func<FileInfo, Stream> stream)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new LockedFile(file, stream(file));
        }

        /// <summary>
        /// Create a <see cref="LockedFile"/> returns null if <paramref name="file"/> does not exits.
        /// </summary>
        public static LockedFile CreateIfExists(FileInfo file, Func<FileInfo, Stream> stream)
        {
            if (file == null)
            {
                return null;
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
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

        /// <summary>
        /// Call Close on the stream.
        /// </summary>
        public void Close()
        {
            this.Stream?.Close();
        }

        /// <summary>
        /// Dispose the stream and delete the file.
        /// </summary>
        public void DisposeAndDeleteFile()
        {
            this.Dispose();
            this.File.Delete();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Stream?.Dispose();
        }
    }
}