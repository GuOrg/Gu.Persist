namespace Gu.Settings.Json.Tests
{
    using System.IO;

    using Gu.Settings.Tests;
    using Gu.Settings.Tests.Repositories;

    using NUnit.Framework;

    public class JsonDefaultTests : RepositoryTests
    {
        protected override IRepository Create()
        {
            return new JsonRepository(Settings);
        }

        protected override RepositorySettings Settings
        {
            get { return RepositorySettings.DefaultFor(Directory); }
        }

        protected override BackupSettings BackupSettings
        {
            get { return BackupSettings.DefaultFor(Directory); }
        }

        protected override void Save<T>(T item, FileInfo file)
        {
            TestHelper.Save(item, file);
        }

        protected override T Read<T>(FileInfo file)
        {
            return TestHelper.Read<T>(file);
        }
    }
}