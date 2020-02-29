namespace Gu.Persist.NewtonsoftJson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using Gu.Persist.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// For example when updating between versions.
    /// </summary>
    public class JsonMigration : Migration
    {
        private readonly IReadOnlyList<Func<JObject, JObject>> steps;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMigration"/> class.
        /// </summary>
        /// <param name="steps">A sequence of transformations of the <see cref="JObject"/> read from the stream.</param>
        public JsonMigration(params Func<JObject, JObject>[] steps)
        {
            this.steps = steps;
        }

        /// <inheritdoc/>
        public override bool TryUpdate(Stream stream, [NotNullWhen(true)] out Stream? updated)
        {
            var jObject = Load();
            var hash = JsonEqualsComparer<JObject>.Default.GetHashCode(jObject);
            var updatedJObject = jObject;
            foreach (var step in this.steps)
            {
                updatedJObject = step(updatedJObject);
            }

            if (ReferenceEquals(jObject, updatedJObject) &&
                hash == JsonEqualsComparer<JObject>.Default.GetHashCode(updatedJObject))
            {
                updated = null;
                return false;
            }

            updated = PooledMemoryStream.Borrow();
            using (var streamWriter = new StreamWriter(updated, Encoding.UTF8, 1024, leaveOpen: true))
            {
                using var jsonWriter = new JsonTextWriter(streamWriter);
                jObject.WriteTo(jsonWriter);
            }

            updated.Position = 0;
            return true;

            JObject Load()
            {
                using var streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true);
                using var reader = new JsonTextReader(streamReader);
                return JObject.Load(reader);
            }
        }
    }
}