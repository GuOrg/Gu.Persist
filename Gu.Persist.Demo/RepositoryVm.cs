namespace Gu.Persist.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Gu.Persist.Core;
    using Gu.Persist.Git;
    using Gu.Persist.NewtonsoftJson;
    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public sealed class RepositoryVm : INotifyPropertyChanged
    {
        private RepositoryVm()
        {
            this.Repository = CreateJsonRepositoryWithGitBackuper();
            this.ManualSaveSetting = this.Repository.ReadOrCreate(() => (ManualSaveSetting)Activator.CreateInstance(typeof(ManualSaveSetting), nonPublic: true)!);
            this.AutoSaveSetting = this.Repository.ReadOrCreate(() => (AutoSaveSetting)Activator.CreateInstance(typeof(AutoSaveSetting), nonPublic: true)!);
            this.AutoSaveSetting.PropertyChanged += (o, e) => this.Save((AutoSaveSetting)o);
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        public static RepositoryVm Instance { get; } = new();

        public IRepository Repository { get; }

        public ManualSaveSetting ManualSaveSetting { get; }

        public AutoSaveSetting AutoSaveSetting { get; }

        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        internal void Save<T>(T item)
        {
            this.Repository?.Save(item);
            this.Log.Add($"Saved: {typeof(T).Name}");
        }

        private static SingletonRepository CreateJsonRepositoryWithGitBackuper()
        {
            var jsonSerializerSettings = RepositorySettings.CreateDefaultJsonSettings();
            var settings = new RepositorySettings(
                directory: Directories.Default.FullName,
                jsonSerializerSettings: jsonSerializerSettings,
                isTrackingDirty: false,
                backupSettings: null);

            var gitBackuper = new GitBackuper(settings.Directory);
            return new SingletonRepository(settings, gitBackuper);
        }
    }
}
