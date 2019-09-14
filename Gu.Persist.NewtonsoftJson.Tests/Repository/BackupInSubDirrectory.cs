// ReSharper disable UnusedMember.Global
namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;
    using Gu.Persist.NewtonsoftJson;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class BackupInSubDirrectory : JsonRepositoryTests
    {
        protected override IRepository CreateRepository()
        {
            var backupSettings = new BackupSettings(
                directory: this.Directory.FullName + "\\Backup",
                extension: ".bak",
                timeStampFormat: BackupSettings.DefaultTimeStampFormat,
                numberOfBackups: 1,
                maxAgeInDays: int.MaxValue);

            var settings = new RepositorySettings(
                directory: this.Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: false,
                backupSettings: backupSettings);

            return new SingletonRepository(settings);
        }
    }
}