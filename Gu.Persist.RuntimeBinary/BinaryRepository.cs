#pragma warning disable 1573
namespace Gu.Persist.RuntimeBinary
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>
    /// </summary>
    public class BinaryRepository : Repository<BinaryRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public BinaryRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public BinaryRepository(PathAndSpecialFolder directory)
            : this(directory.CreateDirectoryInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// It will use BinaryRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public BinaryRepository(DirectoryInfo directory)
            : base(directory, () => BinaryRepositorySettings.DefaultFor(directory), BinarySerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public BinaryRepository(PathAndSpecialFolder directory, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), settingsCreator, BinarySerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public BinaryRepository(DirectoryInfo directory, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory, settingsCreator, BinarySerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public BinaryRepository(PathAndSpecialFolder directory, IBackuper backuper, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), backuper, settingsCreator, BinarySerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public BinaryRepository(DirectoryInfo directory, IBackuper backuper, Func<BinaryRepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, BinarySerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryRepository"/> class.
        /// </summary>
        public BinaryRepository(BinaryRepositorySettings settings)
            : base(settings, BinarySerialize.Default)
        {
        }
    }
}