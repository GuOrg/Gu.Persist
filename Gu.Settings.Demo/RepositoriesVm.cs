namespace Gu.Settings.Demo
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings.Annotations;
    using Gu.Settings.IO;
    using Gu.Settings.Json;

    public class RepositoriesVm : INotifyPropertyChanged
    {
        public static RepositoriesVm Instance = new RepositoriesVm();
        private readonly IReadOnlyList<IRepository> _repositories;
        private RepositorySettings _settings;
        private IRepository _selectedRepository;

        private RepositoriesVm()
        {
            var directory = Directories.ExecutingDirectory.Subdirectory("Settings");
            var backupSettings = new BackupSettings(true, directory, ".bak", BackupSettings.DefaultTimeStampFormat, false, 5, 5);
            _settings = new RepositorySettings(directory, backupSettings);
            _repositories = new IRepository[]
                                {
                                    new JsonRepository(_settings),
                                    new XmlRepository(_settings), 
                                    new BinaryRepository(_settings), 
                                };
            var autoSaveSetting = AutoSaveSetting.Instance;
            autoSaveSetting.PropertyChanged += (o, e) => Save((AutoSaveSetting)o);
            PropertyChanged += (_, __) => Save(autoSaveSetting);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IRepository SelectedRepository
        {
            get { return _selectedRepository; }
            set
            {
                if (Equals(value, _selectedRepository)) return;
                _selectedRepository = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyList<IRepository> Repositories
        {
            get { return _repositories; }
        }

        public RepositorySettings Settings
        {
            get { return _settings; }
            set
            {
                if (Equals(value, _settings))
                {
                    return;
                }
                _settings = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Save<T>(T item)
        {
            if (SelectedRepository != null)
            {
                SelectedRepository.Save(item);
            }
        }
    }
}
