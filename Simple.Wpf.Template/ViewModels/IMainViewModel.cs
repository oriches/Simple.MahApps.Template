namespace Simple.Wpf.Template.ViewModels
{
    using System.Windows.Input;

    public interface IMainViewModel : IViewModel
    {
        ICommand MessageCommand { get; }
        IDiagnosticsViewModel Diagnostics { get; }
    }
}