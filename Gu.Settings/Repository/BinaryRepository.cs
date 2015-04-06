namespace Gu.Settings
{
    using System.IO;

    public class BinaryRepository : Repository
    {
        public BinaryRepository(RepositorySetting setting)
            : base(setting)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return BinaryHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return BinaryHelper.ToStream(item);
        }
    }
}