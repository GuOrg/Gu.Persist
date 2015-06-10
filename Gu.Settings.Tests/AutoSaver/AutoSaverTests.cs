namespace Gu.Settings.Tests.AutoSaver
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using Gu.Settings.Tests.Helpers;

    using NUnit.Framework;

    using AutoSaver = Gu.Settings.AutoSaver;

    public class AutoSaverTests
    {
        private FileInfo _file;
        private FileInfo _backup;
        private RepositorySetting _setting;
        private DummySerializable _dummy;
        private XmlRepository _repository;
        private AutoSaver _autoSaver;
        private FileInfos _fileInfos;
        private DummySubject<object> _subject;
        private ManualResetEvent _resetEvent;
        [SetUp]
        public void SetUp()
        {
            _file = new FileInfo(@"C:\Temp\AutoSaverTests.tmp");
            _backup = new FileInfo(@"C:\Temp\AutoSaverTests.bak");
            _fileInfos = new FileInfos(_file, _backup);

            _setting = new RepositorySetting(true, false, _file.Directory, ".tmp", ".bak");
            _file.Delete();
            _backup.Delete();
            _dummy = new DummySerializable(1);
            _repository = new XmlRepository(_setting);
            _autoSaver = new AutoSaver(_repository);
            _subject = new DummySubject<object>();
            _resetEvent = new ManualResetEvent(false);
        }

        [Test]
        public void SavesOnChange()
        {
            _autoSaver.Saved += AutoSaverOnSaved;
            _autoSaver.Add(_dummy, _fileInfos, _subject);
            _subject.OnNext(1);
            _resetEvent.WaitOne();
            AssertExists(true, _file);
            AssertExists(false, _backup);
            _resetEvent.Reset();
            _subject.OnNext(2);
            _resetEvent.WaitOne();
            AssertExists(true, _file);
            AssertExists(true, _backup);
            _autoSaver.Saved -= AutoSaverOnSaved;
        }

        [Test]
        public void RemovesSubscriptionTest()
        {
            var subscription = _autoSaver.Add(_dummy, _fileInfos, _subject);
            var fieldInfo = _autoSaver.GetType()
                                            .GetField("_subscriptions", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(fieldInfo);
            var subscriptions = (List<IDisposable>)fieldInfo.GetValue(_autoSaver);
            CollectionAssert.AreEqual(new[] { subscription }, subscriptions);
            subscription.Dispose();
            CollectionAssert.IsEmpty( subscriptions);
        }

        private void AutoSaverOnSaved(object sender, SaveEventArgs saveEventArgs)
        {
            _resetEvent.Set();
        }

        public static void AssertExists(bool expected, FileInfo fileInfo)
        {
            fileInfo.Refresh();
            Assert.AreEqual(expected, fileInfo.Exists);
        }
    }
}
