#pragma warning disable 1573
namespace Gu.Settings.RuntimeXml
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    /// <summary>
    /// A repository reading and saving files using <see cref="System.Runtime.Serialization.DataContractSerializer"/>
    /// </summary>
    public class XmlRepository : Repository<RuntimeXmlRepositorySettings>
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
        public XmlRepository(DirectoryInfo directory)
            : base(directory, () => RuntimeXmlRepositorySettings.DefaultFor(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new default setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public XmlRepository(DirectoryInfo directory, Func<RuntimeXmlRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// </summary>
        public XmlRepository(RuntimeXmlRepositorySettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository"/> class.
        /// </summary>
        public XmlRepository(RuntimeXmlRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper)
        {
        }

        /// <inheritdoc/>
        protected override T FromStream<T>(Stream stream)
        {
            return XmlHelper.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        protected override Stream ToStream<T>(T item)
        {
            return XmlHelper.ToStream(item);
        }

        /// <inheritdoc/>
        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return XmlEqualsComparer<T>.Default;
        }
    }
}
