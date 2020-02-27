#pragma warning disable SA1600 // Elements must be documented
namespace Gu.Persist.NewtonsoftJson
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    /// <inheritdoc/>
    internal sealed class Serialize<TSetting> : Gu.Persist.Core.Serialize<TSetting>
        where TSetting : Core.RepositorySettings, IJsonRepositorySetting
    {
        internal static readonly Serialize<TSetting> Default = new Serialize<TSetting>();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item, TSetting setting)
        {
            return JsonFile.ToStream(item, setting.JsonSerializerSettings);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, TSetting settings)
        {
            if (item is Stream source)
            {
                source.CopyTo(stream);
                return;
            }

            var serializer = settings != null
                ? JsonSerializer.Create(settings.JsonSerializerSettings)
                : JsonSerializer.Create();
            using var streamWriter = new StreamWriter(stream, JsonFile.DefaultEncoding, bufferSize: 1024, leaveOpen: true);
            using var writer = new JsonTextWriter(streamWriter);
            serializer.Serialize(writer, item);
        }

        /// <inheritdoc/>
        public override T FromStream<T>(Stream stream, TSetting setting)
        {
            return JsonFile.FromStream<T>(stream, setting.JsonSerializerSettings);
        }

        /// <inheritdoc/>
        public override T Clone<T>(T item, TSetting setting)
        {
            return JsonFile.Clone(item, setting.JsonSerializerSettings);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>(TSetting setting)
        {
            return setting.JsonSerializerSettings is null
                       ? JsonEqualsComparer<T>.Default
                       : new JsonEqualsComparer<T>(setting.JsonSerializerSettings);
        }
    }
}
