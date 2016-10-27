namespace Gu.Persist.Core
{
    using System.IO;

    internal static class DirectoryInfoExt
    {
        /// <summary>
        /// Create a subdirector in <paramref name="directory"/> named <paramref name="name"/>
        /// </summary>
        /// <returns>A new DirectoryInfo that is a subdirectory</returns>
        // ReSharper disable once UnusedMember.Global
        internal static DirectoryInfo Subdirectory(this DirectoryInfo directory, string name)
        {
            var path = Path.Combine(directory.FullName, name);
            return new DirectoryInfo(path);
        }

        internal static bool IsSubDirectoryOfOrSame(this DirectoryInfo directoryInfo, DirectoryInfo potentialParent)
        {
            if (DirectoryInfoComparer.Default.Equals(directoryInfo, potentialParent))
            {
                return true;
            }

            return IsStrictSubDirectoryOf(directoryInfo, potentialParent);
        }

        /// <summary>
        /// Check if <paramref name="directoryInfo"/> is a subdirectory of <paramref name="potentialParent"></paramref>
        /// </summary>
        /// <returns>True if if <paramref name="directoryInfo"/> is a subdirectory of <paramref name="potentialParent"></paramref>
        /// False if they are the same directory or otherwise</returns>
        internal static bool IsStrictSubDirectoryOf(this DirectoryInfo directoryInfo, DirectoryInfo potentialParent)
        {
            while (directoryInfo.Parent != null)
            {
                if (DirectoryInfoComparer.Default.Equals(directoryInfo.Parent, potentialParent))
                {
                    return true;
                }

                directoryInfo = directoryInfo.Parent;
            }

            return false;
        }

        /// <summary>
        /// Creates a fileinfo in <paramref name="directory"/>
        /// </summary>
        /// <returns>A FileInfo in the directory</returns>
        internal static FileInfo CreateFileInfoInDirectory(this DirectoryInfo directory, string fileName)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNullOrEmpty(fileName, nameof(fileName));

            var path = Path.Combine(directory.FullName, fileName);
            return new FileInfo(path);
        }

        /// <summary>
        /// Creates the directory if not exists
        /// </summary>
        internal static DirectoryInfo CreateIfNotExists(this DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }

            return directory;
        }
    }
}