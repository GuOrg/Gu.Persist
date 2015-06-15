namespace Gu.Settings.Tests.Backup
{
    using System;
    using System.IO;
    using System.Linq;

    using Gu.Settings.Backup;

    using NUnit.Framework;

    // ReSharper disable once TestClassNameSuffixWarning
    public class BackupFile_ : BackupTests
    {
        [Test]
        public void GetAllBackupsForNoTimeStamp()
        {
            _file.VoidCreate();
            _backup.VoidCreate();
            var restores = BackupFile.GetAllBackupsFor(_file, _setting);
            Assert.AreEqual(1, restores.Count);
            Assert.AreEqual(_backup.FullName, restores[0].File.FullName);
        }

        [Test]
        public void GetAllBackupsFor()
        {
            _file.VoidCreate();
            foreach (var backup in _timestampedBackups)
            {
                backup.VoidCreate();
            }
            var restores = BackupFile.GetAllBackupsFor(_file, _setting);
            var expected = _timestampedBackups.Select(x => x.FullName).OrderBy(x => x).ToArray();
            var actual = restores.Select(x => x.File.FullName).OrderBy(x => x).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void GetRestoreFileForNoTimeStamp()
        {
            _file.VoidCreate();
            _backup.VoidCreate();
            var restore = BackupFile.GetRestoreFileFor(_file, _setting);
            Assert.AreEqual(_backup.FullName, restore.FullName);
        }

        [Test]
        public void GetRestoreFileFor()
        {
            _setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            _file.VoidCreate();
            foreach (var backup in _timestampedBackups)
            {
                backup.VoidCreate();
            }
            var restore = BackupFile.GetRestoreFileFor(_file, _setting);
            Assert.AreEqual(_backup1.FullName, restore.FullName);
        }

        [TestCase(@"C:\Temp\Meh.cfg", @"C:\Temp\Meh.bak")]
        public void CreateForNoTimestamp(string fileName, string expected)
        {
            var file = new FileInfo(fileName);
            var setting = new BackupSettings(true, file.Directory, ".bak", null, false, 1, Int32.MaxValue);
            var backup = BackupFile.CreateFor(file, setting);
            Assert.AreEqual(expected, backup.FullName);
        }

        [TestCase(@"C:\\Temp\\Gu.Settings\\BackupFile_\\Meh\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak")]
        public void CreateFor(string expected)
        {
            _setting.TimeStampFormat = BackupSettings.DefaultTimeStampFormat;
            var backup = BackupFile.CreateFor(_file, _setting);
            StringAssert.IsMatch(expected, backup.FullName);
        }

        [TestCase(@"C:\Temp\Meh.2015_06_13_17_05_15.bak")]
        public void GetTimeStamp(string fileName)
        {
            var file = new FileInfo(fileName);
            var setting = new BackupSettings(true, file.Directory, ".bak", BackupSettings.DefaultTimeStampFormat, false, 1, Int32.MaxValue);
            var timeStamp = file.GetTimeStamp(setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }
    }
}