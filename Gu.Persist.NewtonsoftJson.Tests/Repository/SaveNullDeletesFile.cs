namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SaveNullDeletesFile : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = new RepositorySettings(
                this.Directory,
                Core.RepositorySettings.CreateDefaultJsonSettings(),
                false,
                true,
                BackupSettings.Create(this.Directory));
            return new SingletonRepository(settings);
        }
    }
}