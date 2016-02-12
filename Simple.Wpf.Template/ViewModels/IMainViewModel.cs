namespace Simple.Wpf.Template.ViewModels
{
    using System.Windows.Input;
    using Commands;

    public interface IMainViewModel : IViewModel
    {
        ReactiveCommand<object> MessageCommand { get; }
        IDiagnosticsViewModel Diagnostics { get; }
    }
}