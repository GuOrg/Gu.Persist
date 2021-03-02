namespace Gu.Persist.Core.Tests.Repositories
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        [TestCaseSource(nameof(TestCases))]
        public void Read(TestCase testCase)
        {
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = testCase.Read<DummySerializable>(repository, file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read1 = testCase.Read<DummySerializable>(repository, file);
            var read2 = testCase.Read<DummySerializable>(repository, file);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileDoesNotExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read = testCase.ReadOrCreate(repository, file, () => dummy);
            Assert.AreSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileExistsManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read1 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
            var read2 = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenFileDoesNotExistsManagesSingleton(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read1 = testCase.ReadOrCreate(repository, file, () => dummy);
            var read2 = testCase.ReadOrCreate(repository, file, () => dummy);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void IsNotDirtyAfterRead(TestCase testCase)
        {
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, new DummySerializable(1));
            if (repository.Settings.IsTrackingDirty)
            {
                var read = testCase.Read<DummySerializable>(repository, file);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));

                read.Value++;
                Assert.AreEqual(true, testCase.IsDirty(repository, file, read));

                testCase.Save(repository, file, read);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));
            }
            else
            {
                var exception = Assert.Throws<InvalidOperationException>(() => repository.IsDirty(new DummySerializable(1)));
                Assert.AreEqual("This repository is not tracking dirty.", exception!.Message);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void IsNotDirtyAfterReadOrCreateWhenFileExists(TestCase testCase)
        {
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, new DummySerializable(1));
            if (repository.Settings.IsTrackingDirty)
            {
                var read = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."));
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));

                read.Value++;
                Assert.AreEqual(true, testCase.IsDirty(repository, file, read));

                testCase.Save(repository, file, read);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));
            }
            else
            {
                var exception = Assert.Throws<InvalidOperationException>(() => repository.IsDirty(new DummySerializable(1)));
                Assert.AreEqual("This repository is not tracking dirty.", exception!.Message);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void IsNotDirtyAfterReadOrCreateWhenFileDoesNotExists(TestCase testCase)
        {
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            if (repository.Settings.IsTrackingDirty)
            {
                var read = testCase.ReadOrCreate(repository, file, () => new DummySerializable(1));
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));

                read.Value++;
                Assert.AreEqual(true, testCase.IsDirty(repository, file, read));

                testCase.Save(repository, file, read);
                Assert.AreEqual(false, testCase.IsDirty(repository, file, read));
            }
            else
            {
                var exception = Assert.Throws<InvalidOperationException>(() => repository.IsDirty(new DummySerializable(1)));
                Assert.AreEqual("This repository is not tracking dirty.", exception!.Message);
            }
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadWhenNoMigration(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var migration = new NoMigration();
            var read = testCase.Read<DummySerializable>(repository, file, migration);
            Assert.AreEqual(1, read.Value);
            Assert.AreEqual(true, migration.WasCalled);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadWhenIdentityMigration(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var migration = new IdentityMigration();
            var read = testCase.Read<DummySerializable>(repository, file, migration);
            Assert.AreEqual(1, read.Value);
            Assert.AreEqual(true, migration.WasCalled);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenNoMigrationWhenFileExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var read = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."), new NoMigration());
            Assert.AreEqual(1, read.Value);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenIdentityMigrationWhenFileExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            this.Save(file, dummy);
            var migration = new IdentityMigration();
            var read = testCase.ReadOrCreate<DummySerializable>(repository, file, () => throw new AssertionException("Should not get here."), migration);
            Assert.AreEqual(1, read.Value);
            Assert.AreEqual(true, migration.WasCalled);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenNoMigrationWhenFileDoesNotExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read = testCase.ReadOrCreate(repository, file, () => dummy, new NoMigration());
            Assert.AreSame(dummy, read);
        }

        [TestCaseSource(nameof(TestCases))]
        public void ReadOrCreateWhenIdentityMigrationWhenFileDoesNotExists(TestCase testCase)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = testCase.File<DummySerializable>(repository);
            var read = testCase.ReadOrCreate(repository, file, () => dummy, new IdentityMigration());
            Assert.AreSame(dummy, read);
        }

        private class NoMigration : Migration
        {
#pragma warning disable SA1401 // Fields should be private
            internal bool WasCalled;
#pragma warning restore SA1401 // Fields should be private

            public override bool TryUpdate(Stream stream, [NotNullWhen(true)] out Stream? updated)
            {
                this.WasCalled = true;
                updated = null;
                return false;
            }
        }

        private class IdentityMigration : Migration
        {
#pragma warning disable SA1401 // Fields should be private
            internal bool WasCalled;
#pragma warning restore SA1401 // Fields should be private

            public override bool TryUpdate(Stream stream, out Stream updated)
            {
                this.WasCalled = true;
                updated = PooledMemoryStream.Borrow();
                stream.CopyTo(updated);
                updated.Position = 0;
                return true;
            }
        }
    }
}