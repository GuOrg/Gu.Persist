namespace Gu.Settings.Json
{
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// A repository reading and saving files using <see cref="JsonSerializer"/>
    /// </summary>
    public class JsonRepository : Repository
    {
        /// <inheritdoc/>
        public JsonRepository()
            : base(Directories.Default)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        /// <inheritdoc/>
        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default)
        {
            JsonSettings = jsonSettings;
        }

        /// <inheritdoc/>
        public JsonRepository(DirectoryInfo directory)
            : base(directory)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        /// <inheritdoc/>
        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory)
        {
            JsonSettings = jsonSettings;
        }

        /// <inheritdoc/>
        public JsonRepository(RepositorySettings settings)
            : base(settings)
        {
            JsonSettings = ReadOrCreateCore(() => JsonHelper.DefaultJsonSettings);
        }

        /// <inheritdoc/>
        public JsonRepository(RepositorySettings settings, JsonSerializerSettings jsonSettings)
            : base(settings)
        {
            JsonSettings = jsonSettings;
        }

        /// <inheritdoc/>
        public JsonSerializerSettings JsonSettings { get; private set; }

        /// <inheritdoc/>
        protected override T FromStream<T>(Stream stream)
        {
            return JsonHelper.FromStream<T>(stream, JsonSettings);
        }

        /// <inheritdoc/>
        protected override Stream ToStream<T>(T item)
        {
            return JsonHelper.ToStream(item, JsonSettings);
        }

        /// <inheritdoc/>
        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return JsonEqualsComparer<T>.Default;
        }
    }
}
