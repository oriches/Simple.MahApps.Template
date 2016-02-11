namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Subjects;
    using System.Windows.Input;
    using Commands;
    using Extensions;
    using Services;

    public abstract class CloseableViewModel : BaseViewModel, ICloseableViewModel
    {
        private readonly Subject<Unit> _closeRequested;
        private readonly IDisposable _disposable;

        protected CloseableViewModel()
        {
            _closeRequested = new Subject<Unit>()
                .DisposeWith(this);

            CloseCommand = new RelayCommand(() => _closeRequested.OnNext(Unit.Default));

            _disposable = Disposable.Create(() =>
            {
                CloseCommand = null;
            });
        }

        public IObservable<Unit> CloseRequested => _closeRequested;

        public ICommand CloseCommand { get; private set; }
        
        public override void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                base.Dispose();

                _disposable.Dispose();
            }
        }

        protected void Close()
        {
            CloseCommand.Execute(null);
        }
    }
}