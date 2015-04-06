namespace Gu.Settings
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading;

    using Gu.Reactive;

    public class AutoSaver
    {
        private readonly EventLoopScheduler _scheduler = new EventLoopScheduler(CreateThread);

        public void Add<T>(T item, IRepository repository, AutoSaveSetting setting) where T : INotifyPropertyChanged
        {
            switch (setting.Mode)
            {
                case AutoSaveMode.OnChanged:
                    item.ObservePropertyChanged()
                        .ObserveOn(_scheduler)
                        .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                    break;
                case AutoSaveMode.Deferred:
                    item.ObservePropertyChanged()
                        .ObserveOn(_scheduler)
                        .Throttle(setting.Time)
                        .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                    break;
                case AutoSaveMode.OnSchedule:
                    Observable.Timer(setting.Time, setting.Time)
                              .ObserveOn(_scheduler)
                              .Subscribe(_ => repository.Save(item, setting.CreateBackup, setting.FileName));
                    break;
                case AutoSaveMode.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static Thread CreateThread(ThreadStart threadStart)
        {
            return new Thread(threadStart) { Name = "Autosaver thread", IsBackground = true };
        }
    }
}
