namespace Gu.Persist.Core.Tests.Backup
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Gu.Persist.Core;
    using Gu.Persist.Core.Backup;

    using NUnit.Framework;

    [NonParallelizable]
    public static class BackupFileTests
    {
        private static readonly DirectoryInfo Directory = Directories.TempDirectory
                                                                     .CreateSubdirectory("Gu.Persist.Tests")
                                                                     .CreateSubdirectory(typeof(BackupFileTests).FullName);

        [SetUp]
        public static void SetUp()
        {
            Directory.Refresh();
            Directory.CreateIfNotExists();
        }

        [TearDown]
        public static void TearDown()
        {
            Directory.DeleteIfExists(true);
        }

        [Test]
        public static void GetAllBackupsForNoTimeStamp()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            file.CreateFileOnDisk();
            backup.CreateFileOnDisk();
            var restores = BackupFile.GetAllBackupsFor(file, new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, null, 1, 1));
            Assert.AreEqual(1, restores.Count);
            Assert.AreEqual(backup.FullName, restores[0].File.FullName);
        }

        [Test]
        public static void GetAllBackupsFor()
        {
            var settings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            file.CreateFileOnDisk();
            var backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneMinuteOld.CreateFileOnDisk();
            var backupOneHourOld = backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings);
            backupOneHourOld.CreateFileOnDisk();
            var backupOneDayOld = backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings);
            backupOneDayOld.CreateFileOnDisk();
            var backupOneMonthOld = backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings);
            backupOneMonthOld.CreateFileOnDisk();
            var backupOneYearOld = backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings);
            backupOneYearOld.CreateFileOnDisk();

            var restores = BackupFile.GetAllBackupsFor(file, settings);
            var expected = new[]
            {
                backupOneYearOld.Name,
                backupOneMonthOld.Name,
                backupOneDayOld.Name,
                backupOneHourOld.Name,
                backupOneMinuteOld.Name,
            };
            var actual = restores.Select(x => x.File.Name).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void GetRestoreFileForNoTimeStamp()
        {
            var file = CreateFile();
            var backup = CreateBackupFile();
            file.CreateFileOnDisk();
            backup.CreateFileOnDisk();
            var restore = BackupFile.GetRestoreFileFor(file, Default.BackupSettings(Directory));
            Assert.AreEqual(backup.FullName, restore.FullName);
        }

        [Test]
        public static void GetRestoreFileFor()
        {
            var settings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1);
            var file = CreateFile();
            var backup = CreateBackupFile();
            file.CreateFileOnDisk();
            var backupOneMinuteOld = backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), settings);
            backupOneMinuteOld.CreateFileOnDisk();
            backup.WithTimeStamp(DateTime.Now.AddHours(-1), settings).CreateFileOnDisk();
            backup.WithTimeStamp(DateTime.Now.AddDays(-1), settings).CreateFileOnDisk();
            backup.WithTimeStamp(DateTime.Now.AddMonths(-1), settings).CreateFileOnDisk();
            backup.WithTimeStamp(DateTime.Now.AddYears(-1), settings).CreateFileOnDisk();

            var restore = BackupFile.GetRestoreFileFor(file, settings);
            Assert.AreEqual(backupOneMinuteOld.FullName, restore.FullName);
        }

        [Test]
        public static void CreateForNoTimestamp()
        {
            var file = CreateFile();
            file.CreateFileOnDisk();
            var setting = new BackupSettings(file.DirectoryName, BackupSettings.DefaultExtension, null, 1, int.MaxValue);
            var backup = BackupFile.CreateFor(file, setting);
            StringAssert.IsMatch(@"BackupFileTests\\CreateForNoTimestamp\.bak", backup.FullName);
        }

        [Test]
        public static void CreateFor()
        {
            var file = CreateFile();
            file.CreateFileOnDisk();
            var backupSettings = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, 1);
            var backup = BackupFile.CreateFor(file, backupSettings);
            StringAssert.IsMatch(@"BackupFileTests\\CreateFor\.\d\d\d\d_\d\d_\d\d_\d\d_\d\d_\d\d\.bak", backup.FullName);
        }

        [TestCase(@"C:\Temp\Meh.2015_06_13_17_05_15.bak")]
        public static void GetTimeStamp(string fileName)
        {
            var file = new FileInfo(fileName);
            var setting = new BackupSettings(Directory.FullName, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, 1, int.MaxValue);
            var timeStamp = file.GetTimeStamp(setting);
            Assert.AreEqual(new DateTime(2015, 06, 13, 17, 05, 15), timeStamp);
        }

        private static FileInfo CreateFile([CallerMemberName] string name = null) => Directory.CreateFileInfoInDirectory(name + ".cfg");

        private static FileInfo CreateBackupFile([CallerMemberName] string name = null) => Directory.CreateFileInfoInDirectory(name + ".bak");
    }
}