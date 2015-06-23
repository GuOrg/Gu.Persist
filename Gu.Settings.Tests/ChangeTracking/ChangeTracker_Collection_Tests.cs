namespace Gu.Settings.Tests.ChangeTracking
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gu.Settings.Tests.ChangeTracking.Helpers;

    using NUnit.Framework;

    public class ChangeTracker_Collection_Tests
    {
        private List<PropertyChangedEventArgs> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<PropertyChangedEventArgs>();
        }

        [Test]
        public void NotifiesOnAddInt()
        {
            var root = new Level();
            using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
                
                root.Ints.Add(1);
                Assert.AreEqual(1, tracker.Changes);  
                Assert.AreEqual(1, _changes.Count);
                
                root.Ints = new ObservableCollection<int>();
                Assert.AreEqual(2, tracker.Changes);  
                Assert.AreEqual(2, _changes.Count);

                root.Ints.Add(1);
                Assert.AreEqual(3, tracker.Changes);  
                Assert.AreEqual(3, _changes.Count);

                tracker.Dispose();
                root.Ints.Add(2);
                Assert.AreEqual(3, tracker.Changes);  
                Assert.AreEqual(3, _changes.Count);
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesOnAdd()
        {
            var root = new Level();
            using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;

                var level = new Level();
                root.Levels.Add(level);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);

                level.Value ++;
                Assert.AreEqual(2, tracker.Changes);
                Assert.AreEqual(2, _changes.Count);

                root.Levels.Add(level);
                Assert.AreEqual(3, tracker.Changes);
                Assert.AreEqual(3, _changes.Count);

                root.Levels.Add(new Level());
                Assert.AreEqual(4, tracker.Changes);
                Assert.AreEqual(4, _changes.Count);

                root.Levels = new ObservableCollection<Level>();
                Assert.AreEqual(5, tracker.Changes);
                Assert.AreEqual(5, _changes.Count);

                tracker.Dispose();

                level.Value++;
                Assert.AreEqual(5, tracker.Changes);
                Assert.AreEqual(5, _changes.Count);

                root.Levels.Add(new Level());
                Assert.AreEqual(5, tracker.Changes);
                Assert.AreEqual(5, _changes.Count);
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesOnAddSpecialCollection()
        {
            var root = new SpecialCollection();
            using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;

                var level = new Level();
                root.Add(level);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);

                level.Value++;
                Assert.AreEqual(2, tracker.Changes);
                Assert.AreEqual(2, _changes.Count);

                root.Remove(level);
                Assert.AreEqual(3, tracker.Changes);
                Assert.AreEqual(3, _changes.Count);

                level.Value++;
                Assert.AreEqual(3, tracker.Changes);
                Assert.AreEqual(3, _changes.Count);
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void TracksAddedStopsOnRemoved()
        {
            var root = new Level();
            using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
                
                var level = new Level();
                root.Levels.Add(level);
                Assert.AreEqual(1, tracker.Changes);  
                Assert.AreEqual(1, _changes.Count);
           
                level.Value++;
                Assert.AreEqual(2, tracker.Changes);  
                Assert.AreEqual(2, _changes.Count);
               
                root.Levels.Clear();
                Assert.AreEqual(3, tracker.Changes);
                Assert.AreEqual(3, _changes.Count);

                level.Value++;
                Assert.AreEqual(3, tracker.Changes);                
                Assert.AreEqual(3, _changes.Count);

                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesThreeLevels()
        {
            var root = new Level { Next = new Level { Next = new Level() } };
            using (var tracker = ChangeTracker.Track(root, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;
               
                var level = new Level();
                root.Levels.Add(level);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);

                level.Next = new Level();
                Assert.AreEqual(2, tracker.Changes);
                Assert.AreEqual(2, _changes.Count);

                level.Next.Value++;
                Assert.AreEqual(3, tracker.Changes);               
                Assert.AreEqual(3, _changes.Count);

                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        private void TrackerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changes.Add(e);
        }
    }
}