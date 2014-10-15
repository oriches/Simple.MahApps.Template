namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using Commands;
    using NLog;

    public abstract class CloseableViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Subject<Unit> _closeRequested;
        private readonly IDisposable _disposable;

        protected CloseableViewModel()
        {
            _closeRequested = new Subject<Unit>();

            CloseCommand = new RelayCommand(() => _closeRequested.OnNext(Unit.Default));

            _disposable = Disposable.Create(() =>
            {
                CloseCommand = null;

                _closeRequested.OnCompleted();
                _closeRequested.Dispose();
            });
        }

        public IObservable<Unit> CloseRequested { get { return _closeRequested; } }

        public ICommand CloseCommand { get; private set; }
        
        public virtual void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        protected void Close()
        {
            CloseCommand.Execute(null);
        }
    }
}