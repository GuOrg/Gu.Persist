// ReSharper disable UnusedMember.Global
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;

    public class BackupInSubDirectory : BinaryRepositoryTests
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
                isTrackingDirty: false,
                backupSettings: backupSettings);

            return new SingletonRepository(settings);
        }
    }
}