namespace Gu.Persist.NewtonsoftJson
{
    using System.IO;
    using Gu.Persist.Core;
    using Newtonsoft.Json;

    /// <inheritdoc/>
    internal sealed class JsonSerialize : Serialize<JsonRepositorySettings>
    {
        public static readonly JsonSerialize Default = new JsonSerialize();

        /// <inheritdoc/>
        public override Stream ToStream<T>(T item)
        {
            return JsonFile.ToStream(item);
        }

        /// <inheritdoc/>
        public override void ToStream<T>(T item, Stream stream, JsonRepositorySettings settings)
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
    }
}
