namespace Gu.Settings
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Annotations;
    using Reactive;

    public class SettingCollection : IEnumerable<ISetting>, INotifyPropertyChanged
    {
        private readonly List<ISetting> _inner = new List<ISetting>();

        public event PropertyChangedEventHandler PropertyChanged;

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsDirty { get; set; }

        public IEnumerator<ISetting> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISetting item)
        {
            item.ObservePropertyChanged(x => x.IsDirty)
                .Subscribe(_ => IsDirty = this.Any(x => x.IsDirty));
            _inner.Add(item);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
