namespace Gu.Persist.NewtonsoftJson
{
    using Gu.Persist.Core;

    using Newtonsoft.Json;

    /// <inheritdoc/>
    public sealed class JsonEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        /// <summary>
        /// Returns the default instance.
        /// </summary>
        public static new readonly JsonEqualsComparer<T> Default = new JsonEqualsComparer<T>(null);

        private readonly JsonSerializerSettings? settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEqualsComparer{T}"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/>.</param>
        public JsonEqualsComparer(JsonSerializerSettings? settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        protected override IPooledStream GetStream(T item)
        {
            return JsonFile.ToStream(item, this.settings);
        }
    }
}
