namespace Gu.Settings.SystemXml
{
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.IO;

    public class XmlRepository : Repository
    {
        public XmlRepository()
            : base(Directories.Default)
        {
        }

        public XmlRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public XmlRepository(RepositorySettings settings) 
            : base(settings)
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