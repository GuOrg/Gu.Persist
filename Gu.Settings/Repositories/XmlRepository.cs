namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    public class XmlRepository : Repository
    {
        public XmlRepository()
        {
        }

        public XmlRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public XmlRepository(RepositorySetting setting) 
            : base(setting)
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