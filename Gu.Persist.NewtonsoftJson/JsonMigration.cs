namespace Gu.Persist.NewtonsoftJson
{
    using System;
    using System.Collections.Generic;
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

        public JsonMigration(IReadOnlyList<Func<JObject, JObject>> steps)
        {
            this.steps = steps;
        }

        /// <inheritdoc/>
        public override bool TryUpdate(Stream stream, out Stream updated)
        {
            using (var reader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true)))
            {
                var jObject = JObject.Load(reader);
                var updatedJObject = jObject;
                foreach (var step in this.steps)
                {
                    updatedJObject = step(updatedJObject);
                }

                if (jObject == updatedJObject)
                {
                    updated = null;
                    return false;
                }

                updated = PooledMemoryStream.Borrow();
                using (var jsonWriter = new JsonTextWriter(new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true)))
                {
                    jObject.WriteTo(jsonWriter);
                }

                updated.Position = 0;
                return true;
            }
        }
    }
}