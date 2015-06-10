namespace Gu.Wpf.Settings.Demo
{
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Gu.Settings;
    using Gu.Settings.Annotations;

    public class FakeRepository : IRepository, INotifyPropertyChanged
    {
        private string _xml;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Xml
        {
            get { return _xml; }
            set
            {
                if (value == _xml)
                {
                    return;
                }
                _xml = value;
                OnPropertyChanged();
            }
        }

        public T Read<T>(string fileName)
        {
            throw new System.NotImplementedException();
        }

        public void Save<T>(T setting, bool createBackup, string fileName = null)
        {
            throw new System.NotImplementedException();
        }

        public IRepositorySetting Setting { get; private set; }

        public T Read<T>(FileInfo file)
        {
            throw new System.NotImplementedException();
        }

        public void Save<T>(T setting, FileInfo file)
        {
            throw new System.NotImplementedException();
        }

        public void Save<T>(T setting, FileInfos fileInfos)
        {
            throw new System.NotImplementedException();
        }

        public void Save<T>(T setting, string fileName)
        {
            throw new System.NotImplementedException();
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
