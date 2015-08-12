namespace Gu.Settings.Core.Tests.ChangeTracking
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Gu.Settings.Core;
    using Gu.Settings.Core.Tests.ChangeTracking.Helpers;

    using NUnit.Framework;

    // ReSharper disable once TestClassNameDoesNotMatchFileNameWarning
    // ReSharper disable once InconsistentNaming
    public class ChangeTracker_PropertyChanged_Tests
    {
        private List<PropertyChangedEventArgs> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<PropertyChangedEventArgs>();
        }

        [Test]
        public void NotifiesOnCurrentLevelAndStopsOnDisposed()
        {
            var root = new Level();
            using (var tracker = PropertyChangeTracker.Track(root))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
                
                root.Value++;
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);
                
                tracker.Dispose();
                root.Value++;
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);
              
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesNextLevel()
        {
            var level = new Level { Next = new Level() };
            using (var tracker = PropertyChangeTracker.Track(level))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;

                level.Next.Value++;
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);
                
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesThreeLevels()
        {
            var level = new Level { Next = new Level { Next = new Level() } };
            using (var tracker = PropertyChangeTracker.Track(level))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
               
                level.Next.Value++;
                Assert.AreEqual(1, tracker.Changes);                
                Assert.AreEqual(1, _changes.Count);
                
                level.Next.Next.Value++;
                Assert.AreEqual(2, tracker.Changes);                
                Assert.AreEqual(2, _changes.Count);

                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void StartSubscribingToNextLevel()
        {
            var level = new Level();
            using (var tracker = PropertyChangeTracker.Track(level))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
                
                level.Next = new Level();
                Assert.AreEqual(1, tracker.Changes);                
                Assert.AreEqual(1, _changes.Count);

                level.Next.Value++;
                Assert.AreEqual(2, tracker.Changes);                
                Assert.AreEqual(2, _changes.Count);

                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void StopsSubscribingNextLevel()
        {
            var level = new Level { Next = new Level() };
            using (var tracker = PropertyChangeTracker.Track(level))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
                
                var next = level.Next;
                level.Next = null;
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);

                next.Value++;
                Assert.AreEqual(1, tracker.Changes);                
                Assert.AreEqual(1, _changes.Count);
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        private void TrackerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changes.Add(e);
        }
    }
}
