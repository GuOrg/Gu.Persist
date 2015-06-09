namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading;

    using Gu.Reactive;

    public class AutoSaver : IDisposable
    {
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler(CreateThread);
        private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
        private bool _disposed = false;

        public void Add<T>(T item, IRepository repository, AutoSaveSetting setting) 
            where T : INotifyPropertyChanged
        {
            _subscriptions.Add(CreateSubscription(item, repository, setting));
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _scheduler.Dispose();
                _subscriptions.Dispose();
            }

            // Free any unmanaged objects here. 
            _disposed = true;
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        private static Thread CreateThread(ThreadStart threadStart)
        {
            return new Thread(threadStart) { Name = "Autosaver thread", IsBackground = true };
        }

        private IDisposable CreateSubscription<T>(T item, IRepository repository, AutoSaveSetting setting)
            where T : INotifyPropertyChanged
        {
            switch (setting.Mode)
            {
                case AutoSaveMode.OnChanged:
                    return item.ObservePropertyChanged()
                               .ObserveOn(_scheduler)
                               .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                case AutoSaveMode.Deferred:
                    return item.ObservePropertyChanged()
                               .ObserveOn(_scheduler)
                               .Throttle(setting.Time)
                               .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                case AutoSaveMode.OnSchedule:
                    return Observable.Timer(setting.Time, setting.Time)
                                     .ObserveOn(_scheduler)
                                     .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                case AutoSaveMode.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
