namespace Gu.Settings.SystemXml.Tests
{
    using System.IO;
    using NUnit.Framework;

    public class SettingsTests
    {
        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupSettings = new BackupSettings(new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name + @"\Backup"));
            var  directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            var settings = new RepositorySettings(directory, true, true, backupSettings, ".cfg", ".tmp");
            var repository = new XmlRepository(settings);
            repository.Save(settings);
            var repositorySettings = repository.Read<RepositorySettings>();
        }
    }
}
