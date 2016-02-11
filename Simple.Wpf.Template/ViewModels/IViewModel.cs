namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.ComponentModel;

    public interface IViewModel : IDisposable, INotifyPropertyChanged
    {
        IDisposable SuspendNotifications();
    }
}