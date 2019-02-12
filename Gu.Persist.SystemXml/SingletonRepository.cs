#pragma warning disable 1573
namespace Gu.Persist.SystemXml
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Xml.Serialization.XmlSerializer"/>
    /// This repository keeps a cache of all saves and reads an manages singleton instances.
    /// </summary>
    public class SingletonRepository : SingletonRepository<RepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// Uses <see cref="Directories.Default"/>.
        /// </summary>
        public SingletonRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public SingletonRepository(DirectoryInfo directory)
            : base(() => CreateDefaultSettings(directory), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        public SingletonRepository(Func<RepositorySettings> settingsCreator)
            : base(settingsCreator, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonRepository"/> class.
        /// If the directory contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        public SingletonRepository(Func<RepositorySettings> settingsCreator, IBackuper backuper)
            : base(settingsCreator, backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SingletonRepository"/> class.</summary>
        public SingletonRepository(RepositorySettings settings)
            : base(settings, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SingletonRepository"/> class.</summary>
        public SingletonRepository(RepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SingletonRepository"/> class.</summary>
        public SingletonRepository(IRepositorySettings settings)
            : base(CreateSettings(settings), Serialize<RepositorySettings>.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="SingletonRepository"/> class.</summary>
        public SingletonRepository(IRepositorySettings settings, IBackuper backuper)
            : base(CreateSettings(settings), backuper, Serialize<RepositorySettings>.Default)
        {
        }

        private static RepositorySettings CreateDefaultSettings(DirectoryInfo directory)
        {
            return CreateSettings(Default.RepositorySettings(directory));
        }

        private static RepositorySettings CreateSettings(IRepositorySettings settings)
        {
            return new RepositorySettings(
                       settings.Directory,
                       settings.IsTrackingDirty,
                       settings.BackupSettings,
                       settings.Extension,
                       settings.TempExtension);
        }
    }
}