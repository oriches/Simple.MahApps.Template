using System;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Services
{
    public interface IMessageService : IService
    {
        IObservable<Message> Show { get; }

        void Post(string header, ICloseableViewModel viewModel);
    }
}