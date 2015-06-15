namespace Gu.Settings.Json
{
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.IO;

    public class JsonRepository : Repository
    {
        public JsonRepository()
            : base(Directories.Default)
        {
        }

        public JsonRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public JsonRepository(RepositorySettings settings)
            : base(settings)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return JsonHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return JsonHelper.ToStream(item);
        }

        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return JsonEqualsComparer<T>.Default;
        }
    }
}
