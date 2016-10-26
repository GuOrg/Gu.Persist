namespace Gu.Settings.Demo
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Git;
    using Gu.Settings.Core;

    using JetBrains.Annotations;
    using NewtonsoftJson;

    public class RepositoryVm : INotifyPropertyChanged
    {
        public static RepositoryVm Instance { get; } = new RepositoryVm();

        private RepositoryVm()
        {
            this.Repository = CreateJsonRepositoryWithGitBackuper();
            this.ManualSaveSetting = this.Repository.ReadOrCreate(() => (ManualSaveSetting)Activator.CreateInstance(typeof(ManualSaveSetting), true));
            this.AutoSaveSetting = this.Repository.ReadOrCreate(() => (AutoSaveSetting)Activator.CreateInstance(typeof(AutoSaveSetting), true));
            this.AutoSaveSetting.PropertyChanged += (o, e) => this.Save((AutoSaveSetting)o);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IRepository Repository { get; }

        public ManualSaveSetting ManualSaveSetting { get; }

        public AutoSaveSetting AutoSaveSetting { get; }

        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Save<T>(T item)
        {
            this.Repository?.Save(item);
            this.Log.Add($"Saved: {typeof(T).Name}");
        }

        private static JsonRepository CreateJsonRepository()
        {
            var jsonSerializerSettings = JsonRepositorySettings.DefaultJsonSettings;
            var backupSettings = new BackupSettings(Directories.DefaultBackup, 10, 2);
            var jsonRepositorySettings = new JsonRepositorySettings(jsonSerializerSettings, true, true, backupSettings);
            return new JsonRepository(jsonRepositorySettings);
        }

        private static JsonRepository CreateJsonRepositoryWithGitBackuper()
        {
            var jsonSerializerSettings = JsonRepositorySettings.DefaultJsonSettings;
            var jsonRepositorySettings = new JsonRepositorySettings(jsonSerializerSettings, true, true, null);
            var gitBackuper = new GitBackuper(jsonRepositorySettings.DirectoryPath);
            return new JsonRepository(jsonRepositorySettings, gitBackuper);
        }
    }
}
