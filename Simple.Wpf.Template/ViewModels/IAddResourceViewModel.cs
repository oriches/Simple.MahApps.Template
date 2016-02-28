namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;

    public interface IAddResourceViewModel : ICloseableViewModel
    {
        string Path { get; set; }

        string Json { get; set; }

        IObservable<Unit> Added { get; } 
    }
}