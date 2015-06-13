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
        private RepositorySetting _setting;
        private IRepository _selectedRepository;

        private RepositoriesVm()
        {
            var directory = Directories.ExecutingDirectory.Subdirectory("Settings");
            var backupSettings = new BackupSettings(true, directory, ".bak", BackupSettings.DefaultTimeStampFormat, false, 5, 5);
            _setting = new RepositorySetting(directory, backupSettings);
            _repositories = new IRepository[]
                                {
                                    new JsonRepository(_setting),
                                    new XmlRepository(_setting), 
                                    new BinaryRepository(_setting), 
                                };
            var autoSaveSetting = AutoSaveSetting.Instance;
            autoSaveSetting.PropertyChanged += (o, e) => Save((AutoSaveSetting)o);
            PropertyChanged += (_, __) => Save(autoSaveSetting);
        }

        private void Save<T>(T item)
        {
            if (SelectedRepository != null)
            {
                SelectedRepository.Save(item);
            }
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

        public RepositorySetting Setting
        {
            get { return _setting; }
            set
            {
                if (Equals(value, _setting))
                {
                    return;
                }
                _setting = value;
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
    }
}
