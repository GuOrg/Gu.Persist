#pragma warning disable 1573
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
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository()
            : this(Directories.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings)) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(JsonSerializerSettings jsonSettings)
            : base(Directories.Default, () => JsonRepositorySettings.DefaultFor(Directories.Default, jsonSettings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use JsonRepositorySettings.DefaultFor(directory) as settings.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(DirectoryInfo directory)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// It will use  JsonRepositorySettings.DefaultFor(directory, jsonSettings) to create the setting if no settings file exists.
        /// This creates a setting file for the repository in the directory if it does not exist.
        /// After this the settings file will be used.
        /// </summary>
        public JsonRepository(DirectoryInfo directory, JsonSerializerSettings jsonSettings)
            : base(directory, () => JsonRepositorySettings.DefaultFor(directory, jsonSettings))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository"/> class.
        /// If <paramref name="directory"/> contains a settings file it is read and used.
        /// If not a new setting is created and saved.
        /// </summary>
        /// <param name="settingsCreator">Creates settings if file is missing</param>
        public JsonRepository(DirectoryInfo directory, Func<JsonRepositorySettings> settingsCreator)
            : base(directory, settingsCreator)
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

        /// <summary>
        /// The settings used by the repository.
        /// </summary>
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
