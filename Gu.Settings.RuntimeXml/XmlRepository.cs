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
        /// <inheritdoc/>
        public XmlRepository()
            : this(Directories.Default)
        {
        }

        /// <inheritdoc/>
        public XmlRepository(DirectoryInfo directory)
            : base(directory, () => RuntimeXmlRepositorySettings.DefaultFor(directory))
        {
        }

        /// <inheritdoc/>
        public XmlRepository(DirectoryInfo directory, Func<RuntimeXmlRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        /// <inheritdoc/>
        public XmlRepository(RuntimeXmlRepositorySettings settings)
            : base(settings)
        {
        }

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
