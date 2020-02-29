#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core.Backup;

    internal sealed class SaveTransaction : IDisposable
    {
        private readonly FileInfo file;
        private readonly FileInfo tempFile;
        private readonly object? contents;
        private readonly IBackuper backuper;
        private LockedFile? lockedFile;
        private LockedFile? lockedSoftDelete;
        private LockedFile? lockedTempFile;
        private bool fileExistedBefore;

        internal SaveTransaction(FileInfo file, FileInfo tempFile, object? contents, IBackuper backuper)
        {
            this.file = file;
            this.tempFile = tempFile;
            this.contents = contents;
            this.backuper = backuper ?? NullBackuper.Default;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.lockedFile?.Dispose();
            this.lockedSoftDelete?.DisposeAndDeleteFile();
            this.lockedTempFile?.DisposeAndDeleteFile();
        }

        internal void Commit<TSetting>(Serialize<TSetting> serialize, TSetting setting)
        {
            this.BeforeCopy();
            if (this.contents != null &&
                this.lockedTempFile?.Stream is { } stream)
            {
                serialize.ToStream(this.contents, stream, setting);
            }

            this.AfterCopy();
        }

        internal async Task CommitAsync()
        {
            this.BeforeCopy();
            if (this.contents != null &&
                this.lockedTempFile?.Stream is { } stream)
            {
                await ((Stream)this.contents).CopyToAsync(stream)
                                             .ConfigureAwait(false);
            }

            this.AfterCopy();
        }

        private void BeforeCopy()
        {
            this.file.Refresh();
            this.fileExistedBefore = this.file.Exists;
            this.file.Directory.CreateIfNotExists();
            this.lockedFile?.Dispose();
            this.lockedFile = LockedFile.Create(this.file, x => x.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete));
            this.lockedSoftDelete?.Dispose();
            this.lockedSoftDelete = LockedFile.Create(this.file.SoftDeleteFile(), x => x.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete));
            _ = this.backuper.BeforeSave(this.file);
            if (this.contents is null)
            {
                return;
            }

            this.lockedTempFile?.Dispose();
            this.lockedTempFile = LockedFile.Create(this.tempFile, x => x.Open(FileMode.Create, FileAccess.Write, FileShare.Write | FileShare.Delete));
        }

        private void AfterCopy()
        {
            if (this.fileExistedBefore)
            {
                this.backuper.Backup(this.lockedFile!);
            }

            try
            {
                this.lockedFile!.DisposeAndDeleteFile();
                if (this.contents != null)
                {
                    _ = Kernel32.MoveFileEx(this.tempFile.FullName, this.file.FullName, MoveFileFlags.REPLACE_EXISTING);
                    this.lockedTempFile!.Dispose();
                }

                this.lockedSoftDelete!.DisposeAndDeleteFile();
            }
            catch (Exception exception)
            {
                try
                {
                    if (!this.fileExistedBefore)
                    {
                        this.lockedFile?.DisposeAndDeleteFile();
                    }

                    _ = this.backuper.TryRestore(this.file);
                }
                catch (Exception restoreException)
                {
                    throw new RestoreException(exception, restoreException);
                }

                throw;
            }

            this.backuper.AfterSave(this.lockedFile);
        }
    }
}
