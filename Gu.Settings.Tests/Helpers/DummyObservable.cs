namespace Gu.Settings.Tests
{
    using System;

    public class DummySubject<T> : IObservable<T>, IObserver<T>, IDisposable
    {
        private IObserver<T> _observer; 
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_observer != null)
            {
                throw new InvalidOperationException();
            }
            _observer = observer;
            return this;
        }

        public void Dispose()
        {
        }

        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
