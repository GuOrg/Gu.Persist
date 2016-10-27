namespace Gu.Persist.Core.Tests.Sandbox
{
    using System.Threading;

    using NUnit.Framework;

    [Explicit]
    public class SyncBoxTests
    {
        [Test]
        public void Meh()
        {
            var slim = new SemaphoreSlim(1, 1);
            for (int i = 0; i < 10; i++)
            {
                slim.Wait();
                slim.Release();
            }
        }

        [Test]
        public void TestName()
        {
            int n = 10;
            var slim = new SemaphoreSlim(1, 1);
            for (int i = 0; i < n; i++)
            {
                slim.Wait();
            }

            for (int i = 0; i < n; i++)
            {
                slim.Release();
            }
        }

        [Test]
        public void MonitorMany()
        {
            int n = 10;
            var @lock = new object();
            for (int i = 0; i < n; i++)
            {
                Monitor.Enter(@lock);
            }

            for (int i = 0; i < n; i++)
            {
                Monitor.Exit(@lock);
            }
        }
    }
}
