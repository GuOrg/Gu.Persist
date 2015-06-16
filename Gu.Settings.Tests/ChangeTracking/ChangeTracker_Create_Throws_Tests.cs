namespace Gu.Settings.Tests.ChangeTracking
{
    using System;
    using System.ComponentModel;

    using Gu.Settings.Tests.ChangeTracking.Helpers;

    using NUnit.Framework;

    public class ChangeTracker_Create_Throws_Tests
    {
        [Test]
        public void ThrowsOnIllegalObjectNested()
        {
            var item = new Dummy();
            var exception = Assert.Throws<ArgumentException>(() => Tracker.Track(item));
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void ThrowsOnIllegalEnumerableNested()
        {
            var item = new IllegalEnumerable();
            var exception = Assert.Throws<ArgumentException>(() => Tracker.Track(item));
            Console.WriteLine(exception.Message);
        }
    }
}