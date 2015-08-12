namespace Gu.Settings.RuntimeXml
{
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;

    using Gu.Settings.IO;

    /// <summary>
    /// A repository reading and saving files using <see cref="DataContractSerializer"/>
    /// </summary>
    public class XmlRepository : Repository
    {
        /// <inheritdoc/>
        public XmlRepository()
            : base(Directories.Default)
        {
        }

        /// <inheritdoc/>
        public XmlRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <inheritdoc/>
        public XmlRepository(RepositorySettings settings)
            : base(settings)
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
