namespace Gu.Persist.Core.Tests.IO
{
    using System;
    using System.Globalization;
    using System.IO;

    using Gu.Persist.Core;

    using Moq;

    using NUnit.Framework;

    public class FileInfoExtTests
    {
        private readonly DirectoryInfo directoryInfo = new DirectoryInfo(@"C:\Temp");
        private BackupSettings setting;
        private FileInfo file;
        private FileInfo backupFile;

        [SetUp]
        public void SetUp()
        {
            this.setting = new BackupSettings(
                this.directoryInfo,
                BackupSettings.DefaultExtension,
                BackupSettings.DefaultTimeStampFormat,
                3,
                3);
            this.file = this.directoryInfo.CreateFileInfoInDirectory("Meh.cfg");
            this.backupFile = this.file.WithNewExtension(this.setting.Extension);
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
            var newFile = this.file.WithNewExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase("delete", @"C:\Temp\Meh.cfg.delete")]
        [TestCase(".delete", @"C:\Temp\Meh.cfg.delete")]
        public void AppendExtension(string extension, string expected)
        {
            var newFile = this.file.WithAppendedExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase(@"C:\Temp\Meh.cfg", "cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".bak", null)]
        public void RemoveExtension(string filename, string extension, string expected)
        {
            if (expected != null)
            {
                var newFile = this.file.WithRemovedExtension(extension);
                Assert.AreEqual(expected, newFile.FullName);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => this.file.WithRemovedExtension(extension));
            }
        }

        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New", @"C:\Temp\New.2015_06_14_16_54_12.cfg")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New.cfg", @"C:\Temp\New.2015_06_14_16_54_12.cfg")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg", "New.bak", @"C:\Temp\New.2015_06_14_16_54_12.bak")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg.delete", "New.bak.delete", @"C:\Temp\New.2015_06_14_16_54_12.bak.delete")]
        [TestCase(@"C:\Temp\Old.2015_06_14_16_54_12.cfg.delete", "New.bak", @"C:\Temp\New.2015_06_14_16_54_12.bak.delete")]
        public void WithNewNameTimeStamped(string filename, string newName, string expected)
        {
            var settings = Mock.Of<IBackupSettings>(x => x.TimeStampFormat == BackupSettings.DefaultTimeStampFormat);
            var originalFile = new FileInfo(filename);
            var newFile = originalFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, originalFile.FullName);
        }

        [TestCase(@"C:\Temp\Old.cfg", "New", @"C:\Temp\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.cfg", @"C:\Temp\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg.delete", "New", @"C:\Temp\New.cfg.delete")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak", @"C:\Temp\New.bak")]
        [TestCase(@"C:\Temp\Old.cfg.delete", "New.bak", @"C:\Temp\New.bak.delete")]
        public void WithNewNameNoTimestampBackup(string filename, string newName, string expected)
        {
            var settings = Mock.Of<IBackupSettings>(x => x.TimeStampFormat == (string)null);
            var originalFile = new FileInfo(filename);
            var newFile = originalFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, originalFile.FullName);
        }

        [TestCase(@"C:\Temp\Old.cfg", "New", @"C:\Temp\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.cfg", @"C:\Temp\New.cfg")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak", @"C:\Temp\New.bak")]
        [TestCase(@"C:\Temp\Old.cfg", "New.bak.delete", @"C:\Temp\New.bak.delete")]
        public void WithNewNameNoTimestamp(string filename, string newName, string expected)
        {
            var settings = Mock.Of<IFileSettings>();
            var originalFile = new FileInfo(filename);
            var newFile = originalFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.FullName);
            Assert.AreEqual(filename, originalFile.FullName);
        }

        [Test]
        public void AddTimeStamp()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = this.file.WithTimeStamp(time, this.setting);
            Assert.AreEqual(@"C:\Temp\Meh.2015_06_14_16_54_12.cfg", timestamped.FullName);
        }

        [Test]
        public void GetTimeStamp()
        {
            var timestamped = new FileInfo(@"C:\Temp\Meh.2015_06_14_16_54_12.bak");
            var actual = timestamped.GetTimeStamp(this.setting);
            var expected = new DateTime(2015, 06, 14, 16, 54, 12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeStampRoundtrip()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = this.backupFile.WithTimeStamp(time, this.setting);
            var actual = timestamped.GetTimeStamp(this.setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(this.setting);
            Assert.AreEqual(this.backupFile.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void TimeStampRoundtrip2()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = this.file.WithTimeStamp(time, this.setting);
            var actual = timestamped.GetTimeStamp(this.setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(this.setting);
            Assert.AreEqual(this.file.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void RemoveTimeStamp()
        {
            var fileInfo = this.file.WithRemovedTimeStamp(this.setting);
            Assert.AreEqual(this.file.FullName, fileInfo.FullName);
        }

        [Test]
        public void CreatePatternDefaultFormat()
        {
            var format = BackupSettings.DefaultTimeStampFormat;
            var pattern = FileInfoExt.CreateTimeStampPattern(format);
            var dateTime = new DateTime(2015, 06, 14, 12, 33, 24);
            var s = "." + dateTime.ToString(format, CultureInfo.InvariantCulture);
            var strictPattern = $"^{pattern}$";
            ////Console.WriteLine(pattern);
            Assert.AreEqual(@"\.(?<timestamp>\d+_\d+_\d+_\d+_\d+_\d+)", pattern);
            StringAssert.IsMatch(strictPattern, s);
        }
    }
}
