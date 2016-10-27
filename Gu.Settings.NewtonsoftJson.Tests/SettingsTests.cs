namespace Gu.Settings.NewtonsoftJson.Tests
{
    using System.IO;
    using Core;
    using NUnit.Framework;

    public class SettingsTests
    {
        [Test]
        public void RoundtripRepositorySettings()
        {
            var backupDir = new DirectoryInfo($@"C:\Temp\Gu.Settings\{this.GetType().Name}\Backup");
            var backupSettings = new BackupSettings(backupDir);
            var directory = new DirectoryInfo($@"C:\Temp\Gu.Settings\{this.GetType().Name}");
            var settings = new JsonRepositorySettings(
                               directory,
                               JsonRepositorySettings.DefaultJsonSettings,
                               true,
                               true,
                               backupSettings,
                               ".cfg",
                               ".tmp");
            var repository = new JsonRepository(settings);
            repository.Save(settings);
            var repositorySettings = repository.Read<JsonRepositorySettings>();
            Assert.IsTrue(JsonEqualsComparer<JsonRepositorySettings>.Default.Equals(settings, repositorySettings));
        }
    }
}
