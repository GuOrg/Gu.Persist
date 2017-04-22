// ReSharper disable UnusedMember.Global
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;

    public class BackupInSubDirectory : BinaryRepositoryTests
    {
        protected override IRepository Create()
        {
            var backupSettings = new BackupSettings(
                directory: this.TargetDirectory.FullName + "\\Backup",
                extension: ".bak",
                timeStampFormat: BackupSettings.DefaultTimeStampFormat,
                numberOfBackups: 1,
                maxAgeInDays: int.MaxValue);

            var settings = new RepositorySettings(
                directory: this.TargetDirectory.FullName,
                isTrackingDirty: false,
                backupSettings: backupSettings);

            return new SingletonRepository(settings);
        }
    }
}