namespace Gu.Settings.SystemXml.Tests
{
    using System;
    using System.IO;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests;
    using Gu.Settings.SystemXml;

    using NUnit.Framework;

    public class SimpleSample
    {
        private FileInfo _settingsFile;

        [SetUp]
        public void SetUp()
        {
            Directories.Default = new DirectoryInfo(@"C:\Temp\Gu.Settings\SimpleSample");
            _settingsFile = Directories.Default.CreateFileInfoInDirectory(nameof(XmlRepositorySettings) + ".cfg");
            if (_settingsFile.Exists)
            {
                _settingsFile.Delete();
            }
        }

        [Test]
        public void XmlSample()
        {
            var repository = new XmlRepository();
            Console.WriteLine(repository.Settings.DirectoryPath);

            Console.WriteLine(_settingsFile.FullName);
            AssertFile.Exists(true, _settingsFile); // using the default constructor bootstraps with a settings file

            var setting = repository.ReadOrCreate(() => new DummySerializable());
            setting.Value++;
            Assert.IsTrue(repository.IsDirty(setting));
            repository.Save(setting);
            Assert.IsFalse(repository.IsDirty(setting));
        }
    }
}
