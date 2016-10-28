namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core.Backup;

    public sealed class SaveTransaction : IDisposable
    {
        private readonly FileInfo file;
        private readonly FileInfo tempFile;
        private readonly Stream contents;
        private readonly IBackuper backuper;
        private LockedFile lockedFile;
        private LockedFile lockedSoftDelete;
        private LockedFile lockedTempFile;

        public SaveTransaction(FileInfo file, FileInfo tempFile, Stream contents, IBackuper backuper)
        {
            this.file = file;
            this.tempFile = tempFile;
            this.contents = contents;
            this.backuper = backuper ?? NullBackuper.Default;
        }

        public void Commit()
        {
            if (!this.BeforeCopy())
            {
                return;
            }

            this.contents.CopyTo(this.lockedTempFile.Stream);
            this.AfterCopy();
        }

        public async Task CommitAsync()
        {
            if (!this.BeforeCopy())
            {
                return;
            }

            await this.contents.CopyToAsync(this.lockedTempFile.Stream)
                      .ConfigureAwait(false);
            this.AfterCopy();
        }

        public void Dispose()
        {
            this.lockedFile?.Dispose();
            this.lockedSoftDelete?.DisposeAndDeleteFile();
            this.lockedTempFile?.DisposeAndDeleteFile();
        }

        private bool BeforeCopy()
        {
            this.lockedFile = LockedFile.CreateIfExists(this.file, x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete));
            this.lockedSoftDelete = LockedFile.CreateIfExists(this.file.GetSoftDeleteFileFor(), x => x.Open(FileMode.Open, FileAccess.Read, FileShare.Delete));
            if (this.contents == null)
            {
                FileHelper.HardDelete(this.file);
                return false;
            }

            this.lockedTempFile = LockedFile.Create(this.tempFile, x => x.Open(FileMode.Create, FileAccess.Write, FileShare.Write | FileShare.Delete));
            this.backuper.BeforeSave(this.file);
            return true;
        }

        private void AfterCopy()
        {
            if (this.file.Exists)
            {
                this.backuper.Backup(this.lockedFile);
            }

            try
            {
                this.tempFile.MoveTo(this.file);
                this.lockedTempFile.DisposeAndDeleteFile();
            }
            catch (Exception exception)
            {
                try
                {
                    this.backuper.TryRestore(this.file);
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
