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

    [Explicit("Finish later or remove autosaver, not sure it adds anything.")]
    public class AutoSaverTests
    {
        private FileInfo _file;
        private FileInfo _backup;
        private RepositorySetting _setting;
        private DummySerializable _dummy;
        private XmlRepository _repository;
        private AutoSaver _autoSaver;
        private DummySubject<object> _subject;
        private ManualResetEvent _resetEvent;
        private BackupSettings _backupSettings;
        private FileInfo _temp;

        [SetUp]
        public void SetUp()
        {
            _backupSettings = new BackupSettings(true, _file.Directory, ".bak", null, false, 1, Int32.MaxValue);
            _setting = new RepositorySetting(true, _file.Directory, _backupSettings, ".cfg", ".tmp");

            _file = new FileInfo(string.Format(@"C:\Temp\{0}{1}", GetType().Name, _setting.Extension));
            _temp = new FileInfo(string.Format(@"C:\Temp\{0}{1}", GetType().Name, _setting.TempExtension));
            _backup = new FileInfo(string.Format(@"C:\Temp\{0}{1}", GetType().Name, _setting.BackupSettings.Extension));

            _file.Delete();
            _temp.Delete();
            _backup.Delete();

            _repository = new XmlRepository(_setting);
            _autoSaver = new AutoSaver(_repository);
            _subject = new DummySubject<object>();
            _resetEvent = new ManualResetEvent(false);

            _dummy = new DummySerializable(1);
        }

        [Test]
        public void SavesOnChange()
        {
            _autoSaver.Saved += AutoSaverOnSaved;
            _autoSaver.Add(_dummy, _file, _temp, _subject);
            _subject.OnNext(1);
            _resetEvent.WaitOne();
            AssertFile.Exists(true, _file);
            AssertFile.Exists(false, _backup);
            _resetEvent.Reset();
            _subject.OnNext(2);
            _resetEvent.WaitOne();
            AssertFile.Exists(true, _file);
            AssertFile.Exists(true, _backup);
            _autoSaver.Saved -= AutoSaverOnSaved;
        }

        [Test]
        public void RemovesSubscriptionTest()
        {
            var subscription = _autoSaver.Add(_dummy, _file, _temp, _subject);
            var fieldInfo = _autoSaver.GetType()
                                            .GetField("_subscriptions", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(fieldInfo);
            var subscriptions = (List<IDisposable>)fieldInfo.GetValue(_autoSaver);
            CollectionAssert.AreEqual(new[] { subscription }, subscriptions);
            subscription.Dispose();
            CollectionAssert.IsEmpty(subscriptions);
        }

        private void AutoSaverOnSaved(object sender, SaveEventArgs saveEventArgs)
        {
            _resetEvent.Set();
        }
    }
}
