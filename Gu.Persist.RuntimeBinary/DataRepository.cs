namespace Gu.Persist.RuntimeBinary
{
    using System;
    using System.IO;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>
    /// </summary>
    public class DataRepository : DataRepository<DataRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public DataRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// It will use BinaryRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public DataRepository(DirectoryInfo directory)
            : base(directory, () => Default.DataRepositorySettings(directory), Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where the repository reads and saves files.</param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(DirectoryInfo directory, Func<DataRepositorySettings> settingsCreator)
            : base(directory, settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public DataRepository(DirectoryInfo directory, IBackuper backuper, Func<DataRepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, Serialize<DataRepositorySettings>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepository"/> class.
        /// </summary>
        public DataRepository(Core.DataRepositorySettings settings)
            : base(settings, Serialize<DataRepositorySettings>.Default)
        {
        }
    }
}