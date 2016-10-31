namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using System.IO;
    using Gu.Persist.Core.Tests.Repositories;

    using File = Gu.Persist.RuntimeBinary.File;

    public abstract class BinaryRepositoryTests : RepositoryTests
    {
        protected override void Save<T>(FileInfo file, T item)
        {
            File.Save(file, item);
        }

        protected override T Read<T>(FileInfo file)
        {
            return File.Read<T>(file);
        }
    }
}