using Simple.Wpf.Template.Commands;

namespace Simple.Wpf.Template.ViewModels
{
    public interface IMainViewModel : IViewModel
    {
        ReactiveCommand<object> RefreshCommand { get; }
        IDiagnosticsViewModel Diagnostics { get; }
    }
}