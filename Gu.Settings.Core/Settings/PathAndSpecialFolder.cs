namespace Gu.Settings.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    [Serializable]
    [DebuggerDisplay("Path: {Path} SpecialFolder: {SpecialFolder}")]

    public class PathAndSpecialFolder
    {
        private static readonly char[] BackSlash = { '\\' };
        private static readonly Environment.SpecialFolder[] SpecialFolders =
            {
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolder.LocalApplicationData,
                Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolder.MyMusic,
                Environment.SpecialFolder.MyPictures,
                Environment.SpecialFolder.MyVideos,
                Environment.SpecialFolder.CommonDocuments,
                Environment.SpecialFolder.CommonMusic,
                Environment.SpecialFolder.CommonPictures,
                Environment.SpecialFolder.CommonVideos,
                Environment.SpecialFolder.DesktopDirectory,
            };

        public static readonly PathAndSpecialFolder Default = new PathAndSpecialFolder(Directories.AppDirectory().Name, Environment.SpecialFolder.ApplicationData);

        public PathAndSpecialFolder(string path, Environment.SpecialFolder? specialFolder)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim(BackSlash);
                if (System.IO.Path.IsPathRooted(path))
                {

                }
                else if (path.StartsWith("."))
                {
                    if (specialFolder != null)
                    {
                        
                    }
                }
            }

            if (string.Equals(path, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), StringComparison.OrdinalIgnoreCase) ||
                specialFolder == Environment.SpecialFolder.ApplicationData && string.IsNullOrWhiteSpace(path))
            {

            }

            //Path = path.TrimStart(Environment.CurrentDirectory)
            //           .TrimStart(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            //           .TrimStart(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
            //           .Trim(BackSlash);
            //SpecialFolder = specialFolder ?? IsSubDirectoryOfOrSame();

            if (SpecialFolder == Environment.SpecialFolder.ApplicationData ||
                SpecialFolder == Environment.SpecialFolder.CommonApplicationData ||
                SpecialFolder == Environment.SpecialFolder.LocalApplicationData)
            {
                if (string.IsNullOrEmpty(Path))
                {
                    throw new ArgumentException($"Not allowed to save in {SpecialFolder} without subdirectory");
                }
            }

            else if (!string.IsNullOrEmpty(path) && path.StartsWith("."))
            {
                if (SpecialFolder != null)
                {
                    string message = $"Special folder must be null when path starts with '.'";
                    throw new ArgumentException(message);
                }
            }
        }

        public string Path { get; }

        public Environment.SpecialFolder? SpecialFolder { get; }

        public static PathAndSpecialFolder Create(DirectoryInfo info)
        {      
            return new PathAndSpecialFolder(info.FullName, null);
        }

        public static PathAndSpecialFolder Create(string path)
        {
            Ensure.NotNullOrEmpty(path, nameof(path));
            return new PathAndSpecialFolder(path, null);
        }

        public static PathAndSpecialFolder DefaultFor(Assembly assembly)
        {
            Ensure.NotNull(assembly, nameof(assembly));
            var name = assembly.GetName().Name;
            return new PathAndSpecialFolder(name, Environment.SpecialFolder.ApplicationData);
        }

        public bool CanCreateDirectoryInfo()
        {
            if (SpecialFolder != null)
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

        internal DirectoryInfo CreateDirectoryInfo()
        {
            var specialFolder = SpecialFolder;
            if (specialFolder != null)
            {
                var specialFolderPath = Environment.GetFolderPath(specialFolder.Value);
                var path = System.IO.Path.Combine(specialFolderPath, Path);
                return new DirectoryInfo(path);
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
            if (l1 > l2)
            {
                if (directory[l2] != '\\')
                {
                    return false;
                }
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

        private static string GetDefaultName()
        {
            return Directories.AppDirectory()?.Name;
        }
    }
}