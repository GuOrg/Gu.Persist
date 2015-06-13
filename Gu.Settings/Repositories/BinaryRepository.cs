namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    public class BinaryRepository : Repository
    {
        public BinaryRepository()
        {
        }

        public BinaryRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public BinaryRepository(RepositorySetting setting)
            : base(setting)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return BinaryHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return BinaryHelper.ToStream(item);
        }

        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return BinaryEqualsComparer<T>.Default;
        }
    }
}