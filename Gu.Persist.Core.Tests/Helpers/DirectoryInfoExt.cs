namespace Gu.Persist.Core.Tests
{
    using System.IO;

    public static class DirectoryInfoExt
    {
        public static void DeleteIfExists(this DirectoryInfo directory, bool recursive)
        {
            if (Directory.Exists(directory.FullName))
            {
                directory.Delete(recursive);
            }
        }
    }
}