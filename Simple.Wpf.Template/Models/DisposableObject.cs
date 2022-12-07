using System;
using System.Reactive.Disposables;
using NLog;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Models
{
    public abstract class DisposableObject : IDisposable
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;

        protected DisposableObject() => _disposable = new CompositeDisposable();

        public virtual void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose - " + GetType()
                       .FullName))
            {
                _disposable.Dispose();
            }
        }

        public static implicit operator CompositeDisposable(DisposableObject disposable) => disposable._disposable;
    }
}