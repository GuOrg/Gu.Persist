namespace Gu.Settings.Tests.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using Moq;
    using NUnit.Framework;

    public class FileInfoExtTests
    {
        private BackupSettings _setting;
        private FileInfo _file;
        private readonly DirectoryInfo _directoryInfo = new DirectoryInfo(@"C:\Temp");
        private FileInfo _backupFile;

        [SetUp]
        public void SetUp()
        {
            _setting = new BackupSettings(
                _directoryInfo,
                true,
                BackupSettings.DefaultExtension,
                BackupSettings.DefaultTimeStampFormat,
                false,
                3,
                3);
            _file = _directoryInfo.CreateFileInfoInDirectory("Meh.cfg");
            _backupFile = _file.WithNewExtension(_setting.Extension);
        }

        [TestCase("New", true)]
        public void IsValidFileName(string name, bool expected)
        {
            Assert.AreEqual(expected, FileInfoExt.IsValidFileName(name));
        }

        [TestCase("bak", @"C:\Temp\Meh.bak")]
        [TestCase(".bak", @"C:\Temp\Meh.bak")]
        public void ChangeExtension(string extension, string expected)
        {
            var newFile = _file.WithNewExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase("delete", @"C:\Temp\Meh.cfg.delete")]
        [TestCase(".delete", @"C:\Temp\Meh.cfg.delete")]
        public void AppendExtension(string extension, string expected)
        {
            var newFile = _file.WithAppendedExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase(@"C:\Temp\Meh.cfg", "cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".bak", null)]
        public void RemoveExtension(string filename, string extension, string expected)
        {
            if (expected != null)
            {
                var newFile = _file.WithRemovedExtension(extension);
                Assert.AreEqual(expected, newFile.FullName);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => _file.WithRemovedExtension(extension));
            }
        }

        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New", @"C:\Temp\NewDir\New.2015_06_14_16_54_12.cfg")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New.cfg", @"C:\Temp\NewDir\New.2015_06_14_16_54_12.cfg")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New.bak", @"C:\Temp\NewDir\New.2015_06_14_16_54_12.bak")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg.delete", "New.bak.delete", @"C:\Temp\NewDir\New.2015_06_14_16_54_12.bak.delete")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg.delete", "New.bak", @"C:\Temp\NewDir\New.2015_06_14_16_54_12.bak.delete")]
        public void WithNewNameTimeStamped(string filename, string newName, string expected)
        {
            var settings = Mock.Of<IBackupSettings>(x => x.Directory == new DirectoryInfo(@"C:\Temp\NewDir") &&
                                                         x.TimeStampFormat == BackupSettings.DefaultTimeStampFormat);
            var file = new FileInfo(filename);
            var newFile = file.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, file.FullName);
        }

        [TestCase(@"C:\Temp\Old.cfg", "New", @"C:\Temp\NewDir\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.cfg", @"C:\Temp\NewDir\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg.delete", "New", @"C:\Temp\NewDir\New.cfg.delete")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak", @"C:\Temp\NewDir\New.bak")]
        [TestCase(@"C:\Temp\Old.cfg.delete", "New.bak", @"C:\Temp\NewDir\New.bak.delete")]
        public void WithNewNameNoTimestampBackup(string filename, string newName, string expected)
        {
            var settings = Mock.Of<IBackupSettings>(x => x.Directory == new DirectoryInfo(@"C:\Temp\NewDir") &&
                                                         x.TimeStampFormat == (string)null);
            var file = new FileInfo(filename);
            var newFile = file.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, file.FullName);
        }

        [TestCase(@"C:\Temp\Old.cfg", "New", @"C:\Temp\NewDir\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.cfg", @"C:\Temp\NewDir\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak", @"C:\Temp\NewDir\New.bak")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak.delete", @"C:\Temp\NewDir\New.bak.delete")]
        public void WithNewNameNoTimestamp(string filename, string newName, string expected)
        {
            var dir = new DirectoryInfo(@"C:\Temp\NewDir");
            var settings = Mock.Of<IFileSettings>(x => x.Directory == dir);
            var file = new FileInfo(filename);

            var newFile = file.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, file.FullName);
        }

        [Test]
        public void AddTimeStamp()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = _file.WithTimeStamp(time, _setting);
            Assert.AreEqual(@"C:\Temp\Meh.2015_06_14_16_54_12.cfg", timestamped.FullName);
        }

        [Test]
        public void GetTimeStamp()
        {
            var timestamped = new FileInfo(@"C:\Temp\Meh.2015_06_14_16_54_12.bak");
            var actual = timestamped.GetTimeStamp(_setting);
            var expected = new DateTime(2015, 06, 14, 16, 54, 12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeStampRoundtrip()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = _backupFile.WithTimeStamp(time, _setting);
            var actual = timestamped.GetTimeStamp(_setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(_setting);
            Assert.AreEqual(_backupFile.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void TimeStampRoundtrip2()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = _file.WithTimeStamp(time, _setting);
            var actual = timestamped.GetTimeStamp(_setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(_setting);
            Assert.AreEqual(_file.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void RemoveTimeStamp()
        {
            var fileInfo = _file.WithRemovedTimeStamp(_setting);
            Assert.AreEqual(_file.FullName, fileInfo.FullName);
        }

        [Test]
        public void CreatePatternDefaultFormat()
        {
            var format = BackupSettings.DefaultTimeStampFormat;
            var pattern = FileInfoExt.CreateTimeStampPattern(format);
            var dateTime = new DateTime(2015, 06, 14, 12, 33, 24);
            var s = "." + dateTime.ToString(format, CultureInfo.InvariantCulture);
            var strictPattern = string.Format("^{0}$", pattern);
            Console.WriteLine(pattern);
            Assert.AreEqual(@"\.(?<timestamp>\d+_\d+_\d+_\d+_\d+_\d+)", pattern);
            StringAssert.IsMatch(strictPattern, s);
        }
    }
}
