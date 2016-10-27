namespace Gu.Persist.Tests.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using Helpers;
    using NUnit.Framework;
    using Settings.IO;

    public class BackuperTests
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
        private Backuper _backuper;

        [SetUp]
        public void SetUp()
        {
            _tempDir = new DirectoryInfo(@"C:\Temp\Gu.Persist");
            _setting = new BackupSettings(true, _tempDir, ".bak", BackupSettings.DefaultTimeStampFormat, false, 1, Int32.MaxValue);
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
            _backuper = new Backuper(_setting);
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
        public void PurgeWhenNoFiles()
        {
            _setting.NumberOfBackups = 2;
            _setting.MaxAgeInDays = 2;
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(false, _backup1);
            AssertFile.Exists(false, _backup2);
            AssertFile.Exists(false, _backup3);
            AssertFile.Exists(false, _backup4);
            AssertFile.Exists(false, _backup5);
            //AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeBasedOnNumbers()
        {
            foreach (var backup in _timestampedBackups)
            {
                backup.VoidCreate();
            }
            _setting.NumberOfBackups = 2;
            _setting.MaxAgeInDays = Int32.MaxValue;
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(true, _backup1);
            AssertFile.Exists(true, _backup2);
            AssertFile.Exists(false, _backup3);
            AssertFile.Exists(false, _backup4);
            AssertFile.Exists(false, _backup5);
            AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeBasedOnDays()
        {
            foreach (var backup in _timestampedBackups)
            {
                backup.VoidCreate();
            }
            _setting.NumberOfBackups = Int32.MaxValue;
            _setting.MaxAgeInDays = 2;
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(true, _backup1);
            AssertFile.Exists(true, _backup2);
            AssertFile.Exists(true, _backup3);
            AssertFile.Exists(false, _backup4);
            AssertFile.Exists(false, _backup5);
            AssertFile.Exists(false, _softDelete);
        }
    }
}
