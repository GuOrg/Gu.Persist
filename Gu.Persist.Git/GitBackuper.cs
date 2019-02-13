namespace Gu.Persist.Git
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Persist.Core;

    using LibGit2Sharp;

    /// <summary>
    /// An implementation of <see cref="IBackuper"/> that uses a git repository for backups.
    /// </summary>
    public class GitBackuper : IBackuper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GitBackuper"/> class.
        /// Creates a git repository in <paramref name="directory"/>.
        /// </summary>
        public GitBackuper(string directory)
        {
            this.Directory = directory;
            var directoryInfo = new DirectoryInfo(directory);
            Git.InitRepository(directoryInfo.FullName);
        }

        /// <summary>
        /// The director where the repository is.
        /// </summary>
        public string Directory { get; }

        /// <inheritdoc/>
        public bool BeforeSave(FileInfo file)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Backup(LockedFile file)
        {
            file.Close();
            Git.StageAndCommit(file.File, allowEmptyCommit: false);
        }

        /// <inheritdoc/>
        public void Backup(FileInfo file)
        {
            Git.StageAndCommit(file, allowEmptyCommit: false);
        }

        /// <inheritdoc/>
        public void Backup(FileInfo file, FileInfo backup)
        {
            Git.StageAndCommit(file, allowEmptyCommit: false);
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
        public void AfterSave(LockedFile file)
        {
            file.Close();
            Git.StageAndCommit(file.File, allowEmptyCommit: false);
        }

        /// <inheritdoc/>
        public bool CanRename(FileInfo file, string newName)
        {
            return true;
        }

        /// <inheritdoc/>
        void IBackuper.Rename(FileInfo file, string newName, bool overWrite)
        {
            // nop
        }

        /// <inheritdoc/>
        public IReadOnlyList<RenamePair> GetRenamePairs(FileInfo file, string newName)
        {
            Ensure.NotNull(file, nameof(file));
            Ensure.NotNullOrEmpty(newName, nameof(newName));
            return RenamePair.EmptyArray;
        }

        /// <inheritdoc/>
        void IBackuper.DeleteBackups(FileInfo file)
        {
            // nop
        }
    }
}
