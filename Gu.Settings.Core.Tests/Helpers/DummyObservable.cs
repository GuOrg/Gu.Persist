namespace Gu.Settings.Core.Tests
{
    using System;

    public class DummySubject<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private IObserver<T> observer; 
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (this.observer != null)
            {
                throw new InvalidOperationException();
            }

            this.observer = observer;
            return this;
        }

        public void Dispose()
        {
        }

        public void OnNext(T value)
        {
            this.observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            throw new NotSupportedException();
        }

        public void OnCompleted()
        {
            throw new NotSupportedException();
        }
    }
}
