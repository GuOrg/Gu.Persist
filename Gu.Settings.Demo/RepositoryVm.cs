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
        private RepositoryVm()
        {
            this.Repository = CreateJsonRepositoryWithGitBackuper();
            this.ManualSaveSetting = this.Repository.ReadOrCreate(() => (ManualSaveSetting)Activator.CreateInstance(typeof(ManualSaveSetting), true));
            this.AutoSaveSetting = this.Repository.ReadOrCreate(() => (AutoSaveSetting)Activator.CreateInstance(typeof(AutoSaveSetting), true));
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

        private static JsonRepository CreateJsonRepositoryWithGitBackuper()
        {
            var jsonSerializerSettings = JsonRepositorySettings.CreateDefaultJsonSettings();
            var jsonRepositorySettings = new JsonRepositorySettings(jsonSerializerSettings, true, true, null);
            var gitBackuper = new GitBackuper(jsonRepositorySettings.DirectoryPath);
            return new JsonRepository(jsonRepositorySettings, gitBackuper);
        }
    }
}
