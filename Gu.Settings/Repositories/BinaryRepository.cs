namespace Gu.Settings
{
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.IO;

    public class BinaryRepository : Repository
    {
        public BinaryRepository()
            : base(Directories.Default)
        {
        }

        public BinaryRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public BinaryRepository(RepositorySettings settings)
            : base(settings)
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