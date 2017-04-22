namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SaveNullDeletesFileCaching : JsonRepositoryTests
    {
        protected override IRepository Create()
        {
            var settings = new NewtonsoftJson.DataRepositorySettings(
                directory: this.TargetDirectory.FullName,
                jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                isTrackingDirty: true,
                saveNullDeletesFile: true,
                backupSettings: Default.BackupSettings(this.TargetDirectory));
            return new DataRepository(settings);
        }
    }
}