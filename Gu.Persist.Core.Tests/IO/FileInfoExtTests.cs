// ReSharper disable AssignNullToNotNullAttribute
namespace Gu.Persist.Core.Tests.IO
{
    using System;
    using System.Globalization;

    using Gu.Persist.Core;

    using NUnit.Framework;

    public class FileInfoExtTests
    {
        [TestCase("New", true)]
        public void IsValidFileName(string name, bool expected)
        {
            Assert.AreEqual(expected, FileInfoExt.IsValidFileName(name));
        }

        [TestCase("bak")]
        [TestCase(".bak")]
        public void ChangeExtension(string extension)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var file = directory.CreateFileInfoInDirectory(nameof(this.ChangeExtension) + ".cfg");
            var newFile = file.WithNewExtension(extension);
            var expected = directory.CreateFileInfoInDirectory(nameof(this.ChangeExtension) + ".bak");
            Assert.AreEqual(expected.FullName, newFile.FullName);
        }

        [TestCase("delete")]
        [TestCase(".delete")]
        public void AppendExtension(string extension)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var file = directory.CreateFileInfoInDirectory(nameof(this.AppendExtension) + ".cfg");
            var newFile = file.WithAppendedExtension(extension);
            var expected = directory.CreateFileInfoInDirectory(nameof(this.AppendExtension) + ".cfg.delete");
            Assert.AreEqual(expected.FullName, newFile.FullName);
        }

        [TestCase("cfg")]
        [TestCase(".cfg")]
        [TestCase("bak")]
        [TestCase(".bak")]
        public void RemoveExtension(string extension)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            if (extension.EndsWith("cfg", StringComparison.Ordinal))
            {
                var file = directory.CreateFileInfoInDirectory(nameof(this.AppendExtension) + ".cfg");
                var newFile = file.WithRemovedExtension(extension);
                var expected = directory.CreateFileInfoInDirectory(nameof(this.AppendExtension));
                Assert.AreEqual(expected.FullName, newFile.FullName);
            }
            else
            {
                var file = directory.CreateFileInfoInDirectory(nameof(this.AppendExtension) + ".cfg");
                var exception = Assert.Throws<ArgumentException>(() => file.WithRemovedExtension(extension));
                Assert.AreEqual("Fail\r\nParameter name: extension", exception.Message);
            }
        }

        [TestCase("Old.2015_06_14_16_54_12.cfg",        "New",            "New.2015_06_14_16_54_12.cfg")]
        [TestCase("Old.2015_06_14_16_54_12.cfg",        "New.cfg",        "New.2015_06_14_16_54_12.cfg")]
        [TestCase("Old.2015_06_14_16_54_12.cfg",        "New.bak",        "New.2015_06_14_16_54_12.bak")]
        [TestCase("Old.2015_06_14_16_54_12.cfg.delete", "New.bak.delete", "New.2015_06_14_16_54_12.bak.delete")]
        [TestCase("Old.2015_06_14_16_54_12.cfg.delete", "New.bak",        "New.2015_06_14_16_54_12.bak.delete")]
        public void WithNewNameTimeStamped(string oldName, string newName, string expected)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var oldFile = directory.CreateFileInfoInDirectory(oldName);
            var newFile = oldFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.Name);
            Assert.AreEqual(oldName, oldFile.Name);
        }

        [TestCase("Old.cfg",        "New",     "New.cfg")]
        [TestCase("Old.cfg",        "New.cfg", "New.cfg")]
        [TestCase("Old.cfg.delete", "New",     "New.cfg.delete")]
        [TestCase("Old.cfg",        "New.bak", "New.bak")]
        [TestCase("Old.cfg.delete", "New.bak", "New.bak.delete")]
        public void WithNewNameNoTimestampBackup(string oldName, string newName, string expected)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", null, 1, int.MaxValue);
            var oldFile = directory.CreateFileInfoInDirectory(oldName);
            var newFile = oldFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.Name);
            Assert.AreEqual(oldName, oldFile.Name);
        }

        [TestCase("Old.cfg", "New",            "New.cfg")]
        [TestCase("Old.cfg", "New.cfg",        "New.cfg")]
        [TestCase("Old.cfg", "New.bak",        "New.bak")]
        [TestCase("Old.cfg", "New.bak.delete", "New.bak.delete")]
        public void WithNewNameNoTimestamp(string oldName, string newName, string expected)
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new FileSettings(directory.FullName, ".cfg");
            var oldFile = directory.CreateFileInfoInDirectory(oldName);
            var newFile = oldFile.WithNewName(newName, settings);
            Assert.AreEqual(expected, newFile.Name);
            Assert.AreEqual(oldName, oldFile.Name);
        }

        [Test]
        public void AddTimeStamp()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var file = directory.CreateFileInfoInDirectory(nameof(this.AddTimeStamp) + ".cfg");
            var timestamped = file.WithTimeStamp(time, settings);
            var expected = directory.CreateFileInfoInDirectory(nameof(this.AddTimeStamp) + ".2015_06_14_16_54_12.cfg");
            Assert.AreEqual(expected.FullName, timestamped.FullName);
        }

        [Test]
        public void GetTimeStamp()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var file = directory.CreateFileInfoInDirectory(nameof(this.AddTimeStamp) + ".2015_06_14_16_54_12.bak");
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var actual = file.GetTimeStamp(settings);
            var expected = new DateTime(2015, 06, 14, 16, 54, 12);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TimeStampRoundtrip()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var file = directory.CreateFileInfoInDirectory(nameof(this.TimeStampRoundtrip) + ".bak");
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = file.WithTimeStamp(time, settings);
            var actual = timestamped.GetTimeStamp(settings);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(settings);
            Assert.AreEqual(file.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void TimeStampRoundtrip2()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var file = directory.CreateFileInfoInDirectory(nameof(this.TimeStampRoundtrip) + ".bak");
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = file.WithTimeStamp(time, settings);
            var actual = timestamped.GetTimeStamp(settings);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.WithRemovedTimeStamp(settings);
            Assert.AreEqual(file.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void RemoveTimeStamp()
        {
            var directory = Directories.TempDirectory.CreateSubdirectory("Gu.Persist.Tests")
                                       .CreateSubdirectory(this.GetType().FullName);
            var settings = new BackupSettings(directory.FullName, ".bak", BackupSettings.DefaultTimeStampFormat, 3, int.MaxValue);
            var file = directory.CreateFileInfoInDirectory(nameof(this.TimeStampRoundtrip) + ".2015_06_14_16_54_12.bak");

            var fileInfo = file.WithRemovedTimeStamp(settings);
            var expected = directory.CreateFileInfoInDirectory(nameof(this.TimeStampRoundtrip) + ".bak");
            Assert.AreEqual(expected.FullName, fileInfo.FullName);
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
