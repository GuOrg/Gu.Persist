namespace Gu.Persist.Core.Tests.Repositories
{
    using System.Threading.Tasks;
    using NUnit.Framework;

    public abstract partial class RepositoryTests
    {
        [Test]
        public void ReadFileInfo()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = repository.Read<DummySerializable>(file);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadName()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = repository.Read<DummySerializable>(file.Name);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadFullName()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = repository.Read<DummySerializable>(file.FullName);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public void ReadGeneric()
        {
            var repository = this.CreateRepository();
            var dummy = new DummySerializable(1);
            var file = repository.GetGenericTestFileInfo(dummy);
            this.Save(file, dummy);
            var read = repository.Read<DummySerializable>();
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncFileInfo()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = await repository.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncName()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = await repository.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncFullName()
        {
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            var dummy = new DummySerializable(1);
            this.Save(file, dummy);
            var read = await repository.ReadAsync<DummySerializable>(file.FullName).ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [Test]
        public async Task ReadAsyncGeneric()
        {
            var repository = this.CreateRepository();
            var dummy = new DummySerializable(1);
            var file = repository.GetGenericTestFileInfo(dummy);
            this.Save(file, dummy);
            var read = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            Assert.AreEqual(dummy.Value, read.Value);
            Assert.AreNotSame(dummy, read);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFileInfo(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateFullName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file.FullName, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateName(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(file.Name, () => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ReadOrCreateGeneric(bool exists)
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);

            if (exists)
            {
                this.Save(file, dummy);
            }

            var read = repository.ReadOrCreate(() => dummy);
            AssertFile.Exists(true, file);
            if (exists)
            {
                Assert.AreEqual(dummy.Value, read.Value);
                Assert.AreNotSame(dummy, read);
            }
            else
            {
                Assert.AreSame(dummy, read);
            }
        }

        [Test]
        public void ReadFileInfoCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = repository.Read<DummySerializable>(file);
            var read2 = repository.Read<DummySerializable>(file);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadFullNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = repository.Read<DummySerializable>(file.FullName);
            var read2 = repository.Read<DummySerializable>(file.FullName);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = repository.Read<DummySerializable>(file.Name);
            var read2 = repository.Read<DummySerializable>(file.Name);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public void ReadGenericCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            this.Save(file, dummy);
            var read1 = repository.Read<DummySerializable>();
            var read2 = repository.Read<DummySerializable>();
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncFileInfoCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(file).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncFullNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(file.FullName).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(file.FullName).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncNameCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetTestFileInfo();
            this.Save(file, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>(file.Name).ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }

        [Test]
        public async Task ReadAsyncGenericCaches()
        {
            var dummy = new DummySerializable(1);
            var repository = this.CreateRepository();
            var file = repository.GetGenericTestFileInfo(dummy);
            this.Save(file, dummy);
            var read1 = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            var read2 = await repository.ReadAsync<DummySerializable>().ConfigureAwait(false);
            if (repository is ISingletonRepository)
            {
                Assert.AreSame(read1, read2);
            }
            else
            {
                Assert.AreNotSame(read1, read2);
            }
        }
    }
}