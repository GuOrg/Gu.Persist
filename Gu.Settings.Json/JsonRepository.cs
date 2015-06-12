namespace Gu.Settings.Json
{
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    public class JsonRepository : Repository
    {
        public JsonRepository(DirectoryInfo directory)
            : base(directory)
        {
        }

        public JsonRepository(IRepositorySetting setting)
            : base(setting)
        {
        }

        protected override T FromStream<T>(Stream stream)
        {
            return JsonHelper.FromStream<T>(stream);
        }

        protected override Stream ToStream<T>(T item)
        {
            return JsonHelper.ToStream(item);
        }
    }
}
