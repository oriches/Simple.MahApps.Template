using System;
using System.Reactive;

namespace Simple.Wpf.Template.Services
{
    public interface IHeartbeatService : IService
    {
        IObservable<Unit> Listen { get; }
    }
}