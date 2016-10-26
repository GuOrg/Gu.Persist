namespace Gu.Settings.NewtonsoftJson
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gu.Settings.Core;

    using Newtonsoft.Json;

    /// <summary>
    /// A repository reading and saving files using <see cref="JsonSerializer"/>
    /// </summary>
    public class JsonRepository : Repository<JsonRepositorySettings>
    {
        /// <inheritdoc/>
        public JsonRepository()
            : this(Directories.Default)
        {
        }

        /// <inheritdoc/>
        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default, () => JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings))
        {
        }

        /// <inheritdoc/>
        public JsonRepository(DirectoryInfo directory)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory))
        {
        }

        /// <inheritdoc/>
        public JsonRepository(DirectoryInfo directory, Func<JsonRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        /// <inheritdoc/>
        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory, jsonSettings))
        {
        }

        /// <inheritdoc/>
        public JsonRepository(JsonRepositorySettings settings)
            : base(settings)
        {
        }

        public JsonRepository(JsonRepositorySettings settings, IBackuper backuper)
            : base(settings, backuper)
        {
        }

        public new JsonRepositorySettings Settings => (JsonRepositorySettings)base.Settings;

        /// <inheritdoc/>
        protected override T FromStream<T>(Stream stream)
        {
            return JsonHelper.FromStream<T>(stream, this.Settings.JsonSerializerSettings);
        }

        /// <inheritdoc/>
        protected override Stream ToStream<T>(T item)
        {
            return JsonHelper.ToStream(item, this.Settings.JsonSerializerSettings);
        }

        /// <inheritdoc/>
        protected override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return JsonEqualsComparer<T>.Default;
        }
    }
}
