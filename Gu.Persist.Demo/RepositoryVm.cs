namespace Gu.Persist.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Git;
    using Gu.Persist.Core;

    using JetBrains.Annotations;
    using NewtonsoftJson;

    using RepositorySettings = Gu.Persist.NewtonsoftJson.RepositorySettings;

    public class RepositoryVm : INotifyPropertyChanged
    {
        private RepositoryVm()
        {
            this.Repository = CreateJsonRepositoryWithGitBackuper();
            this.ManualSaveSetting = this.Repository.ReadOrCreate(() => (ManualSaveSetting)Activator.CreateInstance(typeof(ManualSaveSetting), nonPublic: true));
            this.AutoSaveSetting = this.Repository.ReadOrCreate(() => (AutoSaveSetting)Activator.CreateInstance(typeof(AutoSaveSetting), nonPublic: true));
            this.AutoSaveSetting.PropertyChanged += (o, e) => this.Save((AutoSaveSetting)o);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static RepositoryVm Instance { get; } = new RepositoryVm();

        public IRepository Repository { get; }

        public ManualSaveSetting ManualSaveSetting { get; }

        public AutoSaveSetting AutoSaveSetting { get; }

        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        internal void Save<T>(T item)
        {
            this.Repository?.Save(item);
            this.Log.Add($"Saved: {typeof(T).Name}");
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
