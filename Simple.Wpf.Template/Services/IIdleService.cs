using System;
using System.Reactive;

namespace Simple.Wpf.Template.Services
{
    public interface IIdleService : IService
    {
        IObservable<Unit> Idling { get; }
    }
}