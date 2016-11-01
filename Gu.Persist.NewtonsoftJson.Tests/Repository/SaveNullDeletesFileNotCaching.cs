namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using DataRepositorySettings = Gu.Persist.NewtonsoftJson.DataRepositorySettings;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SaveNullDeletesFileNoCaching : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = new DataRepositorySettings(
                               PathAndSpecialFolder.Create(this.TargetDirectory),
                               RepositorySettings.CreateDefaultJsonSettings(),
                               false,
                               true,
                               BackupSettings.Create(this.TargetDirectory));
            return new DataRepository(settings);
        }
    }
}