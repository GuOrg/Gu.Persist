// ReSharper disable UnusedMember.Global
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;

    public class BackupInSubDirrectory : BinaryRepositoryTests
    {
        protected override IRepository Create()
        {
            var backupSettings = new BackupSettings(this.TargetDirectory.FullName + "\\Backup", ".bak", BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var settings = new RepositorySettings(this.TargetDirectory.FullName, false, backupSettings);
            return new SingletonRepository(settings);
        }
    }
}