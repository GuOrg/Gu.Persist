// ReSharper disable UnusedMember.Global
namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.NewtonsoftJson;

    using RepositorySettings = Gu.Persist.Core.RepositorySettings;

    public class BackupInSubDirrectory : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var backupSettings = new BackupSettings(new PathAndSpecialFolder(this.Directory.FullName + "\\Backup", null), ".bak", BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var settings = new RepositorySettings(PathAndSpecialFolder.Create(this.Directory), false, backupSettings);
            return new SingletonRepository(settings);
        }
    }
}