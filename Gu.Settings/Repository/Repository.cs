namespace Gu.Settings
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;

    public abstract class Repository : IRepository
    {
        private readonly ConcurrentDictionary<string, FileInfos> _files = new ConcurrentDictionary<string, FileInfos>();

        protected Repository(RepositorySetting setting)
        {
            Setting = setting;
            FileHelper.CreateDirectoryIfNotExists(setting.Directory);
        }

        public RepositorySetting Setting { get; private set; }

        /// <summary>
        /// Reads from file the first time. After that it returns returns cached value (singleton).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Optional if blank a file with the name of the class is read.</param>
        /// <returns></returns>
        public T Read<T>(string fileName = null)
        {
            var fileInfos = GetFileInfos<T>(fileName);
            if (fileInfos.Value == null)
            {
                var value = FileHelper.Read(fileInfos.File, FromStream<T>);
                fileInfos.Value = value;
            }
            return (T)fileInfos.Value;
        }

        public void Save<T>(T setting, bool createBackup, string fileName = null)
        {
            var fileInfos = GetFileInfos<T>(fileName);
            if (fileInfos.Value != null && !ReferenceEquals(setting, fileInfos.Value))
            {
                throw new InvalidOperationException("");
            }
            if (Setting.CreateBackupOnSave)
            {
                FileHelper.Backup(fileInfos);
            }
            try
            {
                using (var stream = ToStream(setting))
                {
                    FileHelper.Save(fileInfos.File, stream);
                }
            }
            catch (Exception)
            {
                if (Setting.CreateBackupOnSave)
                {
                    FileHelper.Restore(fileInfos);
                }
                throw;
            }
        }

        protected abstract T FromStream<T>(Stream stream);
        
        protected abstract Stream ToStream<T>(T item);

        private FileInfos GetFileInfos<T>(string fileName)
        {
            var file = fileName ?? typeof(T).FullName;
            var infos = _files.GetOrAdd(file, x => FileInfos.CreateFileInfos(Setting.Directory, fileName, Setting.CreateBackupOnSave, Setting.Extension, Setting.Extension));
            return infos;
        }
    }
}
