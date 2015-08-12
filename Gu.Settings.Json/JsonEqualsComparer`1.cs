namespace Gu.Settings.Json
{
    using Gu.Settings.Core;

    public class JsonEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public static readonly JsonEqualsComparer<T> Default = new JsonEqualsComparer<T>();

        protected override byte[] GetBytes(T item)
        {
            using (var stream = JsonHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
