using System;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Services
{
    public interface IOverlayService : IService
    {
        IObservable<OverlayViewModel> Show { get; }

        void Post(string header, BaseViewModel viewModel, IDisposable lifetime);
    }
}