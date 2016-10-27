namespace Gu.Settings.SystemXml
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

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

        public XmlRepository(DirectoryInfo directory)
            : base(directory, () => XmlRepositorySettings.DefaultFor(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(DirectoryInfo directory, Func<XmlRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        public XmlRepository(XmlRepositorySettings settings)
            : base(settings)
        {
        }

        public XmlRepository(XmlRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return XmlHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return XmlHelper.ToStream(item);
        }

        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return XmlEqualsComparer<T>.Default;
        }
    }
}