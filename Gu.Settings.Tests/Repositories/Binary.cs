namespace Gu.Settings.Tests.Repositories
{
    using System.IO;

    public class Binary : RepositoryTests
    {
        protected override IRepository Create(RepositorySetting setting)
        {
            return new BinaryRepository(setting);
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryHelper.Read<T>(file);
        }
    }
}