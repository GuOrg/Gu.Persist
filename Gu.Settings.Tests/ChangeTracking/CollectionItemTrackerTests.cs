namespace Gu.Settings.Tests.ChangeTracking
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Reflection;

    using Gu.Settings.Tests.ChangeTracking.Helpers;

    using NUnit.Framework;

    public class CollectionItemTrackerTests
    {
        public static readonly PropertyInfo DummyPropertyInfo = typeof(List<int>).GetProperty("Count");
        private List<PropertyChangedEventArgs> _changes;

        [SetUp]
        public void SetUp()
        {
            _changes = new List<PropertyChangedEventArgs>();
        }

        [Test]
        public void NotifiesOnCollectionChanged()
        {
            var ints = new ObservableCollection<int>();
            using (var tracker = new CollectionTracker(typeof(CollectionItemTrackerTests), DummyPropertyInfo, ints, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;

                ints.Add(1);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);
                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        [Test]
        public void NotifiesOnCollectionItemChanged()
        {
            var items = new ObservableCollection<Level>();
            using (var tracker = new CollectionTracker(typeof(CollectionItemTrackerTests), DummyPropertyInfo, items, ChangeTrackerSettings.Default))
            {
                tracker.PropertyChanged += TrackerOnPropertyChanged;

                var item = new Level();
                items.Add(item);
                Assert.AreEqual(1, tracker.Changes);
                Assert.AreEqual(1, _changes.Count);

                item.Value++;
                Assert.AreEqual(2, tracker.Changes);
                Assert.AreEqual(2, _changes.Count);

                tracker.PropertyChanged -= TrackerOnPropertyChanged;
            }
        }

        private void TrackerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changes.Add(e);
        }
    }
}
