namespace Gu.Settings.Tests.IO
{
    using System;
    using System.Globalization;
    using System.IO;
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
            _backupFile = _file.ChangeExtension(_setting.Extension);
        }

        [TestCase("bak", @"C:\Temp\Meh.bak")]
        [TestCase(".bak", @"C:\Temp\Meh.bak")]
        public void ChangeExtension(string extension, string expected)
        {
            var newFile = _file.ChangeExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase("delete", @"C:\Temp\Meh.cfg.delete")]
        [TestCase(".delete", @"C:\Temp\Meh.cfg.delete")]
        public void AppendExtension(string extension, string expected)
        {
            var newFile = _file.AppendExtension(extension);
            Assert.AreEqual(expected, newFile.FullName);
        }

        [TestCase(@"C:\Temp\Meh.cfg", "cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".cfg", @"C:\Temp\Meh")]
        [TestCase(@"C:\Temp\Meh.cfg", ".bak", null)]
        public void RemoveExtension(string filename, string extension, string expected)
        {
            if (expected != null)
            {
                var newFile = _file.RemoveExtension(extension);
                Assert.AreEqual(expected, newFile.FullName);
            }
            else
            {
                Assert.Throws<ArgumentException>(() => _file.RemoveExtension(extension));
            }
        }

        [Test]
        public void AddTimeStamp()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = _file.AddTimeStamp(time, _setting);
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
            var timestamped = _backupFile.AddTimeStamp(time, _setting);
            var actual = timestamped.GetTimeStamp(_setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.RemoveTimeStamp(_setting);
            Assert.AreEqual(_backupFile.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void TimeStampRoundtrip2()
        {
            var time = new DateTime(2015, 06, 14, 16, 54, 12);
            var timestamped = _file.AddTimeStamp(time, _setting);
            var actual = timestamped.GetTimeStamp(_setting);
            Assert.AreEqual(time, actual);

            var removeTimeStamp = timestamped.RemoveTimeStamp(_setting);
            Assert.AreEqual(_file.FullName, removeTimeStamp.FullName);
        }

        [Test]
        public void RemoveTimeStamp()
        {
            var fileInfo = _file.RemoveTimeStamp(_setting);
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
