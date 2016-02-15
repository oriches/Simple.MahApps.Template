namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using Commands;

    public interface ICloseableViewModel : IViewModel
    {
        IObservable<Unit> Closed { get; }
        IObservable<Unit> Denied { get; }
        IObservable<Unit> Confirmed { get; }
    }
}