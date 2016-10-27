namespace Gu.Persist.SystemXml.Tests
{
    using System;
    using System.IO;

    using Gu.Persist.Core;
    using Gu.Persist.Core.Tests;
    using Gu.Persist.SystemXml;

    using NUnit.Framework;

    [Explicit("Sample")]
    public class SimpleSample
    {
        private FileInfo settingsFile;

        [SetUp]
        public void SetUp()
        {
            // Directories.Default = new DirectoryInfo(@"C:\Temp\Gu.Persist\SimpleSample");
            this.settingsFile = Directories.Default.CreateFileInfoInDirectory(nameof(XmlRepositorySettings) + ".cfg");
            if (this.settingsFile.Exists)
            {
                this.settingsFile.Delete();
            }
        }

        [Test]
        public void XmlSample()
        {
            var repository = new XmlRepository();
            Console.WriteLine(repository.Settings.DirectoryPath);

            Console.WriteLine(this.settingsFile.FullName);
            AssertFile.Exists(true, this.settingsFile); // using the default constructor bootstraps with a settings file

            var setting = repository.ReadOrCreate(() => new DummySerializable());
            setting.Value++;
            Assert.IsTrue(repository.IsDirty(setting));
            repository.Save(setting);
            Assert.IsFalse(repository.IsDirty(setting));
        }
    }
}
