namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SaveNullDeletesFileCaching : JsonRepositoryTests
    {
        protected override IRepository CreateRepository()
        {
            var settings = new NewtonsoftJson.DataRepositorySettings(
                directory: this.Directory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: true,
                saveNullDeletesFile: true,
                backupSettings: Default.BackupSettings(this.Directory));
            return new DataRepository(settings);
        }
    }
}