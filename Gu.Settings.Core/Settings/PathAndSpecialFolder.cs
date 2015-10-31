namespace Gu.Settings.Core
{
    using System;
    using System.IO;
    using System.Reflection;

    public class PathAndSpecialFolder
    {
        public static readonly PathAndSpecialFolder Default = new PathAndSpecialFolder(Assembly.GetExecutingAssembly().GetName().Name, Environment.SpecialFolder.ApplicationData);

        public PathAndSpecialFolder(string path, Environment.SpecialFolder specialFolder)
        {
            Path = path;
            Environment.SpecialFolder = specialFolder;
        }

        internal PathAndSpecialFolder(string path, Environment.SpecialFolder? specialFolder)
        {
            Path = path;
            SpecialFolder = specialFolder;
        }

        public string Path { get; }

        public Environment.SpecialFolder? SpecialFolder { get; }

        public static PathAndSpecialFolder Create(DirectoryInfo info)
        {
            var path = TryCreate(info, Environment.SpecialFolder.ApplicationData) ??
                       new PathAndSpecialFolder(info.FullName, null);
            return path;
        }

        public static PathAndSpecialFolder Create(string path)
        {
            return Create(new DirectoryInfo(path));
        }

        public bool CanCreateDirectoryInfo
        {
            get
            {
                if (Environment.SpecialFolder != null)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(Path))
                {
                    return false;
                }

                if (Path.StartsWith("."))
                {
                    return true;
                }

                return System.IO.Path.IsPathRooted(Path);
            }
        }

        internal DirectoryInfo CreateDirectoryInfo()
        {
            var specialFolder = Environment.SpecialFolder;
            if (specialFolder != null)
            {
                var folderPath = Environment.GetFolderPath(specialFolder.Value);
                return new DirectoryInfo(System.IO.Path.Combine(folderPath, Path));
            }
            return new DirectoryInfo(Path);
        }

        private static PathAndSpecialFolder TryCreate(DirectoryInfo info, Environment.SpecialFolder specialFolder)
        {
            var directory = Environment.GetFolderPath(specialFolder);
            if (info.IsSubDirectoryOfOrSame(new DirectoryInfo(directory)))
            {
                var relativePath = info.FullName.Substring(directory.Length);
                return new PathAndSpecialFolder(relativePath, specialFolder);
            }
            return null;
        }

        internal static bool IsSubDirectoryOfOrSame(string directory, string potentialParent)
        {
            var l1 = GetPathLength(directory);
            var l2 = GetPathLength(potentialParent);
            if (l2 > l1)
            {
                return false;
            }
            return string.Compare(directory, 0, potentialParent, 0, l2, true) == 0;
        }

        private static int GetPathLength(string directory)
        {
            if (directory[directory.Length - 1] == '\\')
            {
                return directory.Length - 1;
            }
            return directory.Length;
        }
    }
}