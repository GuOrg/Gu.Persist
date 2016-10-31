namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    public class SaveNullDeletesFile : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = new JsonRepositorySettings(
                this.Directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                false,
                true,
                BackupSettings.Create(this.Directory));
            return new JsonRepository(settings);
        }
    }
}