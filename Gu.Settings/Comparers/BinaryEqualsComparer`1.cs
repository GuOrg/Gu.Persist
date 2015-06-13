namespace Gu.Settings
{
    public class BinaryEqualsComparer<T> : SerializedEqualsComparer<T>
    {
        public static readonly BinaryEqualsComparer<T> Default = new BinaryEqualsComparer<T>();

        protected override byte[] GetBytes(T item)
        {
            using (var stream = BinaryHelper.ToStream(item))
            {
                return stream.ToArray();
            }
        }
    }
}
