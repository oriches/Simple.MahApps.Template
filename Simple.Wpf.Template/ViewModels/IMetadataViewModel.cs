using System;
using System.Reactive;
using Simple.Wpf.Template.Commands;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.ViewModels
{
    public interface IMetadataViewModel : ITransientViewModel
    {
        Uri Url { get; }
        bool Editable { get; }
        Metadata Metadata { get; }
        ReactiveCommand<object> ModifyCommand { get; }
        ReactiveCommand<object> DeleteCommand { get; }
        IObservable<Unit> Deleted { get; }
    }
}