namespace Gu.Settings.Core.Tests.Backup
{
    using System;
    using System.IO;

    using Gu.Settings.Core;

    using NUnit.Framework;

    public class BackupTests
    {
        protected readonly DirectoryInfo Directory;
        protected BackupSettings Setting;
        protected FileInfo File;
        protected FileInfo Backup;
        protected FileInfo BackupOneMinuteOld;
        protected FileInfo BackupOneHourOld;
        protected FileInfo BackupOneDayOld;
        protected FileInfo BackupOneMonthOld;
        protected FileInfo BackupOneYearOld;
        protected FileInfo[] TimestampedBackups;
        protected FileInfo SoftDelete;
        protected FileInfo OtherBackup;

        public BackupTests()
        {
            Directory = new DirectoryInfo(@"C:\Temp\Gu.Settings\" + GetType().Name);
            Directory.CreateIfNotExists();

            Setting = new BackupSettings(Directory, true, BackupSettings.DefaultExtension, BackupSettings.DefaultTimeStampFormat, false, 2, 3);

            File = Directory.CreateFileInfoInDirectory("Meh.cfg");
            SoftDelete = File.WithAppendedExtension(FileHelper.SoftDeleteExtension);
            Backup = Directory.CreateFileInfoInDirectory("Meh.bak");

            BackupOneMinuteOld = Backup.WithTimeStamp(DateTime.Now.AddMinutes(-1), Setting);
            BackupOneHourOld = Backup.WithTimeStamp(DateTime.Now.AddHours(-1), Setting);
            BackupOneDayOld = Backup.WithTimeStamp(DateTime.Now.AddDays(-1), Setting);
            BackupOneMonthOld = Backup.WithTimeStamp(DateTime.Now.AddMonths(-1), Setting);
            BackupOneYearOld = Backup.WithTimeStamp(DateTime.Now.AddYears(-1), Setting);

            OtherBackup = Directory.CreateFileInfoInDirectory("Other.bak").WithTimeStamp(DateTime.Now.AddHours(1), Setting);

            TimestampedBackups = new[]
                                      {
                                          BackupOneMinuteOld,
                                          BackupOneHourOld,
                                          BackupOneDayOld,
                                          BackupOneMonthOld,
                                          BackupOneYearOld
                                      };
        }

        [SetUp]
        public virtual void SetUp()
        {
            File.Delete();
            Backup.Delete();
            foreach (var backup in TimestampedBackups)
            {
                backup.Delete();
            }

            OtherBackup.Delete();
            OtherBackup.VoidCreate();
            SoftDelete.Delete();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete();
            Backup.Delete();

            foreach (var backup in TimestampedBackups)
            {
                backup.Delete();
            }

            OtherBackup.Delete();
            SoftDelete.Delete();
        }
    }
}