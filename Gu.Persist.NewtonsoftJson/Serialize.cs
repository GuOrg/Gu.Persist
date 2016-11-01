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
        public static readonly Serialize<TSetting> Default = new Serialize<TSetting>();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item)
        {
            return JsonFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, TSetting settings)
        {
            var source = item as Stream;
            if (source != null)
            {
                source.CopyTo(stream);
                return;
            }

            var serializer = settings != null
                ? JsonSerializer.Create(settings.JsonSerializerSettings)
                : JsonSerializer.Create();
            using (var writer = new JsonTextWriter(new StreamWriter(stream, JsonFile.DefaultEncoding, 1024, true)))
            {
                serializer.Serialize(writer, item);
            }
        }

        /// <inheritdoc/>
        public override T FromStream<T>(Stream stream)
        {
            return JsonFile.FromStream<T>(stream);
        }

        /// <inheritdoc/>
        public override T Clone<T>(T item)
        {
            return JsonFile.Clone(item);
        }

        /// <inheritdoc/>
        public override IEqualityComparer<T> DefaultStructuralEqualityComparer<T>()
        {
            return JsonEqualsComparer<T>.Default;
        }
    }
}
