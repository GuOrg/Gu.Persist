// ReSharper disable UnusedMember.Global
namespace Gu.Persist.RuntimeBinary.Tests.Repository
{
    using Gu.Persist.Core;

    public class BackupInSubDirrectory : BinaryRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = BinaryRepositorySettings.DefaultFor(this.Directory);
            return new BinaryRepository(settings);
        }
    }
}