namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using Commands;

    public interface ICloseableViewModel : IViewModel
    {
        IObservable<Unit> Closed { get; }
        ReactiveCommand<object> ConfirmCommand { get; }
        ReactiveCommand<object> DenyCommand { get; }
        ReactiveCommand<object> CancelCommand { get; }
    }
}