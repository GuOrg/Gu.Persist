namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    public class SaveNullDeletesFileNoCaching : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = new JsonRepositorySettings(
                this.Directory,
                JsonRepositorySettings.CreateDefaultJsonSettings(),
                false,
                false,
                true,
                BackupSettings.DefaultFor(this.Directory));
            return
                new JsonRepository(settings);
        }
    }
}