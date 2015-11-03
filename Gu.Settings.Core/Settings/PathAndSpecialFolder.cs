﻿namespace Gu.Settings.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    [Serializable]
    [DebuggerDisplay("Path: {Path} SpecialFolder: {SpecialFolder}")]

    public class PathAndSpecialFolder : IEquatable<PathAndSpecialFolder>
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
        public static readonly PathAndSpecialFolder DefaultBackup = new PathAndSpecialFolder($@"{Directories.AppDirectory().Name}\Backup", Environment.SpecialFolder.ApplicationData);

        public PathAndSpecialFolder(string path, Environment.SpecialFolder? specialFolder)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Trim(BackSlash);
                if (System.IO.Path.IsPathRooted(path) || path.StartsWith("."))
                {
                    var directoryInfo = new DirectoryInfo(path);
                    if (IsSubDirectoryOfOrSame(directoryInfo.FullName, Environment.CurrentDirectory))
                    {
                        Path = $".{directoryInfo.FullName.Substring(Environment.CurrentDirectory.Length)}";
                        SpecialFolder = null;
                    }

                    if (Path == null)
                    {
                        foreach (var folder in SpecialFolders)
                        {
                            var folderPath = Environment.GetFolderPath(folder);
                            if (IsSubDirectoryOfOrSame(directoryInfo.FullName, folderPath))
                            {
                                Path = directoryInfo.FullName.Substring(folderPath.Length).Trim(BackSlash);
                                SpecialFolder = folder;
                                break;
                            }
                        }
                    }

                    if (Path == null)
                    {
                        Path = directoryInfo.FullName;
                        SpecialFolder = null;
                    }
                }
                else
                {
                    Path = path;
                    if (specialFolder == null)
                    {
                        throw new ArgumentNullException(nameof(specialFolder));
                    }
                    SpecialFolder = specialFolder;
                }
            }
            else
            {
                if (specialFolder == null)
                {
                    throw new ArgumentNullException(nameof(specialFolder));
                }
                SpecialFolder = specialFolder;
            }

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
            Ensure.NotNull(info, nameof(info));
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

        public DirectoryInfo CreateDirectoryInfo()
        {
            if (SpecialFolder == null)
            {
                return new DirectoryInfo(Path);
            }

            var specialFolderPath = Environment.GetFolderPath(SpecialFolder.Value);
            var path = System.IO.Path.Combine(specialFolderPath, Path);
            return new DirectoryInfo(path);
        }

        public static bool operator ==(PathAndSpecialFolder left, PathAndSpecialFolder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PathAndSpecialFolder left, PathAndSpecialFolder right)
        {
            return !Equals(left, right);
        }

        public bool Equals(PathAndSpecialFolder other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase) && SpecialFolder == other.SpecialFolder;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((PathAndSpecialFolder)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null
                             ? StringComparer.OrdinalIgnoreCase.GetHashCode(Path)
                             : 0) * 397) ^ SpecialFolder.GetHashCode();
            }
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

        private static bool IsSubDirectoryOfOrSame(string directory, string potentialParent)
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