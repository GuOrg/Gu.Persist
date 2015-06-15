namespace Gu.Settings.Tests.IO
{
    using System;
    using NUnit.Framework;
    using Settings.IO;

    public class DirectoriesTests
    {
        [Test]
        public void Default()
        {
            var directoryInfo = Directories.Default;
            Console.WriteLine(directoryInfo.FullName);
        }

        [Test]
        public void MyDocuments()
        {
            var directoryInfo = Directories.MyDocuments;
            Console.WriteLine(directoryInfo.FullName);
        }
    }
}
