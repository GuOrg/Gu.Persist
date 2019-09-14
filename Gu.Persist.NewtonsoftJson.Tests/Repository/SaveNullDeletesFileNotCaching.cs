namespace Gu.Persist.NewtonsoftJson.Tests.Repository
{
    using Gu.Persist.Core;

    using DataRepositorySettings = Gu.Persist.NewtonsoftJson.DataRepositorySettings;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class SaveNullDeletesFileNoCaching : JsonRepositoryTests
    {
        protected override IRepository CreateRepository()
        {
            var settings = new DataRepositorySettings(
                               directory: this.Directory.FullName,
                               jsonSerializerSettings: RepositorySettings.CreateDefaultJsonSettings(),
                               isTrackingDirty: false,
                               saveNullDeletesFile: true,
                               backupSettings: Default.BackupSettings(this.Directory));
            return new DataRepository(settings);
        }
    }
}