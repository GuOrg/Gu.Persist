namespace Gu.Settings.Git
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    using LibGit2Sharp;

    internal static class Git
    {
        private static readonly CommitOptions CommitOptions = new CommitOptions { AllowEmptyCommit = false };
        private static readonly CheckoutOptions ForceCheckoutOptions = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };

        internal static bool IsValid(DirectoryInfo directory)
        {
            return LibGit2Sharp.Repository.IsValid(directory.FullName);
        }

        internal static void InitRepository(DirectoryInfo directory)
        {
            directory.CreateIfNotExists();
            if (!LibGit2Sharp.Repository.IsValid(directory.FullName))
            {
                var init = LibGit2Sharp.Repository.Init(directory.FullName);
            }
        }

        internal static FileStatus GetStatus(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.Stage(file.FullName);
                var status = repository.RetrieveStatus(file.FullName);
                repository.Unstage(file.FullName);
                return status;
            }
        }

        internal static void Stage(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.Stage(file.FullName);
            }
        }

        internal static bool Commit(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                var status = repository.RetrieveStatus(file.FullName);

                switch (status)
                {
                    case FileStatus.Unaltered:
                        return false;
                    case FileStatus.Nonexistent:
                    case FileStatus.Added:
                    case FileStatus.Staged:
                    case FileStatus.Removed:
                    case FileStatus.RenamedInIndex:
                    case FileStatus.StagedTypeChange:
                    case FileStatus.Untracked:
                    case FileStatus.Modified:
                    case FileStatus.Missing:
                    case FileStatus.TypeChanged:
                    case FileStatus.RenamedInWorkDir:
                        {
                            var commit = repository.Commit(status.ToString(), CommitOptions);
                            return true;
                        }
                    case FileStatus.Unreadable:
                    case FileStatus.Ignored:
                        throw new InvalidOperationException($"FileStatus: {status}");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal static void Revert(FileInfo file)
        {
            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.CheckoutPaths(repository.Head.Name, new[] { file.FullName }, ForceCheckoutOptions);
            }
        }
    }
}