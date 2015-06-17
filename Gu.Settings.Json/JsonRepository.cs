namespace Gu.Settings.Json
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using Gu.Settings.IO;
    using Newtonsoft.Json;

    public class JsonRepository : Repository
    {
        public JsonRepository()
            : base(Directories.Default)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default)
        {
            JsonSettings = jsonSettings;
        }

        public JsonRepository(DirectoryInfo directory)
            : base(directory)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory)
        {
            JsonSettings = jsonSettings;
        }

        public JsonRepository(RepositorySettings settings)
            : base(settings)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        public JsonRepository(RepositorySettings settings, JsonSerializerSettings jsonSettings)
            : base(settings)
        {
            JsonSettings = jsonSettings;
        }

        public JsonSerializerSettings JsonSettings { get; private set; }

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
