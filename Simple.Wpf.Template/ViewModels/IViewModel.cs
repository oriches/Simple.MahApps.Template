namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.ComponentModel;

    public interface IViewModel : INotifyPropertyChanged, IDisposable
    {
        IDisposable SuspendNotifications();
    }
}