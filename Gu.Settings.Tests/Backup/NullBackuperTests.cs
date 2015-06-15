namespace Gu.Settings.Tests.Backup
{
    using Gu.Settings.Backup;

    using NUnit.Framework;

    public class NullBackuper_ : BackupTests
    {
        private NullBackuper _backuper;

        [SetUp]
        public override void SetUp()
        {
            _backuper = NullBackuper.Default;
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
    }
}
