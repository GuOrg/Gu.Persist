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
        private LockedFile(FileInfo file, Stream? stream)
        {
            this.File = file;
            this.Stream = stream;
        }

        /// <summary>
        /// Gets the locked file.
        /// </summary>
        public FileInfo File { get; }

        /// <summary>
        /// Gets the stream locking the <see cref="File"/>
        /// Can be null if file does not exits.
        /// </summary>
        public Stream? Stream { get; }

        /// <summary>
        /// Create a locked file.
        /// </summary>
        /// <param name="file">The file to create.</param>
        /// <param name="timeout">The max time to wait if the file exists.</param>
        /// <returns>A <see cref="LockedFile"/>.</returns>
        public static async Task<LockedFile> CreateAsync(FileInfo file, TimeSpan timeout)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var stopwatch = Stopwatch.StartNew();
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

            return Create(file, f => f.Create());
        }

        /// <summary>
        /// Create a <see cref="LockedFile"/> throws if <paramref name="file"/> does not exits.
        /// </summary>
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="stream">Specifies how the file should be locked.</param>
        /// <returns>A <see cref="LockedFile"/>.</returns>
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
        /// <param name="file">The <see cref="FileInfo"/>.</param>
        /// <param name="stream">Specifies how the file should be locked.</param>
        /// <returns>A <see cref="LockedFile"/>.</returns>
        public static LockedFile? CreateIfExists(FileInfo file, Func<FileInfo, Stream> stream)
        {
            if (file is null)
            {
                return null;
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            file.Refresh();
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