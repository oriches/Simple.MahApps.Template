namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;

    public interface IIdleService : IService
    {
        IObservable<Unit> Idling { get; }
    }
}