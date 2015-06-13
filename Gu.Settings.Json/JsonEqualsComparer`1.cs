namespace Gu.Settings.Json
{
    public class JsonEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public static readonly JsonEqualsComparer<T> Default = new JsonEqualsComparer<T>();

        protected override byte[] GetBytes(T item)
        {
            using (var stream = XmlHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
