#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Persist.Git
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    using LibGit2Sharp;

    internal static class Git
    {
        private static readonly CommitOptions CommitOptions = new CommitOptions { AllowEmptyCommit = false };
        private static readonly CheckoutOptions ForceCheckoutOptions = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };
        private static readonly Signature Signature = new Signature(new Identity("Gu.Persist.Git", "Gu.Persist.Git@github.com"), DateTimeOffset.UtcNow);

        internal static bool IsValid(DirectoryInfo directory)
        {
            return LibGit2Sharp.Repository.IsValid(directory.FullName);
        }

        internal static void InitRepository(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.CreateIfNotExists();
                Repository.Init(directory.FullName);
                return;
            }

            if (!LibGit2Sharp.Repository.IsValid(directory.FullName))
            {
                Repository.Init(directory.FullName);
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

        internal static bool StageAndCommit(FileInfo file)
        {
            if (!File.Exists(file.FullName))
            {
                return false;
            }

            using (var repository = new LibGit2Sharp.Repository(file.DirectoryName))
            {
                repository.Stage(file.FullName, new StageOptions { IncludeIgnored = true });
                var status = repository.RetrieveStatus(file.FullName);

                switch (status)
                {
                    case FileStatus.Nonexistent:
                    case FileStatus.Unaltered:
                        return false;
                    case FileStatus.NewInIndex:
                    case FileStatus.ModifiedInIndex:
                    case FileStatus.DeletedFromIndex:
                    case FileStatus.RenamedInIndex:
                    case FileStatus.TypeChangeInIndex:
                    case FileStatus.NewInWorkdir:
                    case FileStatus.ModifiedInWorkdir:
                    case FileStatus.DeletedFromWorkdir:
                    case FileStatus.TypeChangeInWorkdir:
                    case FileStatus.RenamedInWorkdir:
                        {
                            repository.Commit("Created backup", Signature, Signature, CommitOptions);
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
                repository.CheckoutPaths(repository.Head.FriendlyName, new[] { file.FullName }, ForceCheckoutOptions);
            }
        }
    }
}