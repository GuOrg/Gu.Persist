namespace Gu.Settings.Git
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    using LibGit2Sharp;

    public class GitBackuper : IBackuper
    {
        private readonly PathAndSpecialFolder _directory;

        public GitBackuper(PathAndSpecialFolder directory)
        {
            _directory = directory;
            var directoryInfo = directory.CreateDirectoryInfo();
            Git.InitRepository(directoryInfo);
        }

        public bool TryBackup(FileInfo file)
        {
            Git.Stage(file);
            return Git.Commit(file);
        }

        public void Backup(FileInfo file, FileInfo backup)
        {
            Git.Stage(file);
            Git.Commit(file);
        }

        public bool CanRestore(FileInfo file)
        {
            var status = Git.GetStatus(file);
            switch (status)
            {
                case FileStatus.Nonexistent:
                case FileStatus.Unaltered:
                case FileStatus.Added:
                case FileStatus.Staged:
                case FileStatus.Removed:
                case FileStatus.RenamedInIndex:
                case FileStatus.StagedTypeChange:
                case FileStatus.Modified:
                case FileStatus.TypeChanged:
                case FileStatus.RenamedInWorkDir:
                    return true;
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
            var canRestore = CanRestore(file);
            Git.Revert(file);
            return canRestore;
        }

        public void PurgeBackups(FileInfo file)
        {
            // nop
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
