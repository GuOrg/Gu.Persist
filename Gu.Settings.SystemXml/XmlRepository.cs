namespace Gu.Settings.SystemXml
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    public class XmlRepository : Repository<XmlRepositorySettings>
    {
        public XmlRepository()
            : this(Directories.Default)
        {
        }

        public XmlRepository(DirectoryInfo directory)
            : base(directory, () => XmlRepositorySettings.DefaultFor(directory))
        {
        }

        /// <inheritdoc/>
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