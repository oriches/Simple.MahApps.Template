using System;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.Services
{
    public interface IDiagnosticsService : IService
    {
        IObservable<string> Log { get; }

        IObservable<Memory> Memory { get; }

        IObservable<int> Cpu { get; }
    }
}