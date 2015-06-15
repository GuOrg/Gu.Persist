namespace Gu.Settings.Tests.Backup
{
    using Gu.Settings.Backup;

    using NUnit.Framework;

    public class Backuper_ : BackupTests
    {
        private Backuper _backuper;

        [SetUp]
        public override void SetUp()
        {
            _backuper = (Backuper)Backuper.Create(_setting);
        }

        [Test]
        public void BackupWhenNotExtsis()
        {
            _setting.NumberOfBackups = 1;
            _setting.TimeStampFormat = null;
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _backup);

            _backuper.Backup(_file);
            
            AssertFile.Exists(false, _file);
            AssertFile.Exists(false, _backup);
        }


        [Test]
        public void RestoreWhenHasRestoreAndSoftDeleteFile()
        {
            Assert.Fail();

            //_file.WriteAllText("File");
            //_backup.WriteAllText("Restore");
            //_softDeleteFile.WriteAllText("Soft");
            //_file.Restore(_backup);
            //AssertFile.Exists(true, _file);
            //Assert.AreEqual("Soft", _file.ReadAllText());
            //AssertFile.Exists(true, _backup);
            //AssertFile.Exists(false, _softDeleteFile);
        }

        [Test]
        public void PurgeAll()
        {
            Assert.Fail("do we want this?");
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
            _setting.MaxAgeInDays = int.MaxValue;
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
            _setting.NumberOfBackups = int.MaxValue;
            _setting.MaxAgeInDays = 2;
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(true, _backup1);
            AssertFile.Exists(true, _backup2);
            AssertFile.Exists(true, _backup3);
            AssertFile.Exists(false, _backup4);
            AssertFile.Exists(false, _backup5);
            AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletesNoTimestamp()
        {
            _softDelete.VoidCreate();
            _file.VoidCreate();
            _backup.VoidCreate();
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void PurgeDeletesSoftDeletes()
        {
            foreach (var backup in _timestampedBackups)
            {
                backup.VoidCreate();
            }
            _softDelete.VoidCreate();
            _file.VoidCreate();
            _backup.VoidCreate();
            _setting.NumberOfBackups = int.MaxValue;
            _setting.MaxAgeInDays = int.MaxValue;
            _backuper.PurgeBackups(_file);
            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            AssertFile.Exists(false, _softDelete);
        }

        [Test]
        public void Rename()
        {
            Assert.Fail("Backups must be renamed when original file is renamed");
        }
    }
}
