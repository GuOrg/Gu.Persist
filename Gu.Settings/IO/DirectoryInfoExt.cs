namespace Gu.Settings
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="fileName"></param>
        /// <returns>A FileInfo in the driectory</returns>
        public static FileInfo CreateFileInfoInDirectory(this DirectoryInfo directory, string fileName)
        {
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