namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Commands;
    using Extensions;

    public abstract class CloseableViewModel : BaseViewModel, ICloseableViewModel
    {
        private readonly Subject<Unit> _closed;
        private readonly Subject<Unit> _deny;
        private readonly Subject<Unit> _confirm;

        protected CloseableViewModel()
        {
            _closed = new Subject<Unit>()
                .DisposeWith(this);

            _deny = new Subject<Unit>()
               .DisposeWith(this);

            _confirm = new Subject<Unit>()
               .DisposeWith(this);

            CancelCommand = ReactiveCommand.Create()
                .DisposeWith(this);

            CancelCommand.ActivateGestures()
                .Subscribe(x => _closed.OnNext(Unit.Default))
                .DisposeWith(this);

            InitialiseConfirmAndDeny();

            ConfirmCommand.ActivateGestures()
                .Subscribe(x =>
            {
                _confirm.OnNext(Unit.Default);
                _closed.OnNext(Unit.Default);
            })
            .DisposeWith(this);

            DenyCommand.ActivateGestures()
                .Subscribe(x =>
            {
                _deny.OnNext(Unit.Default);
                _closed.OnNext(Unit.Default);
            })
            .DisposeWith(this);
        }

        public IObservable<Unit> Closed => _closed;
        public IObservable<Unit> Deny => _deny;
        public IObservable<Unit> Confirm => _confirm;
        public ReactiveCommand<object> CancelCommand { get; }
        public ReactiveCommand<object> ConfirmCommand { get; protected set; }
        public ReactiveCommand<object> DenyCommand { get; protected set; }

        protected abstract void InitialiseConfirmAndDeny();
    }
}