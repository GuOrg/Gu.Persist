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
            var backupSettings = new BackupSettings(this.TargetDirectory.FullName + "\\Backup", ".bak", BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var settings = new RepositorySettings(this.TargetDirectory.FullName, false, backupSettings);
            return new SingletonRepository(settings);
        }
    }
}