namespace Gu.Settings.Tests.Backup
{
    using System;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Backup;

    using NUnit.Framework;

    // ReSharper disable once TestClassNameSuffixWarning
    public class BackupFileTests : BackupTests
    {
        [Test]
        public void GetAllBackupsForNoTimeStamp()
        {
            File.VoidCreate();
            Backup.VoidCreate();
            var restores = BackupFile.GetAllBackupsFor(File, Setting);
            Assert.AreEqual(1, restores.Count);
            Assert.AreEqual(Backup.FullName, restores[0].File.FullName);
        }

        [Test]
        public void GetAllBackupsFor()
        {
            File.VoidCreate();
            foreach (var backup in TimestampedBackups)
            {
                backup.VoidCreate();
            }
            var restores = BackupFile.GetAllBackupsFor(File, Setting);
            var expected = TimestampedBackups.Select(x => x.FullName).OrderBy(x => x).ToArray();
            var actual = restores.Select(x => x.File.FullName).OrderBy(x => x).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRestoreFileForNoTimeStamp()
        {
            File.VoidCreate();
            Backup.VoidCreate();
            var restore = BackupFile.GetRestoreFileFor(File, Setting);
            Assert.AreEqual(Backup.FullName, restore.FullName);
        }

        [Test]
        public void GetRestoreFileFor()
        {
            Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            File.VoidCreate();
            foreach (var backup in TimestampedBackups)
            {
                backup.VoidCreate();
            }
            var restore = BackupFile.GetRestoreFileFor(File, Setting);
            Assert.AreEqual(BackupOneMinuteOld.FullName, restore.FullName);
        }

        [TestCase(@"C:\Temp\Gu.Settings\BackupFileTests\Meh.bak")]
        public void CreateForNoTimestamp(string expected)
        {
            File.VoidCreate();
            var setting = new BackupSettings(File.Directory, true, BackupSettings.DefaultExtension, null, false, 1, Int32.MaxValue);
            var backup = BackupFile.CreateFor(File, setting);
            Assert.AreEqual(expected, backup.FullName);
        }

        [TestCase(@"C:\\Temp\\Gu.Settings\\BackupFileTests\\Meh\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak")]
        public void CreateFor(string expected)
        {
            File.VoidCreate();
            Setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            var backup = BackupFile.CreateFor(File, Setting);
            StringAssert.IsMatch(expected, backup.FullName);
        }

        [TestCase(@"C:\Temp\Meh.2015_06_13_17_05_15.bak")]
        public void GetTimeStamp(string fileName)
        {
            var file = new FileInfo(fileName);
            var setting = new BackupSettings(file.Directory, true, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, false, 1, Int32.MaxValue);
            var timeStamp = file.GetTimeStamp(setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }
    }
}