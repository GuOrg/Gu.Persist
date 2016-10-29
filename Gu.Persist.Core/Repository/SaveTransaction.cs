﻿namespace Gu.Persist.Core
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gu.Persist.Core.Backup;

    internal sealed class SaveTransaction : IDisposable
    {
        private readonly FileInfo file;
        private readonly FileInfo tempFile;
        private readonly object contents;
        private readonly IBackuper backuper;
        private LockedFile lockedFile;
        private LockedFile lockedSoftDelete;
        private LockedFile lockedTempFile;
        private bool fileExistedBefore;

        public SaveTransaction(FileInfo file, FileInfo tempFile, object contents, IBackuper backuper)
        {
            this.file = file;
            this.tempFile = tempFile;
            this.contents = contents;
            this.backuper = backuper ?? NullBackuper.Default;
        }

        public void Commit<TSetting>(Serialize<TSetting> serialize, TSetting setting)
        {
            if (!this.BeforeCopy())
            {
                return;
            }

            serialize.ToStream(this.contents, this.lockedTempFile.Stream, setting);
            this.AfterCopy();
        }

        public async Task CommitAsync()
        {
            if (!this.BeforeCopy())
            {
                return;
            }

            await ((Stream)this.contents).CopyToAsync(this.lockedTempFile.Stream)
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
            this.file.Refresh();
            this.fileExistedBefore = this.file.Exists;
            this.file.Directory.CreateIfNotExists();
            this.lockedFile = LockedFile.Create(this.file, x => x.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete));
            this.lockedSoftDelete = LockedFile.Create(this.file.GetSoftDeleteFileFor(), x => x.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete));
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
            if (this.fileExistedBefore)
            {
                this.backuper.Backup(this.lockedFile);
            }

            try
            {
                this.lockedFile.DisposeAndDeleteFile();
                this.tempFile.MoveTo(this.file);
                this.lockedTempFile.DisposeAndDeleteFile();
                this.lockedSoftDelete.DisposeAndDeleteFile();
            }
            catch (Exception exception)
            {
                try
                {
                    if (!this.fileExistedBefore)
                    {
                        this.lockedFile.DisposeAndDeleteFile();
                    }

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
