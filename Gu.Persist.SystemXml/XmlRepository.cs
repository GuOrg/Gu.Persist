#pragma warning disable 1573
namespace Gu.Persist.SystemXml
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Persist.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Xml.Serialization.XmlSerializer"/>
    /// </summary>
    public class XmlRepository : Repository<XmlRepositorySettings>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public XmlRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public XmlRepository(PathAndSpecialFolder directory)
            : this(directory.CreateDirectoryInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// It will use XmlRepositorySettings.DefaultFor(directory) as settings.
        /// </summary>
        public XmlRepository(DirectoryInfo directory)
            : base(directory, () => XmlRepositorySettings.DefaultFor(directory), XmlSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(PathAndSpecialFolder directory, Func<XmlRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), settingsCreator, XmlSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(DirectoryInfo directory, Func<XmlRepositorySettings> settingsCreator)
            : base(directory, settingsCreator, XmlSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(PathAndSpecialFolder directory, IBackuper backuper, Func<XmlRepositorySettings> settingsCreator)
            : base(directory.CreateDirectoryInfo(), backuper, settingsCreator, XmlSerialize.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="directory">The directory where files will be saved.</param>
        /// <param name="backuper">
        /// The backuper.
        /// Note that a custom backuper may not use the backupsettings.
        /// </param>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(DirectoryInfo directory, IBackuper backuper, Func<XmlRepositorySettings> settingsCreator)
            : base(directory, backuper, settingsCreator, XmlSerialize.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="XmlRepository"/> class.</summary>
        public XmlRepository(XmlRepositorySettings settings)
            : base(settings, XmlSerialize.Default)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="XmlRepository"/> class.</summary>
        public XmlRepository(XmlRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper, XmlSerialize.Default)
        {
        }
    }
}