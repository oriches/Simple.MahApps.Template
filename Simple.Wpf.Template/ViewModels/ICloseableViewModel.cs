namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Windows.Input;

    public interface ICloseableViewModel : IViewModel
    {
        IObservable<Unit> CloseRequested { get; }
        ICommand CloseCommand { get; }
    }
}