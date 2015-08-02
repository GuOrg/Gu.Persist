namespace Gu.Settings.RuntimeXml.Tests
{
    using System.IO;

    using Gu.Settings.Tests.Repositories;

    public class XmlRepositoryTests : RepositoryTests
    {
        protected override RepositorySettings Settings
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                return (RepositorySettings)Repository.Settings;
            }
        }

        protected override BackupSettings BackupSettings
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                return Repository.Settings.BackupSettings;
            }
        }

        protected override IRepository Create()
        {
            return new XmlRepository();
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