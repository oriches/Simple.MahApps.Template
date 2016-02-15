namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;

    public interface IHeartbeatService
    {
        IObservable<Unit> Listen { get; }
    }
}