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
            this.File.CreatePlaceHolder();
            this.Backup.CreatePlaceHolder();
            var restores = BackupFile.GetAllBackupsFor(this.File, BackupSettings.Create(this.Directory));
            Assert.AreEqual(1, restores.Count);
            Assert.AreEqual(this.Backup.FullName, restores[0].File.FullName);
        }

        [Test]
        public void GetAllBackupsFor()
        {
            this.File.CreatePlaceHolder();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreatePlaceHolder();
            }

            var restores = BackupFile.GetAllBackupsFor(this.File, BackupSettings.Create(this.Directory, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1));
            var expected = this.TimestampedBackups.Select(x => x.FullName).OrderBy(x => x).ToArray();
            var actual = restores.Select(x => x.File.FullName).OrderBy(x => x).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRestoreFileForNoTimeStamp()
        {
            this.File.CreatePlaceHolder();
            this.Backup.CreatePlaceHolder();
            var restore = BackupFile.GetRestoreFileFor(this.File, BackupSettings.Create(this.Directory));
            Assert.AreEqual(this.Backup.FullName, restore.FullName);
        }

        [Test]
        public void GetRestoreFileFor()
        {
            this.File.CreatePlaceHolder();
            foreach (var backup in this.TimestampedBackups)
            {
                backup.CreatePlaceHolder();
            }

            var backupSettings = BackupSettings.Create(this.Directory, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1);
            var restore = BackupFile.GetRestoreFileFor(this.File, backupSettings);
            Assert.AreEqual(this.BackupOneMinuteOld.FullName, restore.FullName);
        }

        [TestCase(@"C:\Temp\Gu.Persist\BackupFileTests\Meh.bak")]
        public void CreateForNoTimestamp(string expected)
        {
            this.File.CreatePlaceHolder();
            var setting = BackupSettings.Create(this.File.Directory, BackupSettings.DefaultExtension, null, 1, int.MaxValue);
            var backup = BackupFile.CreateFor(this.File, setting);
            Assert.AreEqual(expected, backup.FullName);
        }

        [TestCase(@"C:\\Temp\\Gu.Persist\\BackupFileTests\\Meh\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak")]
        public void CreateFor(string expected)
        {
            this.File.CreatePlaceHolder();
            var backupSettings = BackupSettings.Create(this.Directory, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1);
            var backup = BackupFile.CreateFor(this.File, backupSettings);
            StringAssert.IsMatch(expected, backup.FullName);
        }

        [TestCase(@"C:\Temp\Meh.2015_06_13_17_05_15.bak")]
        public void GetTimeStamp(string fileName)
        {
            var file = new FileInfo(fileName);
            var setting = BackupSettings.Create(file.Directory, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var timeStamp = file.GetTimeStamp(setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }
    }
}