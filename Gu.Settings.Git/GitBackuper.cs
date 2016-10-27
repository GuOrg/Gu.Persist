namespace Gu.Settings.Git
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    using LibGit2Sharp;

    /// <summary>
    /// An implementation of <see cref="IBackuper"/> that uses a git repository for backups.
    /// </summary>
    public class GitBackuper : IBackuper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitBackuper"/> class.
        /// Creates a git repository in <paramref name="directory"/>
        /// </summary>
        public GitBackuper(PathAndSpecialFolder directory)
        {
            this.Directory = directory;
            var directoryInfo = directory.CreateDirectoryInfo();
            Git.InitRepository(directoryInfo);
        }

        /// <summary>
        /// The director wwhere the repostory is.
        /// </summary>
        public PathAndSpecialFolder Directory { get; }

        /// <inheritdoc/>
        public bool BeforeSave(FileInfo file)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Backup(FileInfo file, FileInfo backup)
        {
            Git.StageAndCommit(file);
        }

        /// <inheritdoc/>
        public bool CanRestore(FileInfo file)
        {
            var status = Git.GetStatus(file);
            switch (status)
            {
                case FileStatus.NewInIndex:
                case FileStatus.ModifiedInIndex:
                case FileStatus.DeletedFromIndex:
                case FileStatus.RenamedInIndex:
                case FileStatus.TypeChangeInIndex:
                case FileStatus.ModifiedInWorkdir:
                case FileStatus.TypeChangeInWorkdir:
                case FileStatus.RenamedInWorkdir:
                    return true;
                case FileStatus.Unaltered:
                case FileStatus.Nonexistent:
                case FileStatus.NewInWorkdir:
                case FileStatus.DeletedFromWorkdir:
                case FileStatus.Unreadable:
                case FileStatus.Ignored:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc/>
        public bool TryRestore(FileInfo file)
        {
            var canRestore = this.CanRestore(file);
            if (canRestore)
            {
                Git.Revert(file);
            }

            return canRestore;
        }

        /// <inheritdoc/>
        public void AfterSuccessfulSave(FileInfo file)
        {
            Git.StageAndCommit(file);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            return true;
        }

        /// <inheritdoc/>
        void IBackuper.Rename(FileInfo file, string newName, bool owerWrite)
        {
            // nop
        }

        /// <inheritdoc/>
        void IBackuper.DeleteBackups(FileInfo file)
        {
            // nop
        }
    }
}
