using System;
using System.Reactive;

namespace Simple.Wpf.Template.ViewModels
{
    public interface IAddResourceViewModel : ICloseableViewModel
    {
        string Path { get; set; }

        string Json { get; set; }

        IObservable<Unit> Added { get; }
    }
}