namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Linq;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    // ReSharper disable once TestClassNameSuffixWarning
    public class BackupFileTests : BackupTests
    {
        [Test]
        public void GetAllBackupsForNoTimeStamp()
        {
            this.File.VoidCreate();
            this.Backup.VoidCreate();
            var restores = BackupFile.GetAllBackupsFor(this.File, this.Setting);
            Assert.AreEqual(1, restores.Count);
            Assert.AreEqual(this.Backup.FullName, restores[0].File.FullName);
        }

        [Test]
        public void GetAllBackupsFor()
        {
            this.File.VoidCreate();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.VoidCreate();
            }

            var restores = BackupFile.GetAllBackupsFor(this.File, this.Setting);
            var expected = this.TimestampedBackups.Select(x => x.FullName).OrderBy(x => x).ToArray();
            var actual = restores.Select(x => x.File.FullName).OrderBy(x => x).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRestoreFileForNoTimeStamp()
        {
            this.File.VoidCreate();
            this.Backup.VoidCreate();
            var restore = BackupFile.GetRestoreFileFor(this.File, this.Setting);
            Assert.AreEqual(this.Backup.FullName, restore.FullName);
        }

        [Test]
        public void GetRestoreFileFor()
        {
            this.Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            this.File.VoidCreate();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.VoidCreate();
            }

            var restore = BackupFile.GetRestoreFileFor(this.File, this.Setting);
            Assert.AreEqual(this.BackupOneMinuteOld.FullName, restore.FullName);
        }

        [TestCase(@"C:\Temp\Gu.Persist\BackupFileTests\Meh.bak")]
        public void CreateForNoTimestamp(string expected)
        {
            this.File.VoidCreate();
            var setting = new BackupSettings(this.File.Directory, true, BackupSettings.DefaultExtension, null, false, 1, int.MaxValue);
            var backup = BackupFile.CreateFor(this.File, setting);
            Assert.AreEqual(expected, backup.FullName);
        }

        [TestCase(@"C:\\Temp\\Gu.Persist\\BackupFileTests\\Meh\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak")]
        public void CreateFor(string expected)
        {
            this.File.VoidCreate();
            this.Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            var backup = BackupFile.CreateFor(this.File, this.Setting);
            StringAssert.IsMatch(expected, backup.FullName);
        }

        [TestCase(@"C:\Temp\Meh.2015_06_13_17_05_15.bak")]
        public void GetTimeStamp(string fileName)
        {
            var file = new FileInfo(fileName);
            var setting = new BackupSettings(file.Directory, true, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, false, 1, int.MaxValue);
            var timeStamp = file.GetTimeStamp(setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }
    }
}