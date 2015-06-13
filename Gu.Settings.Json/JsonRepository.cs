namespace Gu.Settings.Json
{
    using System.Collections.Generic;
    using System.IO;

    public class JsonRepository : Repository
    {
        public JsonRepository()
        {
        }

        public JsonRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public JsonRepository(RepositorySetting setting)
            : base(setting)
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
