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
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// Uses <see cref="Directories.Default"/>
        /// </summary>
        public JsonRepository()
            : this(Directories.Default)
        {
        }

        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default, () => JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings))
        {
        }

        public JsonRepository(DirectoryInfo directory)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory))
        {
        }

        public JsonRepository(DirectoryInfo directory, Func<JsonRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
        {
        }

        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory, jsonSettings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// </summary>
        public JsonRepository(JsonRepositorySettings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// </summary>
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
