// ReSharper disable UnusedMember.Global
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;

    public class BackupInSubDirrectory : BinaryRepositoryTests
    {
        protected override IRepository Create()
        {
            var backupSettings = new BackupSettings(new PathAndSpecialFolder(this.TargetDirectory.FullName + "\\Backup", null), ".bak", BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var settings = new RepositorySettings(PathAndSpecialFolder.Create(this.TargetDirectory), false, backupSettings);
            return new SingletonRepository(settings);
        }
    }
}