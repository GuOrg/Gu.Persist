namespace Gu.Settings.RuntimeXml
{
    public class XmlEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public static readonly XmlEqualsComparer<T> Default = new XmlEqualsComparer<T>();

        protected override byte[] GetBytes(T item)
        {
            using (var stream = XmlHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
