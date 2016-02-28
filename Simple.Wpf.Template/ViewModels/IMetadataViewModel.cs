namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using Commands;
    using Models;

    public interface IMetadataViewModel : IDisposable
    {
        Uri Url { get; }
        bool Editable { get; }
        Metadata Metadata { get; }
        ReactiveCommand<object> ModifyCommand { get; }
        ReactiveCommand<object> DeleteCommand { get; }
        IObservable<Unit> Deleted { get; } 
    }
}