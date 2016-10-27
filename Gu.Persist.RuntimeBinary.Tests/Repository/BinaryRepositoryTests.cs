namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    public abstract class BinaryRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(T item, FileInfo file)
        {
            BinaryFile.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return BinaryFile.Read<T>(file);
        }
    }
}