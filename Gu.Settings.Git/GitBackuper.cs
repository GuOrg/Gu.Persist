namespace Gu.Settings.Git
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    using LibGit2Sharp;

    public class GitBackuper : IBackuper
    {
        private readonly PathAndSpecialFolder directory;

        public GitBackuper(PathAndSpecialFolder directory)
        {
            this.directory = directory;
            var directoryInfo = directory.CreateDirectoryInfo();
            Git.InitRepository(directoryInfo);
        }

        public bool BeforeSave(FileInfo file)
        {
            return false;
        }

        public void Backup(FileInfo file, FileInfo backup)
        {
            Git.StageAndCommit(file);
        }

        public bool CanRestore(FileInfo file)
        {
            var status = Git.GetStatus(file);
            switch (status)
            {
                case FileStatus.Added:
                case FileStatus.Staged:
                case FileStatus.Removed:
                case FileStatus.RenamedInIndex:
                case FileStatus.StagedTypeChange:
                case FileStatus.Modified:
                case FileStatus.TypeChanged:
                case FileStatus.RenamedInWorkdir:
                    return true;
                case FileStatus.Unaltered:
                case FileStatus.Nonexistent:
                case FileStatus.Untracked:
                case FileStatus.Missing:
                case FileStatus.Unreadable:
                case FileStatus.Ignored:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool TryRestore(FileInfo file)
        {
            var canRestore = this.CanRestore(file);
            if (canRestore)
            {
                Git.Revert(file);
            }

            return canRestore;
        }

        public void AfterSuccessfulSave(FileInfo file)
        {
            Git.StageAndCommit(file);
        }

        public bool CanRename(FileInfo file, string newName)
        {
            return true;
        }

        public void Rename(FileInfo file, string newName, bool owerWrite)
        {
            // nop
        }

        public void DeleteBackups(FileInfo file)
        {
            // nop
        }
    }
}
