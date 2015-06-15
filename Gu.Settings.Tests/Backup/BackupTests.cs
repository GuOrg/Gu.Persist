namespace Gu.Settings.Tests.Backup
{
    using System;
    using System.IO;

    using NUnit.Framework;

    public class BackupTests
    {
        protected readonly DirectoryInfo Directory;
        protected BackupSettings _setting;
        protected FileInfo _file;
        protected FileInfo _backup;
        protected FileInfo _backup1;
        protected FileInfo _backup2;
        protected FileInfo _backup3;
        protected FileInfo _backup4;
        protected FileInfo _backup5;
        protected FileInfo[] _timestampedBackups;
        protected FileInfo _softDelete;
        protected FileInfo _otherBackup;

        public BackupTests()
        {
            Directory =  new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            Directory.CreateIfNotExists();
        }

        [SetUp]
        public virtual void SetUp()
        {
            _setting = new BackupSettings(true, Directory, ".bak", BackupSettings.DefaultTimeStampFormat, false, 2, 3);
            _file = Directory.CreateFileInfoInDirectory("Meh.cfg");
            _backup = Directory.CreateFileInfoInDirectory("Meh.bak");

            _backup1 = _backup.AddTimeStamp(new DateTime(2015, 06, 13, 21, 38, 00), _setting);
            _backup2 = _backup.AddTimeStamp(new DateTime(2015, 06, 13, 21, 37, 00), _setting);
            _backup3 = _backup.AddTimeStamp(new DateTime(2015, 06, 12, 21, 38, 00), _setting);
            _backup4 = _backup.AddTimeStamp(new DateTime(2015, 06, 11, 21, 38, 00), _setting);
            _backup5 = _backup.AddTimeStamp(new DateTime(2015, 06, 10, 21, 38, 00), _setting);

            _otherBackup = Directory.CreateFileInfoInDirectory("Other.bak").AddTimeStamp(new DateTime(2015, 06, 13, 21, 38, 00), _setting);
            _softDelete = _backup.AppendExtension(".delete");
            _timestampedBackups = new[]
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
            _softDelete.Delete();
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
    }
}