namespace Gu.Settings.Core
{
    using System.IO;

    public static class DirectoryInfoExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="name"></param>
        /// <returns>A new DirectoryInfo that is a subdirectory</returns>
        public static DirectoryInfo Subdirectory(this DirectoryInfo directory, string name)
        {
            var path = Path.Combine(directory.FullName, name);
            return new DirectoryInfo(path);
        }

        public static bool IsSubDirectoryOfOrSame(this DirectoryInfo directoryInfo, DirectoryInfo potentialParent)
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
        /// <param name="directoryInfo"></param>
        /// <param name="potentialParent"></param>
        /// <returns>True if if <paramref name="directoryInfo"/> is a subdirectory of <paramref name="potentialParent"></paramref>
        /// False if they are the same directory or otherwise</returns>
        public static bool IsStrictSubDirectoryOf(this DirectoryInfo directoryInfo, DirectoryInfo potentialParent)
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
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <returns>A FileInfo in the driectory</returns>
        public static FileInfo CreateFileInfoInDirectory(this DirectoryInfo directory, string fileName)
        {
            Ensure.NotNull(directory, nameof(directory));
            Ensure.NotNullOrEmpty(fileName, nameof(fileName));

            var path = Path.Combine(directory.FullName, fileName);
            return new FileInfo(path);
        }

        /// <summary>
        /// Creates the directory if not exists
        /// </summary>
        /// <param name="directory"></param>
        internal static void CreateIfNotExists(this DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
        }
    }
}