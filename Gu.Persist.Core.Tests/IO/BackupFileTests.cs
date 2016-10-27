namespace Gu.Persist.Tests.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Helpers;
    using NUnit.Framework;
    using Settings.IO;

    public class BackupFileTests
    {
        private DirectoryInfo _tempDir;
        private BackupSettings _setting;
        private FileInfo _file;
        private FileInfo _backup;
        private FileInfo _backup1;
        private FileInfo _backup2;
        private FileInfo _backup3;
        private FileInfo _backup4;
        private FileInfo _backup5;
        private FileInfo _softDelete;
        private FileInfo _otherBackup;
        private FileInfo[] _timestampedBackups;

        [SetUp]
        public void SetUp()
        {
            _tempDir = new DirectoryInfo(@"C:\Temp\Gu.Persist");
            _setting = new BackupSettings(true, _tempDir, ".bak", null, false, 1, Int32.MaxValue);
            _file = _tempDir.CreateFileInfoInDirectory("Meh.cfg");
            _backup = _tempDir.CreateFileInfoInDirectory("Meh.bak");

            _backup1 = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak", new DateTime(2015, 06, 13, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _backup2 = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak", new DateTime(2015, 06, 13, 21, 37, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _backup3 = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak", new DateTime(2015, 06, 12, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _backup4 = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak", new DateTime(2015, 06, 11, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _backup5 = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak", new DateTime(2015, 06, 10, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));

            _otherBackup = _tempDir.CreateFileInfoInDirectory(string.Format("Other{0}.bak", new DateTime(2015, 06, 13, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _softDelete = _tempDir.CreateFileInfoInDirectory(string.Format("Meh{0}.bak.delete", new DateTime(2015, 06, 10, 21, 38, 00).ToString(BackupSettings.DefaultTimeStampFormat, CultureInfo.InvariantCulture)));
            _timestampedBackups = new FileInfo[]
            {
                _backup1,
                _backup2,
                _backup3,
                _backup4,
                _backup5
            };
            _file.Delete();
            _backup.Delete();
            foreach (var backup in _timestampedBackups)
            {
                backup.Delete();
            }

            _otherBackup.Delete();
            _otherBackup.VoidCreate();
            _softDelete.VoidCreate();
        }

        [TearDown]
        public void TearDown()
        {
            _file.Delete();
            _backup.Delete();

            foreach (var backup in _timestampedBackups)
            {
                backup.Delete();
            }

            _otherBackup.Delete();
            _softDelete.Delete();
        }

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

        [TestCase(@"C:\\Temp\\Gu.Persist\\Meh\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak")]
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
            var timeStamp = BackupFile.GetTimeStamp(file, setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }
    }
}