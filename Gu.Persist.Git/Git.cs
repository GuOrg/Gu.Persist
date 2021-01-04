namespace Gu.Persist.Git
{
    using System;
    using System.IO;

    using LibGit2Sharp;

    internal static class Git
    {
        private static readonly CommitOptions AllowEmptyCommit = new CommitOptions { AllowEmptyCommit = false };
        private static readonly CommitOptions NonEmptyCommitOnly = new CommitOptions { AllowEmptyCommit = false };
        private static readonly CheckoutOptions ForceCheckoutOptions = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };
        private static readonly Signature Signature = new Signature(new Identity("Gu.Persist.Git", "Gu.Persist.Git@github.com"), DateTimeOffset.UtcNow);
        private static readonly StageOptions StageOptionsIncludeIgnored = new StageOptions { IncludeIgnored = true };

        internal static void InitRepository(string directory)
        {
            if (!Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
                _ = Repository.Init(directory);
                return;
            }

            if (!Repository.IsValid(directory))
            {
                _ = Repository.Init(directory);
            }
        }

        internal static FileStatus GetStatus(FileInfo file)
        {
            using var repository = new Repository(file.DirectoryName);
            Commands.Stage(repository, file.FullName);
            var status = repository.RetrieveStatus(file.FullName);
            Commands.Unstage(repository, file.FullName);
            return status;
        }

        internal static bool StageAndCommit(FileInfo file, bool allowEmptyCommit)
        {
            if (!File.Exists(file.FullName))
            {
                return false;
            }

            using var repository = new Repository(file.DirectoryName);
            Commands.Stage(repository, file.FullName, StageOptionsIncludeIgnored);
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
                    var commitOptions = allowEmptyCommit ? AllowEmptyCommit : NonEmptyCommitOnly;
                    repository.Commit($"Create backup for {file.Name}", Signature, Signature, commitOptions);
                    return true;
                case FileStatus.Unreadable:
                case FileStatus.Ignored:
                    throw new InvalidOperationException($"FileStatus: {status}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(file), status, "Unknown status.");
            }
        }

        internal static void Revert(FileInfo file)
        {
            using var repository = new Repository(file.DirectoryName);
            repository.CheckoutPaths(repository.Head.FriendlyName, new[] { file.FullName }, ForceCheckoutOptions);
        }
    }
}