namespace Gu.Settings.RuntimeBinary.Tests
{
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests.Repositories;

    public class BinaryRepositoryBackupInSubDirrectory : RepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = BinaryRepositorySettings.DefaultFor(Directory);
            return new BinaryRepository(settings);
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